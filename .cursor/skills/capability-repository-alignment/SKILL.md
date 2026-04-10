---
name: capability-repository-alignment
description: >-
  Refactors a Symfony capability module so the domain stays rich and free of
  application DTOs, write persistence matches aggregate-persistence rules
  (factory query builders, mappers), and reads match read-models rules
  (readonly read models, query objects, row mappers). Use when aligning an
  existing capability with modular-monolith persistence conventions, after a
  plan similar to “domain + write repo + read repo” refactors, or when fixing
  raw Connection::createQueryBuilder / array-shaped read APIs in tenant-aware code.
---

# Capability alignment: domain richness + repository rules

Use this when **modernizing one capability** so it matches `.cursor/rules/aggregate-persistence.mdc`, `read-models.mdc`, and `modular-monolith.mdc` without leaking layers.

**Worked example to compare:** `src/Capability/UseCaseTables/` (structure factory, `deleteFromOwnerTable()` for a satellite table, `UseCaseTableReadModel`, `UseCaseTableReadRowMapper`, `Application/Queries/*`).

## Layer boundaries

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Aggregates, value objects, invariants, domain events. **Structure built from primitives** (tuples, scalars)—not `Application/…/*Input` DTOs. |
| **Application** | Use cases orchestrate bridges/facades, repos, sequences. **Map HTTP/use-case inputs → primitives** then call domain factories. |
| **Infrastructure** | Write/read repos, capability `*QueryBuilder`, **aggregate mapper (ObjectAccessor)** vs **read row mapper (constructors only)**. |

**Do not** move `StepInput`-style DTOs into `Domain/` or into `UseCaseTableStructureFactory`-style domain builders.

## Domain checklist

- [ ] Duplicated “build steps/pre/post/exceptions from arrays” logic lives in a **domain** helper (e.g. `*StructureFactory`) taking **`list<array{…}>`** shapes, used by create/update use cases.
- [ ] **Invariants** enforced on the aggregate (`create` / `update` share assertions, e.g. non-empty title).
- [ ] **Encapsulation:** internal collections not exposed for mutation (defensive copy / readonly projection where rules require it).
- [ ] **Author / actor rules** that need `OrganizationBridge` or auth stay in the **use case**; domain may use pure helpers (e.g. “ensure actor in list” + callable for display name only when adding).

## Write repository checklist (`aggregate-persistence.mdc`)

- [ ] Interface in `Domain/Repositories/` with only `persist`, `restore`, `remove`, `generateId` (no extras unless the module already diverged).
- [ ] `generateId(): <AggregateName>Id` (non-null); align legacy `?Id` interfaces/implementations with `aggregate-persistence.mdc`.
- [ ] **No aggregate construction** in the repository; **`<Aggregate>Mapper`** + ObjectAccessor for the write path.
- [ ] All DB access via **`QueryBuilderFactoryInterface` → capability `*QueryBuilder`**—avoid `$connection->createQueryBuilder()` for tenant-aware or multi-table flows unless justified.
- [ ] **`delete()` on shared `QueryBuilder`:** if the base `delete()` applies `filterByTenant()` using a column from the **main** table, **satellite tables** (e.g. `*_owner`) may need a **dedicated method** on the capability query builder that returns a plain DBAL delete builder, then add **`tenant_id` + business keys** explicitly in the repository.
- [ ] Inject **`TenantContextInterface`** when deletes/updates on child tables must be tenant-scoped without joining the parent.

## Read repository checklist (`read-models.mdc`)

- [ ] **`Application/ReadModels/`** — `readonly` classes, constructor-promoted properties, `JsonSerializable` with **camelCase** keys.
- [ ] **`Application/Queries/`** — `readonly` query objects (list filters, cursor + `FilterCriteria`, paging)—not mutable DTOs under `Repositories/`.
- [ ] **`Application/Repositories/*ReadRepositoryInterface`** returns read models / small result objects, not **raw row arrays** with JSON decoded in the repo.
- [ ] **`Infrastructure/.../Mapper/*ReadRowMapper`** — `map(array $row): ReadModel`; **no** ObjectAccessor on the read path.
- [ ] Repository: build query → fetch → **delegate mapping** to the read mapper.
- [ ] **Enrichment** from other capabilities: batch via **bridges** in the read repository implementation, not facades from use cases for pure reads.

## Application checklist

- [ ] **Shared input → line array** mapping: either duplicate tiny `array_map` in create/update or extract an **Application**-layer helper; if `StepInput` exists in two namespaces, **consolidate input types** in one Application namespace so one mapper type-hints cleanly.
- [ ] **Bridges** implemented under `Infrastructure/Bridges/`, interfaces under `Application/Bridges/`.

## PHPStan / PHPCBF gotchas

- [ ] `@return list<…>`: wrap `array_map` results with **`array_values(...)`** when keys are not guaranteed sequential.
- [ ] PHPDoc types only in docblocks: if **`use` imports are stripped** as unused, use **FQCN** in `@param` / `@return` (e.g. `\App\Capability\…\ReadModels\…`) or keep a real type usage so the import stays.
- [ ] `@throws` for PHP’s **`DateMalformedStringException`**: use **`\DateMalformedStringException`** (global), not an unqualified name that resolves under the mapper’s namespace.

## Verification (this repo)

- Follow **TDD** in `.cursor/rules/tdd.mdc` for production changes.
- Run **`bin/phpcbf`**, **`bin/phpstan`**, and **`bin/phpunit`** (container: `docker compose exec -T php` or project Makefile), with `XDEBUG_MODE=off`.
- After OpenAPI-affecting controller changes: `php scripts/merge-openapi.php` as per `openapi.mdc`.

## Suggested implementation order

1. Domain factory + aggregate fixes + domain tests.
2. Write path: query builder factory coverage + tenant-safe child deletes.
3. Read models + queries + read mapper + interface/consumers + presentation mapping.

Splitting this way keeps **behavior changes** testable before **wide read-model** ripple updates.

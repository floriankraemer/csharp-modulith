---
name: tenant-scoped-entity-sequence
description: >-
  Adds per-tenant monotonic integer sequences for new entities using Shared
  TenantScopedSequenceAllocator and a dedicated sequence table. Use when the user
  asks for auto-increment-style codes per tenant, sequence keys, BR-/UC-style
  numeric IDs without prefixes in the database, or wiring a new capability to
  the existing sequence infrastructure in this Symfony backend.
---

# Tenant-scoped entity sequences (Shared allocator)

## What you are building

- A **sequence table** per domain: rows are append-only log of issued `sequence_number` values, scoped by **`tenant_id`** and optionally extra columns (e.g. **`type`** for separate SLO vs SLA counters).
- The **entity table** stores the allocated value as an **`INTEGER`** (not a prefixed string). Display prefixes like `BR-1` belong in API clients or formatters, not in DB sequence logic.
- **All allocation** goes through **`TenantScopedSequenceAllocator`** — do not copy-paste `MAX+1` + `INSERT` in capabilities. The Shared allocator uses a **transaction** and **PostgreSQL `pg_advisory_xact_lock`** so concurrent `allocateNext` calls do not collide.

## Core references (read before cloning)

| Piece | Location |
|-------|----------|
| Allocator | [`TenantScopedSequenceAllocator.php`](src/Shared/Infrastructure/Persistence/Sequence/TenantScopedSequenceAllocator.php) |
| Spec | [`TenantSequenceTableSpec.php`](src/Shared/Infrastructure/Persistence/Sequence/TenantSequenceTableSpec.php) |
| Test pattern | [`TenantScopedSequenceAllocatorTest.php`](tests/Shared/Infrastructure/Persistence/Sequence/TenantScopedSequenceAllocatorTest.php) |
| Simple scope (tenant only) | [`BusinessRuleSequenceRepository.php`](src/Capability/BusinessRules/Infrastructure/Persistence/Doctrine/BusinessRuleSequenceRepository.php) |
| Scoped sequence (`type` column) | [`SloSequenceRepository.php`](src/Capability/ServiceLevelObjectives/Infrastructure/Persistence/Doctrine/SloSequenceRepository.php) |

## Implementation rules

1. **Application boundary** — Define `*SequenceRepositoryInterface` under the capability’s `Application/Repositories/` with `allocateNext(...): int` (extra parameters only for scope keys that exist on the sequence table, e.g. `string $type`).

2. **Infrastructure** — Implement a **thin** `*SequenceRepository` that only constructs a **`TenantSequenceTableSpec`** and calls `$this->allocator->allocateNext(spec, tenantId, scopeValues)`.
   - **`tableName`**: physical sequence table name.
   - **`lockNamespace`**: stable, **globally unique** string (e.g. `sequence:my_entity`). Used to derive advisory lock keys; collisions across domains must be avoided.
   - **`scopeColumnNames`**: ordered list of extra column names on the sequence table; **`scopeValues`** must supply exactly those keys (string values). Empty for tenant-only sequences.

3. **Symfony** — Prefer **constructor injection** of `TenantScopedSequenceAllocator` (autowired). No custom SQL in the capability for allocation.

4. **Entity / persistence** — Store **`int`** on the aggregate and in write/read mappers; add **unique** `(tenant_id, *_code)` (or your column name) in DB. Follow [`aggregate-persistence.mdc`](../../rules/aggregate-persistence.mdc) and [`read-models.mdc`](../../rules/read-models.mdc).

5. **Tenant** — For tenant-aware capabilities, allocation must use the **same tenant id** as persistence; follow [`tenant.mdc`](../../rules/tenant.mdc).

6. **Tests** — Follow [`tdd.mdc`](../../rules/tdd.mdc): integration test for the sequence repository (monotonic per tenant, isolation across tenants, scoped streams independent when using scope columns). Optional: concurrency test two parallel `allocateNext` for same tenant/scope expecting distinct integers.

7. **OpenAPI** — Expose numeric codes as **`integer`** in module YAML; run `php scripts/merge-openapi.php` when done.

## Extended checklist

For a full layer-by-layer checklist (migration, use case, facade, fixtures), see [reference.md](reference.md).

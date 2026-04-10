---
name: http-lookup-autocomplete-endpoints
description: >-
  Add a tenant-scoped GET lookup/autocomplete endpoint for a DomainDocumentation
  entity (id + name), mirroring teams/services. Use when implementing
  `/api/{resource}/autocomplete` or similar lightweight search APIs in this backend.
---

# HTTP lookup / autocomplete endpoints (DomainDocumentation)

Use this pattern when adding a **read-only** list endpoint that returns up to **25** rows with **`{entityId}Id` + `name`**, optional **`query`** (case-insensitive substring), sorted by **`name ASC`**, scoped to the **current tenant**.

## Reference implementations

| Piece | Example (teams) | Example (services) |
|-------|-----------------|---------------------|
| Use case | `Application/UseCases/AutocompleteTeams/` | `Application/UseCases/AutocompleteServices/` |
| Read repo method | `TeamReadRepositoryInterface::autocompleteTeams` | `ServiceReadRepositoryInterface::autocompleteServices` |
| Query builder filter | `TeamQueryBuilder::filterByName` → `LOWER(name) LIKE LOWER(:name)` | Same on `ServiceQueryBuilder` |
| HTTP | `GET /api/teams/autocomplete` | `GET /api/services/autocomplete` |
| Response | `data.teams[]` with `teamId`, `name` | `data.services[]` with `serviceId`, `name` |

Study these files before cloning the pattern:

- [AutocompleteTeamsController.php](src/Capability/DomainDocumentation/Presentation/Http/Controller/Api/Teams/AutocompleteTeamsController.php)
- [TeamReadRepository.php](src/Capability/DomainDocumentation/Infrastructure/Persistence/Doctrine/TeamReadRepository.php) (`autocompleteTeams`)
- [AutocompleteServicesController.php](src/Capability/DomainDocumentation/Presentation/Http/Controller/Api/Services/AutocompleteServicesController.php)
- [ServiceReadRepository.php](src/Capability/DomainDocumentation/Infrastructure/Persistence/Doctrine/ServiceReadRepository.php) (`autocompleteServices`)

## Checklist

1. **Query builder** — For name search use `LOWER(name) LIKE LOWER(:name)` with `'%' . $value . '%'` (PostgreSQL-safe, case-insensitive). Do not use raw `LIKE` without `LOWER` for user-facing name search.

2. **Read repository**
   - Add `autocomplete{Entity}(string $query): array` to the capability’s `*ReadRepositoryInterface` with `@return array<array{…Id: string, name: string}>`.
   - Implementation: `select('id', 'name')`, `from` table, **`filterByTenant()`** for tenant-aware modules (DomainDocumentation), `setMaxResults(25)`, if `!empty($query)` apply a **case-insensitive name filter** (`LOWER(name) LIKE LOWER(:param)` with a unique bound param name). Prefer this `andWhere` on the builder after the standard chain if static analysis loses your concrete `*QueryBuilder` type after `select()`; keep `filterByName()` on the concrete query builder in sync for other callers.
   - **`addOrderBy('name', 'ASC')`**, then map rows to camelCase id key + `name`.

3. **Use case** — Folder `Application/UseCases/Autocomplete{Entity}/`: `{Entity}Input` (string `$query`), `{Entity}Result` (array), `{Entity}` use case calling the read repo, **`array_slice(..., 0, 25)`** as defense in depth. Add a **Specification:** block per project rules.

4. **HTTP**
   - Route: **`GET /api/{plural}/autocomplete`** — register this path **before** `GET /api/{plural}/{id}` so `autocomplete` is not captured as an id.
   - `#[MapQueryString]` request payload: `?string $query = null`.
   - Map to input with `query: $request->query ?? ''`.
   - Return `$apiResponse->OK(['data' => ['{plural}' => $result->{plural}]])` using the **API conventions** plural container name and **camelCase** id field (`teamId`, `serviceId`, …).

5. **OpenAPI** — In the module’s `DomainDocumentationOpenAPI.yaml` (or the owning module file): add `GET /{plural}/autocomplete`, document `query`, response schema under `data.{plural}`, add `Autocomplete{Entity}sResponse` style schema. Run `php scripts/merge-openapi.php --type=http`.

6. **Tests**
   - **Repository integration** (tenant set, persist a few rows): empty query, filtered query, **case-insensitive** query, **max 25**, **alphabetical** order when you control all rows in the tenant.
   - **Controller**: authenticated GET 200 + JSON shape; unauthenticated rejected (same as sibling list endpoints).

7. **Symfony** — Controllers and use cases are **autowired**; no manual `services.yaml` entry unless you hit a custom edge case.

8. **Copyright** — New PHP files get the standard proprietary header from [copyright-notice.mdc](.cursor/rules/copyright-notice.mdc).

## Naming

- Path segment: plural resource name matching existing REST collection (`teams`, `services`, …).
- JSON id property: `{entityCamel}Id` to match existing resources (`teamId`, `serviceId`).

## Do not

- Put business logic in the controller (map payload → input → response only).
- Skip `filterByTenant()` on tenant-aware tables.
- Return more than lightweight fields unless the product explicitly requires it.

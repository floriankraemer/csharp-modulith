# Tenant-scoped sequence — extended checklist

Use with [SKILL.md](SKILL.md). Order follows typical DDD layers; obey **TDD** and project rules in `.cursor/rules/`.

## 1. Database

- **Sequence table** (if it does not exist): `tenant_id` (uuid), `sequence_number` (int), optional scope columns (e.g. `type` varchar). Match existing tables such as `business_rule_sequence`, `use_case_table_sequence`, `slo_sequence`.
- **Entity table**: integer column for the issued sequence (e.g. `my_entity_code`). Unique constraint on `(tenant_id, my_entity_code)` where required.
- **Migration**: valid PostgreSQL; document reset/fixtures if no backfill.

## 2. Shared (usually nothing new)

- Reuse **`TenantScopedSequenceAllocator`** and **`TenantSequenceTableSpec`** only. Do not add a second allocator pattern in a capability.

## 3. Capability — Application

- `Application/Repositories/*SequenceRepositoryInterface` with `allocateNext`: `int` return; parameters `(string $tenantId)` or plus scope args aligned with `scopeColumnNames`.
- Create use case(s) that call the sequence repo, then persist the aggregate with the returned `int`.
- Result DTOs: use `?int` / `int` for codes as appropriate.

## 4. Capability — Domain

- Aggregate holds **`int`** (keep or rename field per module consistency).
- No infrastructure or prefix string logic in domain.

## 5. Capability — Infrastructure

- **`*SequenceRepository`**: inject `TenantScopedSequenceAllocator`, build **`TenantSequenceTableSpec`**, delegate `allocateNext`.
- **Write mapper**: bind integer column.
- **Read model / row mapper**: `int` for code field.
- **Read repo / filters**: numeric sort on int column; for text “contains” filters on codes, cast to text in SQL or filter appropriately (see UseCaseTables pattern).

## 6. Presentation

- Controllers: no business logic; map to use case.
- OpenAPI: `integer` for code fields in module YAML.

## 7. Fixtures & cross-cutting

- **Fixture generators**: insert integer codes, not prefixed strings.
- **Global search / other modules**: grep for old string assumptions after type change.

## 8. Frontend (if applicable)

- API returns **numbers**; format `PREFIX-${code}` in UI if product requires a label.

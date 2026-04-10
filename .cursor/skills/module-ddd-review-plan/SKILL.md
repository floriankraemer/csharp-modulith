---
name: module-ddd-review-plan
description: Reviews a Symfony capability module against DDD and project persistence conventions, then produces a structured implementation plan before coding. Use when refactoring or auditing a module under src/Capability, after a DDD or repository review, or when the user asks to align a capability with modular-monolith rules.
---

# Module DDD review and plan

## Workflow (mandatory order)

1. **Review** — Walk the capability using the checklist below; read only what you need (facade, use cases, domain model, repositories, presentation, `services.yaml`, tests).
2. **Plan** — Output a **written plan** (see template). Do not start large refactors until the plan exists unless the user explicitly skips planning.
3. **Implement** — Execute the plan in small slices; follow backend `.cursor/rules` (especially `modular-monolith.mdc`, `aggregate-persistence.mdc`, `read-models.mdc`, `testing.mdc`, `tdd.mdc`).

If the user only wants a plan, stop after step 2.

## Review checklist

Use this as a gap analysis; note **finding → suggested change** for each relevant item.

| Area | Look for |
|------|----------|
| **Aggregate** | Rich behavior vs anemic model; `final` where appropriate; invariants in domain; factory (`create` / language fit); cross-aggregate refs as IDs only. |
| **Domain events** | `EventProducerTrait` + `recordThat`; no ad-hoc event stacks; handlers on correct bus (`#[AsMessageHandler(bus: 'domainEvent.bus')]` for domain reactions). |
| **Write port** | `Domain/Repositories/*WriteRepositoryInterface`: `persist`, `remove` (not `delete` unless ubiquitous language says otherwise), `generateId()` if IDs are allocated by persistence; **no** read methods on write port. |
| **Read port** | `Application/Repositories/*ReadRepositoryInterface`: queries and list projections only; **no** `purgeExpired` / writes. |
| **Persistence** | DBAL write repo: transactions, table constants, dedicated aggregate mapper (`ObjectAccessor`), `publishFromAggregate` after persist/remove; rehydration clears `domainEvents`. |
| **Read models** | List/search returns readonly read models or serialized shapes with stable **camelCase** API fields—not raw DB rows. Dedicated row mappers (constructor-based), not aggregate mappers. |
| **Query builders** | Filters use real column names / `*Table` constants; no invented columns (e.g. wrong `created_at`). |
| **Config** | TTLs, limits, feature flags in `*Config` + `parameters` in `services.yaml`—not magic numbers in aggregates. |
| **Use cases** | Orchestration only; reads via read repo, writes via write repo; no N+1 cross-module calls for enrichment (batch via bridges in read repos per `read-models.mdc`). |
| **Facades** | Thin; correct repo injected per operation. |
| **Wiring** | `services.yaml` aliases for read vs write interfaces; excluded classes only when justified. |
| **API / OpenAPI** | If response shape changes, update module OpenAPI and merge script if required by the task. |
| **Tests** | PHPUnit `#[Group('CapabilityName')]`; facade/integration tests use correct read vs write ports; repository tests target concrete DBAL classes. |

## Plan template (copy and fill)

```markdown
## Capability: <Name>

### Goals
- ...

### Findings (ordered by severity)
1. ...
2. ...

### Proposed changes
| Step | Area | Change | Risk / notes |
|------|------|--------|--------------|
| 1 | ... | ... | ... |

### Files likely touched
- ...

### Test updates
- ...

### Non-goals (this pass)
- ...

### Verification
- `bin/phpcbf`, `bin/phpstan`, `bin/phpunit` (container; `XDEBUG_MODE=off`)
```

## Implementation reminders

- Run PHP tooling **in the Docker PHP container** (see `.cursor/rules/generic-rules.mdc`).
- Prefer **TDD** for backend: failing test → fix → green → gates.
- **Bridges** for cross-capability calls from application layer; no direct facade-to-facade from domain.

## Example outcome (from PasswordReset-style refactors)

- Split `PasswordResetRepository` → `PasswordResetWriteRepository` + `PasswordResetReadRepository`.
- Listing returns `PasswordResetListItem` JSON shape, not raw rows.
- Token TTL from `PasswordResetConfig` + parameter; `PasswordReset::create(..., $expiresAt)` without embedded duration.
- `ConsumeTokenUseCase` / `ResetPasswordUseCase`: find by token on **read** repo, persist on **write** repo.

Use this as a pattern reference, not a one-size-fits-all prescription—some capabilities are ORM-mapped or differ by tenant rules.

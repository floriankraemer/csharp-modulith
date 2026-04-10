---
name: fixture-data-creation
description: >-
  Creates and updates backend fixture data in src/DataFixtures with strict data integrity, tenant and
  organization consistency, and test-safe behavior. Use when adding or modifying fixtures, seed data,
  demo data, or test baseline data in the backend.
---

# Fixture data creation (backend)

## When to use

- Add new fixtures in `src/DataFixtures`.
- Extend existing fixture sets used by tests or local development.
- Refactor fixture structure while preserving data integrity and stable test expectations.

## Core rules

- Keep existing fixture IDs and references stable unless a change is explicitly required.
- Do not break existing tests: preserve required records, required relations, and expected invariants.
- Use realistic, coherent, real-world data. Never use lorem ipsum or placeholder gibberish.
- Use a fictional e-commerce platform domain as the default context (shops, catalogs, carts, orders, payments, shipments, support, etc.).
- Respect tenant boundaries: tenant-aware entities must be linked to the correct tenant/organization.
- Keep organization membership consistent: users, roles, and member records must match the same organization context.
- Avoid orphaned rows and invalid relations. Every foreign key and cross-reference must resolve to an existing fixture.

## Data design standard

- Build a small but rich dataset with meaningful variation:
  - multiple organizations (for isolation tests),
  - multiple members per organization (admin + non-admin),
  - products with realistic names, SKUs, prices, and stock levels,
  - orders in different lifecycle states (placed, paid, shipped, cancelled),
  - invoices/payments aligned with order totals and statuses.
- Keep naming consistent and plausible:
  - company names, people names, emails, addresses, tax IDs, support channels.
- Ensure chronological consistency:
  - `createdAt <= updatedAt`,
  - order placed before shipment,
  - invoice/payment timestamps after order creation.

## Tenant / organization / member checklist

- Every tenant-aware record has the correct tenant identifier.
- Every organization member belongs to an existing organization.
- Member roles are valid and consistent with the test scenarios (for example admin-only flows).
- Cross-organization leakage is impossible in fixture relations.
- If a relation crosses capabilities, confirm that both sides use the same organization/tenant context.

## Safe implementation workflow

1. Read existing fixtures and identify stable anchors (IDs, emails, org IDs, known constants).
2. Plan additions to be additive first; avoid destructive edits unless strictly needed.
3. Implement or update fixture builders/generators with deterministic values.
4. Validate relation graph integrity (all references resolve, no duplicates where uniqueness is expected).
5. Run the relevant test suite(s); fix fixture regressions before finishing.
6. If changing baseline fixtures, update affected tests intentionally and minimally.

## Verification checklist

- Referential integrity: all relations are valid.
- Domain integrity: statuses, totals, and lifecycle data are coherent.
- Tenant integrity: no mixed-tenant data in tenant-aware modules.
- Organization/member integrity: memberships and roles align with authorization scenarios.
- Test integrity: existing tests pass; newly required tests are added for new fixture behavior.

## Output expectations

- Provide a short change note listing:
  - which fixture files changed,
  - which tenant/org/member assumptions were applied,
  - which realism assumptions were introduced for the fictional e-commerce domain,
  - which tests were executed and their outcomes.

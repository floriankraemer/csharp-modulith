---
name: refactor-http-controller-exceptions
description: Refactors Symfony capability HTTP controllers to remove broad exception catches, rely on ExceptionListener, and update tests. Use when cleaning up Presentation/Http/Controller code, removing catch (Exception) around use cases, aligning a module with DomainDocumentation-style API error handling, or when the user mentions controller exception handling or http-api-controller-exceptions.
---

# Refactor HTTP controller exception handling

## Preconditions

- Read **`.cursor/rules/http-api-controller-exceptions.mdc`** (backend repo root) for the non-negotiables.
- Target code lives under **`src/Capability/<Module>/Presentation/**/Controller/**/*.php`** (Http, FrontendApi, or BackendApi).

## Discovery

1. Search the module for broad catches:

   ```bash
   rg '\\\\Exception \$e\)' src/Capability/<Module>/Presentation --glob '*Controller.php'
   rg '\\\\Throwable \$e\)' src/Capability/<Module>/Presentation --glob '*Controller.php'
   ```

2. Classify each `catch`:
   - **Remove**: wraps `->execute()` or similar and returns `badRequest` / leaks `getMessage()`.
   - **Keep**: `HttpJsonRequestValidationException`, mapper `\InvalidArgumentException | InvariantViolationException` (presentation mapping only).
   - **Narrow**: import/parse boundaries — prefer `catch (\Exception)` over `catch (\Throwable)` unless a specific `Error` must become a client-facing validation response (rare).

3. Check for **duplicate route** classes (same `path` + `methods` in two namespaces). Prefer one canonical controller; delete or merge duplicates.

## Refactor

1. Delete outer `try { … } catch (\Exception)` around use-case execution; keep success / `notFound` branches as today.
2. Do **not** add new controller catches to “fix” tests — fix expectations to match **`ExceptionListener`** (`src/Shared/Presentation/Http/ExceptionListener.php`):
   - `test` and `prod`: listener maps exceptions (not `dev`/`debug`).
   - Messages containing **`not found`** (case-insensitive) on `RuntimeException` / `InvalidArgumentException` (and related handler paths) often yield **404**.
   - Generic `\Exception` → **500** in non-dev environments.
3. If a legitimate client error still maps to the wrong status, fix **domain/application** (exception type or message) or extend **`ExceptionListener`** in a focused way — avoid reintroducing controller-wide `catch (\Exception)`.

## Tests and quality gates

1. Add or adjust controller / API tests so assertions match listener behavior (status + problem+json shape where the suite already uses it).
2. In the PHP container (project Makefile / `docker compose exec -T php`):

   ```bash
   export XDEBUG_MODE=off && bin/phpcbf && bin/phpstan && bin/phpunit
   ```

   Scope `phpunit` to the module first, then run the **full** suite before finishing.

3. If **`ExceptionListener`** behavior changes, update **`tests/Shared/Presentation/Http/ExceptionListenerTest.php`** and any docs that describe `test` vs `dev` listener bypass (e.g. permissions guide).

## Out of scope (unless explicitly requested)

- Rewriting all controllers to `MapRequestPayload` vs `JsonRequestBodyDecoder` in one pass.
- Changing OpenAPI files unless response codes or shapes actually change for clients.

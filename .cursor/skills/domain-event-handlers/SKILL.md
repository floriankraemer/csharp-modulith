---
name: domain-event-handlers
description: >-
  Adds Symfony Messenger domain event handlers in backend capabilities: placement on domainEvent.bus,
  bridges for cross-module data, optional Job+Worker enqueueing on job.bus, tests, and alignment with
  docs/Domain-Event-Handlers.md. Use when implementing reactions to domain events (not HTTP), including
  notifications, projections, or enqueueing background work.
---

# Domain event handlers (backend)

## When to use

- React to a **domain event** emitted from an aggregate (`recordThat`, published after persist).
- Keep the reaction **out of** the aggregate and **out of** HTTP controllers.

## Do not

- Put domain/business rules in the handler (belongs on the aggregate or use case).
- Inject another capability’s **repositories** (use a **bridge** to that module’s facade).
- Catch exceptions unless project rules explicitly allow it for that case.
- Register handlers on the default bus: always set `bus: 'domainEvent.bus'`.

## Steps

1. **Event** — Confirm the event class exists under the aggregate’s `Events/` namespace and is recorded where the behavior occurs.
2. **Sync vs async** — If the work is quick and safe (e.g. write an audit row), handle it directly in the handler. If it is slow or flaky (email, HTTP), enqueue a **Job** and implement a **Worker** on `job.bus` (see [Jobs-And-Workers.md](../../docs/Jobs-And-Workers.md)).
3. **Handler class** — `src/Capability/<Capability>/Application/DomainEventHandlers/<Name>Handler.php`:
   - `final readonly`
   - `#[AsMessageHandler(bus: 'domainEvent.bus')]`
   - `public function __invoke(ConcreteEvent $event): void`
4. **Dependencies** — Use capability repositories, `JobQueueInterface`, and `*BridgeInterface` only.
5. **Job + worker (if async)** — Under `Application/Jobs/<JobName>/`:
   - `<JobName>Job.php` implements `App\Shared\Application\Jobs\Job`
   - `<JobName>Worker.php` with `#[AsMessageHandler(bus: 'job.bus')]`
   - Route the job class to transport `local` in `config/packages/messenger.yaml`
6. **Tests** — `#[Group('<Capability>')]`, mock external ports, assert `enqueue` or repository calls.
7. **Headers** — New PHP files: proprietary license header per `.cursor/rules/copyright-notice.mdc`.

## Reference examples

- `src/Capability/ArchitectureDecisionRecords/Application/DomainEventHandlers/AdrOpenedForDiscussionHandler.php`
- `src/Capability/Subscriptions/Application/DomainEventHandlers/SubscriptionWasCreatedHandler.php` (pattern; verify `bus` attribute matches `domainEvent.bus` where used)

## Further reading

- [docs/Domain-Event-Handlers.md](../../docs/Domain-Event-Handlers.md)
- [docs/Jobs-And-Workers.md](../../docs/Jobs-And-Workers.md)

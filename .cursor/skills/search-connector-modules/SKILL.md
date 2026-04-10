---
name: search-connector-modules
description: >-
  Creates search connector modules (Anti-Corruption Layers) that bridge a producer
  capability to the GlobalSearch capability. Listens to domain events, fetches entity
  data via bridge, and indexes into Elasticsearch. Bulk reindex via integration.bus
  + job.bus. Use when adding a new entity type to global search, or when the user asks
  to make a capability searchable.
---

# Search connector modules (backend)

## When to use

- A new capability or entity type needs to appear in the global site-wide search.
- An existing capability emits domain events (`*Created`, `*Updated`, `*Deleted`) and you need to connect it to the `GlobalSearch` module.

## Do not

- Put search logic inside the producer capability (DomainDocumentation, Pages, etc.) — the connector is a separate module.
- Import producer repositories directly in the connector's Application layer — always use a bridge interface.
- Register domain handlers on the default bus — always use `#[AsMessageHandler(bus: 'domainEvent.bus')]`.
- Add business logic to the handler — it only orchestrates (fetch data, map, index/remove).
- Catch exceptions in handlers unless explicitly required.
- Add Shared-kernel interfaces for bulk reindex — use a module-local `*SearchReindexProvider` class only.

## Prerequisites

Before starting, verify:
1. The producer capability emits domain events from its aggregates via `EventProducerTrait`.
2. The producer has a read repository or facade that can return entity data by ID and list all entities.
3. `GlobalSearch` capability exists with `GlobalSearchFacadeInterface`.
4. You will register the entity type string in `config/packages/global_search.yaml` (`global_search.reindex_entity_types`).

## Steps

### 1. Create module skeleton

Create `src/Capability/<Producer>Search/` with:

```
src/Capability/<Producer>Search/
├── <Producer>SearchException.php          (extends BaseException)
├── Application/
│   ├── <Producer>SearchApplicationException.php
│   ├── Bridges/
│   │   ├── <Producer>BridgeInterface.php
│   │   └── GlobalSearchBridgeInterface.php
│   ├── DomainEventHandlers/
│   │   └── When<Entity>ChangedUpdateSearchIndex.php
│   ├── IntegrationEventHandlers/
│   │   └── WhenReindexRequestedEnqueueReindexJob.php
│   ├── Jobs/
│   │   └── Reindex<Entity>/
│   │       ├── Reindex<Entity>Job.php
│   │       └── Reindex<Entity>Worker.php
│   └── Services/
│       └── <Producer>SearchReindexProvider.php
├── Infrastructure/
│   ├── <Producer>SearchInfrastructureException.php
│   └── Bridges/
│       ├── <Producer>Bridge.php
│       └── GlobalSearchBridge.php
```

All PHP files need the proprietary license header from `.cursor/rules/copyright-notice.mdc`.

### 2. Define bridge interfaces (Application layer)

**Producer bridge** (`Application/Bridges/<Producer>BridgeInterface.php`):
- `get<Entity>(string $entityId): ?array` — returns entity data or null
- `listAll<Entities>(): iterable` — returns all entities for reindexing
- Return plain arrays with keys: `id`, `title`, `content/description`, `summary`, `updatedAt`

**Search index bridge** (`Application/Bridges/GlobalSearchBridgeInterface.php`):
- `indexDocument(...)` — required args: `tenantId`, `entityType`, `entityId`, `title`, `content`, `description`, `route`, `updatedAt`. Optional (with defaults): `status`, `createdAt`, `people` (`list<array{id,name}>`), `teams`, `badges` (`list<array{id,label}>`), `tags` (`list<string>`), `metadata` (`array<string,mixed>|null`).
- `bulkIndexDocuments(array $documents): void` — each document array includes the same keys as above; optional keys may be omitted.
- `bulkRemoveDocuments(array $documents): void` — each item is `tenantId`, `entityType`, `entityId`.
- `removeDocument(string $tenantId, string $entityType, string $entityId): void`

Copy `GlobalSearchBridgeInterface` from a reference connector (`ArchitectureDecisionRecordsSearch` or `PagesSearch`) — the method signatures are shared across connectors.

**Elasticsearch document shape** (via `IndexDocumentRequest`): core fields plus optional `status`, `created_at`, nested `people` / `teams` / `badges`, `tags`, and opaque `metadata` (stored, not indexed). See `docs/Search/Global-Search-Architecture.md`.

**Elasticsearch `_id`:** GlobalSearch derives it via `GlobalSearchDocumentId::encode` (deterministic SHA-256 hex over a versioned payload); connectors only pass `tenantId` / `entityType` / `entityId` to the facade — do not build composite string IDs yourself.

### 3. Implement bridges (Infrastructure layer)

**Producer bridge** (`Infrastructure/Bridges/<Producer>Bridge.php`):
- Inject the producer's facade interface only.
- Never inject the producer's repositories in a search bridge implementation.
- Expose dedicated facade methods like `get<Entity>()` and `list<Entities>()` for search access.
- Map the producer's read model to the plain array format.
- For `listAll*()`, paginate through the producer's list method yielding arrays.

**Search index bridge** (`Infrastructure/Bridges/GlobalSearchBridge.php`):
- Inject `GlobalSearchFacadeInterface`.
- Map method arguments to `IndexDocumentRequest` / `RemoveDocumentRequest` and call facade.
- Copy from reference implementation — this class is nearly identical across connectors.

### 4. Create domain event handler

`Application/DomainEventHandlers/When<Entity>ChangedUpdateSearchIndex.php`:

```php
#[AsMessageHandler(bus: 'domainEvent.bus')]
final readonly class When<Entity>ChangedUpdateSearchIndex
{
    private const string ENTITY_TYPE = '<entity_type>';

    public function __construct(
        private <Producer>BridgeInterface $producerBridge,
        private GlobalSearchBridgeInterface $globalSearchBridge,
        private TenantContextInterface $tenantContext,
    ) {
    }

    public function __invoke(
        <Entity>Created|<Entity>Updated|<Entity>Deleted $event,
    ): void {
        $tenantId = $this->tenantContext->getTenantId();
        if ($tenantId === null) {
            return;
        }

        if ($event instanceof <Entity>Deleted) {
            $this->globalSearchBridge->removeDocument(
                tenantId: $tenantId,
                entityType: self::ENTITY_TYPE,
                entityId: $event-><entityId>,
            );
            return;
        }

        $entity = $this->producerBridge->get<Entity>($event-><entityId>);
        if ($entity === null) {
            return;
        }

        $this->globalSearchBridge->indexDocument(
            tenantId: $tenantId,
            entityType: self::ENTITY_TYPE,
            entityId: $entity['<entityId>'],
            title: $entity['title'],
            content: $entity['description'] ?? '',
            description: $entity['summary'] ?? null,
            route: '/<route-prefix>/' . $entity['<entityId>'],
            updatedAt: $entity['updatedAt'] ?? (new DateTimeImmutable())->format('c'),
            // Optional: status, createdAt, people, teams, badges, tags, metadata
        );
    }
}
```

Key rules:
- Union-type all relevant events from the producer (Created, Updated, Deleted, and status change events like Accepted/Rejected if they affect searchable content).
- Always guard on `$tenantId === null`.
- Use `ENTITY_TYPE` constant matching the producer (e.g. `'adr'`, `'team'`, `'page'`).
- Route pattern should match the frontend route for deep-linking.

### 5. Bulk reindex: config + integration handler + job + reindex service

1. **Config:** Add `<entity_type>` to `global_search.reindex_entity_types` in `config/packages/global_search.yaml` (same string as `ENTITY_TYPE` in the domain handler).

2. **Reindex service** (`Application/Services/<Producer>SearchReindexProvider.php`):
   - Module-local `final readonly` class — **do not** implement a Shared interface.
   - `private const ENTITY_TYPE = '<entity_type>'` (internal use in bulk payloads).
   - `reindex(string $tenantId): void` — iterate `listAll*()`, batch (~500), `bulkIndexDocuments()`.

3. **Integration handler** (`Application/IntegrationEventHandlers/WhenReindexRequestedEnqueueReindexJob.php`):
   - `#[AsMessageHandler(bus: 'integration.bus')]`
   - `__invoke(SearchReindexRequested $event): void` — return unless `$event->entityType === '<entity_type>'`; else `JobQueueInterface::enqueue(new Reindex<Entity>Job(...))`.

4. **Job + worker:**
   - `Reindex<Entity>Job implements Job` with `tenantId`.
   - `Reindex<Entity>Worker`: `#[AsMessageHandler(bus: 'job.bus')]`, `setTenantId`, call reindex provider, `clear()` in `finally`.
   - Route job in `config/packages/messenger.yaml`: `Reindex<Entity>Job: local`.

### 6. Wire services in `config/services.yaml`

Add bridge aliases:

```yaml
App\Capability\<Producer>Search\Application\Bridges\<Producer>BridgeInterface: '@App\Capability\<Producer>Search\Infrastructure\Bridges\<Producer>Bridge'
App\Capability\<Producer>Search\Application\Bridges\GlobalSearchBridgeInterface: '@App\Capability\<Producer>Search\Infrastructure\Bridges\GlobalSearchBridge'
```

Handlers are auto-discovered via `#[AsMessageHandler]`.

### 7. Write tests

`tests/Capability/<Producer>Search/Application/DomainEventHandlers/When<Entity>ChangedUpdateSearchIndexTest.php`:

- `#[Group('<Producer>Search')]`
- Mock all three constructor dependencies
- Test cases:
  - Created event → `indexDocument` called with correct arguments
  - Updated event → `indexDocument` called
  - Deleted event → `removeDocument` called, `getAdr` never called
  - No tenant context → nothing called
  - Entity not found → `indexDocument` not called

Add tests for integration handler (enqueue / no-op) and worker (tenant set + reindex) mirroring `ArchitectureDecisionRecordsSearch` tests.

## Reference implementation

- Handler: `src/Capability/ArchitectureDecisionRecordsSearch/Application/DomainEventHandlers/WhenAdrChangedUpdateSearchIndex.php`
- Bridges: `src/Capability/ArchitectureDecisionRecordsSearch/Application/Bridges/`
- Bridge implementations: `src/Capability/ArchitectureDecisionRecordsSearch/Infrastructure/Bridges/`
- Bulk reindex: `Application/Services/ArchitectureDecisionRecordsSearchReindexProvider.php`, `IntegrationEventHandlers/WhenReindexRequestedEnqueueReindexJob.php`, `Jobs/ReindexAdr/`
- Config: `config/packages/global_search.yaml` (`adr` in `global_search.reindex_entity_types`)
- Tests: `tests/Capability/ArchitectureDecisionRecordsSearch/...`

## Further reading

- [docs/Search/Global-Search-Architecture.md](../../docs/Search/Global-Search-Architecture.md) — overall architecture
- [docs/Search/Global-Search-Connector-Guide.md](../../docs/Search/Global-Search-Connector-Guide.md) — detailed developer guide
- [docs/Domain-Event-Handlers.md](../../docs/Domain-Event-Handlers.md) — domain event handler conventions

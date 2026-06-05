---
name: cnma-cap-backend
description: Create the backend structure (DB + SRV) for a SAP CAP project following Conarum standards.
---

# CNMA CAP Backend Skill

## Purpose
Scaffold the **DB** and **SRV** layers of a new SAP CAP project following Conarum's established architecture. This skill ensures every new backend module starts with the correct directory structure, core classes, and conventions.

## When to Use
- Creating a new CAP project from scratch
- Adding a backend module to an existing workspace
- Referenced by the `/init_cap_project` workflow

---

## Namespace Convention (CRITICAL)

Every project **must** have a namespace prefix. Convention: `cnma.{module_name}` (dot-separated lowercase).

| Where | Format | Example |
|-------|--------|---------|
| CDS schema files | `namespace {{namespace}};` | `namespace cnma.notification;` |
| CDS service definition | `using {{namespace}} as ns from '../db/...'` | `using cnma.notification as ns from '../db/schema/Notification';` |
| `package.json` name | `"name": "{{project_name}}"` | `"name": "cnma_notification_service"` |
| Service handler | OData path uses namespace entities | `srv.on('READ', ns.MyEntity, ...)` |

> **The agent MUST ask for the namespace** when scaffolding. It is passed from the `/init_cap_project` workflow.

---

## Architecture Overview

```
{{project_name}}/
├── db/
│   ├── schema/                 # CDS entity definitions
│   │   └── MyEntity.cds
│   ├── src/                    # HANA artifacts (calculation views, etc.)
│   ├── view/                   # HANA SQL views
│   ├── build.js                # Custom DB build script
│   └── package.json            # HDI deployer config
├── srv/
│   ├── server.ts               # ★ Custom CAP Bootstrap (Express middleware, auth, cron)
│   ├── {{ServiceName}}.cds     # OData service definition
│   ├── {{ServiceName}}.ts      # OData service handler (event registration)
│   ├── src/
│   │   ├── core/               # ★ Base classes (CommonService, DBHandler, DestinationCloudService)
│   │   ├── model/
│   │   │   └── core/           # ★ Response classes (ApiResponse, ServiceResponse, ValidationResponse)
│   │   ├── interfaces/         # ★ TypeScript interfaces (ICommon, IApiResponse, etc.)
│   │   ├── enum/               # ★ Enumerations (HttpStatusCodeEnum, domain enums)
│   │   ├── config/             # Configuration files
│   │   ├── services/           # Business logic services
│   │   ├── middleware/         # Express middleware
│   │   ├── utils/              # Utility functions
│   │   ├── api/                # REST API routes (non-OData)
│   │   ├── events/             # Event handlers
│   │   ├── workers/            # Background workers
│   │   └── cds/                # CDS-specific handlers (entities, functions, actions)
│   └── type/                   # TypeScript type definitions
├── i18n/                       # Localization bundles
├── mta.yaml                    # MTA deployment descriptor
├── package.json                # Root dependencies & scripts
├── tsconfig.json               # TypeScript configuration
├── xs-security.json            # XSUAA security descriptor
└── .eslintrc                   # ESLint configuration
```

### 7. Core Classes (Must Include)

### 1. `server.ts` — Custom CAP Bootstrap Hook
> **Template**: `templates/srv/server.ts`

Hooks into `cds.on('bootstrap')` to add Express middleware, authentication (XSUAA/Passport), custom REST routes, and cron jobs. This is the **entry point** referenced by `package.json > "main"`.

### 2. `CommonService.ts` — Base Service Class
> **Template**: `templates/srv/core/CommonService.ts`

All services extend this. Provides:
- `getText()` — i18n text retrieval
- `buildServiceResponse()` / `buildApiResponse()` — standardized response formatting
- `setCreateManaged()` / `setUpdateManaged()` — audit field management

### 3. `DBHandler.ts` — Generic CRUD Handler
> **Template**: `templates/srv/core/DBHandler.ts`

Extends `CommonServiceImpl`. Provides generic CRUD with proper **HANA transaction management**:
- Always uses `cds.tx(cds.context)` (not `cds.tx(req)`)
- Always `tx.commit()` after success to release connection pool
- Always `tx.rollback()` in catch blocks
- Returns `ServiceResponse` with typed HTTP status codes

### 4. `DestinationCloudService.ts` — BTP Destination & Token Management
> **Template**: `templates/srv/core/DestinationCloudService.ts`

Manages BTP destination lookups and OAuth2 `client_credentials` token fetching using `@sap-cloud-sdk/connectivity`.

### 5. Response Classes (model/core/)

| Class | Template | Purpose |
|-------|----------|---------|
| `ApiResponse<T>` | `templates/srv/model/core/ApiResponse.ts` | Standard API envelope (`success`, `message`, `data`) |
| `ServiceResponse<T>` | `templates/srv/model/core/ServiceResponse.ts` | Adds `statusCode` + `toApiResponse()` conversion |
| `ValidationResponse<T>` | `templates/srv/model/core/ValidationResponse.ts` | For input validation (`valid`, `message`) |

### 6. `ICommon.ts` — Interface Contracts
> **Template**: `templates/srv/interfaces/ICommon.ts`

Defines: `IApiResponse<T>`, `IServiceResponse<T>`, `IValidationResponseData<T>`, `ICommonService`, `IDestinationCloudService`, `Managed`, `IBulkUpdateData`.

### 7. `HttpStatusCodeEnum.ts` — HTTP Status Codes
> **Template**: `templates/srv/enum/HttpStatusCodeEnum.ts`

Exports `HTTP_STATUS`, `HTTP_STATUS_2XX`, and `HTTP_SUCCES_STATUSES` arrays for consistent status code usage.

### 8. Logging & Middleware
> **Templates**:
> - `templates/srv/utils/logger.ts`
> - `templates/srv/middlewares/btp-service-logging.middleware.ts`

- **logger.ts**: Winston-based structured logger with consistent format.
- **BTPServiceLoggingMiddleware**: Middleware/Interceptors for logging BTP service calls (start, success, error) and correlation IDs.

---

## Variant Management (Optional)

If the user selects "Yes" for Variant Management during scaffolding, the skill will generate:

### 1. `VariantSettings.cds` (db/schema/)
> **Template**: `templates/db/schema/VariantSettings.cds`

Defines the `VariantSettings` entity extending `cuid` and `managed`.

### 2. `VariantHandler.ts` (srv/cds/handlers/)
> **Template**: `templates/srv/cds/handlers/VariantHandler.ts`

Registers `READ` and `before CREATE` event handlers for `VariantSettings`.

### 3. Variant Event Handlers (srv/cds/events/variant/)
- `OnBeforeCreateVariantSettingsEvent.ts`: Handles conflict checks and default flag logic.
- `OnReadVariantSettingsEvent.ts`: Filters variants by `createdBy`.
- `OnAdjustDefaultVariantSettingAction.ts`: Handles setting/clearing default variants.

### 4. `ActionResponse.ts` (srv/core/)
> **Template**: `templates/srv/core/ActionResponse.ts`

Helper class for standardized action responses used in `AdjustDefaultVariantSetting`.

---

## CDS Event Naming Convention (MANDATORY)

All files under `srv/cds/events/` **MUST** follow these naming patterns:

### Entity Lifecycle Events (suffix `Event`)
```
On{Before|After}{CDS_Hook}{EntityName}Event.ts
```

| CDS Hook | File Name Pattern | Example |
|----------|------------------|---------|
| `srv.on('READ', Entity)` | `OnRead{Entity}Event.ts` | `OnReadVariantSettingsEvent.ts` |
| `srv.before('CREATE', Entity)` | `OnBeforeCreate{Entity}Event.ts` | `OnBeforeCreateVariantSettingsEvent.ts` |
| `srv.after('CREATE', Entity)` | `OnAfterCreate{Entity}Event.ts` | `OnAfterCreateLogRecordEvent.ts` |
| `srv.after('UPDATE', Entity)` | `OnAfterUpdate{Entity}Event.ts` | `OnAfterUpdateConsumerHandlerSettingsEvent.ts` |

### CDS Actions (suffix `Action`)
All CDS Action files **MUST** be placed in `srv/cds/actions/`:
```
On{ActionName}Action.ts
```

| CDS Hook | File Name Pattern | Example |
|----------|------------------|---------|
| `srv.on('actionName')` | `On{ActionName}Action.ts` | `OnReprocessByFilterAction.ts` |
| Special (multi-method) | `{Domain}Action.ts` | `ConsumerHealthAction.ts` |

**Rules:**
- Every file **must** export an `execute(data, req)` function
- Handler files in `srv/cds/handlers/` only register CDS hooks and delegate to event/action classes
- Business logic for Entity hooks lives in `srv/cds/events/` 
- Business logic for Actions lives in `srv/cds/actions/`

---

## Service Handler Convention

> **Template**: `templates/srv/Handler.ts`

Each `.cds` service gets a corresponding `.ts` handler file:

```typescript
module.exports = async (srv) => {
    const { MyEntity } = srv.entities;

    // ENTITY HANDLERS
    srv.on('READ', MyEntity, async (req) => { /* ... */ });
    srv.after('READ', MyEntity, async (data, req) => { /* ... */ });

    // FUNCTION/ACTION HANDLERS
    srv.on('myFunction', async (req) => { /* ... */ });
};
```

**Rules**:
- Keep handler files thin — delegate to `services/` classes
- Always use `try/catch` with `req.error()` or `ApiResponse`
- Group by: Entity Handlers → Function Handlers → Test Handlers

---

## Scaffolding Scripts

| Script | Purpose |
|--------|---------|
| `scripts/scaffold_backend.py` | Creates the full backend directory structure and auto-copies base templates. Use `--variant` flag to include Variant Management. |
| `scripts/validate.py` | Validates all required files and folders exist |

## Execution Steps

1. **Create root structure**: `package.json`, `mta.yaml`, `tsconfig.json`, `xs-security.json`, `.eslintrc`.
2. **Create `db/`**: Schema folder, `build.js`, `package.json`.
3. **Run scaffold script**: Execute `scaffold_backend.py <project_path> --namespace <ns> [--variant]` to create all directories and **automatically copy all base templates (`core/`, `model/`, `utils/`, `server.ts`, etc.)**.
5. **Create `server.ts`**: Use `templates/srv/server.ts`.
6. **Create `mta.yaml`**: Define `db` (hdb) and `srv` (nodejs) modules with proper resource bindings.
7. **Validation**: Run `validate.py` to verify all files exist and directory structure is correct.

---

## 🚨 CRITICAL: HDI Container Protection

### MUST CLEAN `db/undeploy.json` After Project Initialization

After initializing the project, you will see the content in `db/undeploy.json`:
```json
[
 "src/gen/**/*.hdbview",
 "src/gen/**/*.hdbindex",
 "src/gen/**/*.hdbconstraint",
 "src/gen/**/*_drafts.hdbtable",
 "src/gen/**/*.hdbcalculationview"
]
```

**YOU MUST MAKE IT EMPTY BEFORE DEPLOY**:
```json
[]
```

**IF NOT, YOU WILL CLEANUP ALL DATA IN CONTAINER → NEED TO REDEPLOY ALL APPLICATIONS** because we're using the same HDI container.

---

## 🤖 Agent Verification Protocol (MANDATORY)

**Protocol**: After performing any `scaffold`, `refactor`, or `move` operations, the Agent **MUST** automatically run the verification scripts below to validate the integrity of the codebase.

1.  **Run Check Imports**:
    ```bash
    node .agent/skills/cnma-cap-backend/scripts/check_imports.js
    ```
2.  **Run Verify Imports**:
    ```bash
    node .agent/skills/cnma-cap-backend/scripts/verify-imports.js
    ```

**Validation Rule**:
- If any script returns an error (Exit Code 1), the Agent **MUST STOP**, analyze the error, and fix the broken imports before reporting success to the User.

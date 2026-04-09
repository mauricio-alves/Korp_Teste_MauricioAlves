# CODEBASE.md — Dependency Map

This file is the single source of truth for architectural dependencies between layers and services.
Update this file whenever adding new layers or changing service relationships.

## Service Communication

```mermaid
graph LR
    subgraph ApiGateway [":5000 - API Gateway / BFF"]
        GW_Ctrl["Controllers"] --> GW_Prov["Providers"]
    end

    subgraph InventoryService [":5001 - Inventory Service"]
        INV_Ctrl["Controllers"] --> INV_Svc["Services"]
        INV_Svc --> INV_Repo["Repositories"]
        INV_Repo --> INV_Db["InventoryDbContext (inventory_db)"]
    end

    subgraph BillingService [":5002 - Billing Service"]
        BIL_Ctrl["Controllers"] --> BIL_Svc["Services"]
        BIL_Svc --> BIL_Repo["Repositories"]
        BIL_Svc --> BIL_Prov["Providers"]
        BIL_Repo --> BIL_Db["BillingDbContext (billing_db)"]
    end

    Angular["Angular :4200"] -->|HTTP| GW_Ctrl
    GW_Prov -->|HTTP + Polly| INV_Ctrl
    GW_Prov -->|HTTP + Polly| BIL_Ctrl
    BIL_Prov -->|HTTP + Polly| INV_Ctrl
    GW_Prov -->|HTTP| Gemini["Google Gemini API"]
```

## Layer Rules

| Layer        | Allowed Dependencies     | Forbidden                 |
| ------------ | ------------------------ | ------------------------- |
| Controllers  | Services only            | Repositories, Providers   |
| Services     | Repositories + Providers | DbContext directly        |
| Repositories | DbContext only           | Services, Providers, HTTP |
| Providers    | HttpClient only          | DbContext, Repositories   |

## Port Map

| Service           | Port | DB                  |
| ----------------- | ---- | ------------------- |
| API Gateway       | 5000 | None (proxy)        |
| Inventory Service | 5001 | inventory_db (5432) |
| Billing Service   | 5002 | billing_db (5432)   |
| Angular Frontend  | 4200 | None (API only)     |
| PostgreSQL        | 5432 | -                   |

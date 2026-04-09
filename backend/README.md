# Backend — Detalhamento Tecnico

Tres microsservicos C# .NET 8 com Entity Framework Core e PostgreSQL.

---

## Servicos

| Servico           | Porta | Banco        | Swagger                       |
| ----------------- | ----- | ------------ | ----------------------------- |
| API Gateway / BFF | 5000  | —            | http://localhost:5000/swagger |
| Inventory Service | 5001  | inventory_db | http://localhost:5001/swagger |
| Billing Service   | 5002  | billing_db   | http://localhost:5002/swagger |

---

## Arquitetura de Camadas (SRP)

```
Controller  ->  Service  ->  Repository  ->  DbContext
                         ->  Provider   ->  HttpClient (chamadas externas)
```

**Regras:**

- Controllers: apenas orquestram request/response
- Services: toda a logica de negocio
- Repositories: exclusivos para queries no banco (EF Core)
- Providers: exclusivos para chamadas HTTP externas (Inventory API, Gemini API)

---

## Gerenciamento de Dependencias (NuGet)

| Package                               | Versao | Uso                       |
| ------------------------------------- | ------ | ------------------------- |
| Microsoft.EntityFrameworkCore         | 8.x    | ORM                       |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.x    | Driver PostgreSQL         |
| Polly                                 | 8.x    | Retry + Circuit Breaker   |
| Microsoft.Extensions.Http.Polly       | 8.x    | Polly + HttpClientFactory |
| Swashbuckle.AspNetCore                | 6.x    | Swagger / OpenAPI         |
| coverlet.collector                    | 6.x    | Cobertura de testes       |
| Moq                                   | 4.x    | Mocking em testes         |
| xunit                                 | 2.x    | Framework de testes       |

---

## LINQ — Uso no Projeto

Exemplos reais do projeto extraidos dos Repositories:

```csharp
// Listagem com ordenacao
await _context.Products
    .AsNoTracking()
    .OrderBy(p => p.Code)
    .Select(p => new ProductDto(p))
    .ToListAsync();

// Busca por ID com tratamento de nulo
var product = await _context.Products
    .FirstOrDefaultAsync(p => p.Id == id);

if (product is null) throw new NotFoundException($"Product {id} not found");

// Filtro com Where
var lowStock = await _context.Products
    .Where(p => p.Balance < minimumBalance)
    .AsNoTracking()
    .ToListAsync();

// Eager loading de relacionamentos (Invoice com Items)
var invoice = await _context.Invoices
    .Include(i => i.Items)
    .FirstOrDefaultAsync(i => i.Id == id);
```

---

## Tratamento de Erros

**GlobalExceptionMiddleware** — captura todas as excecoes nao tratadas e retorna `ProblemDetails` (RFC 7807):

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Not Found",
  "status": 404,
  "detail": "Product abc-123 not found"
}
```

| Excecao de Dominio          | Status HTTP | Cenario                                           |
| --------------------------- | ----------- | ------------------------------------------------- |
| NotFoundException           | 404         | Produto/Nota nao encontrado                       |
| ConflictException           | 409         | Codigo de produto duplicado ou saldo insuficiente |
| BusinessRuleException       | 422         | Nota ja impressa, quantidade invalida             |
| ServiceUnavailableException | 503         | Microsservico nao responde (Polly esgotado)       |

---

## Resiliencia com Polly

Configurado no `ApiGateway` e `BillingService` para chamadas HTTP entre servicos:

```csharp
// Retry: 3 tentativas com exponential backoff (1s, 2s, 4s)
services.AddHttpClient("InventoryClient")
    .AddPolicyHandler(PollyPolicies.RetryAsync())
    .AddPolicyHandler(PollyPolicies.CircuitBreakerAsync());

// CircuitBreaker: abre apos 5 falhas, aguarda 30s
```

**Fluxo de Impressao com Rollback (Saga/Compensacao):**

1. Para cada item da nota: debita saldo no InventoryService
2. Se qualquer debito falhar: credito reverso em todos os itens ja debitados
3. Retorna 503 com detalhes do erro para o frontend

---

## Como Rodar Isoladamente

```bash
# Pre-requisito: PostgreSQL rodando (docker-compose up -d na raiz)
# Copiar .env.example para .env na raiz e configurar

# Inventory Service
cd InventoryService
dotnet run

# Billing Service (em outro terminal)
cd BillingService
dotnet run

# API Gateway (em outro terminal)
cd ApiGateway
dotnet run
```

## Rodar Testes

```bash
cd backend

# Todos os testes com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Apenas um projeto
cd InventoryService.Tests
dotnet test
```

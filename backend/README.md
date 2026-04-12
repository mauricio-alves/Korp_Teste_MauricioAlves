# Korp Note System | Backend & Engenharia

O backend do **Korp Note System** foi projetado para oferecer alta confiabilidade transacional e segurança robusta, utilizando as melhores práticas da plataforma **.NET 8** e persistência em **PostgreSQL**.

---

## Estrutura de Microsserviços

A arquitetura é composta por três pilares principais que se comunicam de forma síncrona e eficiente:

1. **API Gateway**: Porta de entrada que centraliza a segurança (CSP), roteamento e abstrai a complexidade dos serviços internos.
2. **Billing Service**: Gerencia o ciclo de vida das Notas Fiscais, garantindo a atomicidade entre a geração do registro e a movimentação de estoque.
3. **Inventory Service**: Responsável pelo catálogo de produtos e controle de saldo com suporte a concorrência otimista.

---

## Diferenciais Técnicos

### 1. Atomic Billing Flow (Atomicidade na Impressão)

Implementamos uma lógica de **Saga Patterns Simplificada** no `InvoiceService` focada no evento de **Faturamento/Impressão**:

- **Gatilho**: Ao "Imprimir" uma nota (`PrintAsync`), o sistema dispara a transação.
- **Débito de Estoque**: O sistema executa o débito no `InventoryService`.
- **Validação de Saldo**: Se o débito falha (saldo insuficiente), a nota permanece aberta para correção.
- **Fechamento**: Somente após o sucesso do débito a nota é marcada como `Closed`.
- **Rollback de Compensação**: Se ocorrer um erro após o débito parcial ou falha de rede no fechamento, um mecanismo de compensação devolve automaticamente o saldo ao estoque.

### 2. BaseProvider Pattern (Clean Architecture)

Utilizamos o `BaseProvider.cs` no Gateway para centralizar a infraestrutura de comunicação, seguindo um design que evita conflitos de nomes (shadowing):

- **Métodos Internos**: A infraestrutura base utiliza nomes como `InternalGetAsync` e `InternalPostAsync`.
- **Implementação Limpa**: Os provedores específicos (`Billing`, `Inventory`) implementam suas interfaces públicas chamando estes métodos internos, eliminando a ambiguidade de despacho.
- **Gerenciamento de HttpClient**: Ciclo de vida centralizado com disposal correto e tratamento estruturado de exceções inter-serviços.

### 3. Hardening de Segurança

- **Security Headers (CSP & HSTS)**: Implementados no Gateway para mitigar ataques de XSS e Data Injection. O header `Strict-Transport-Security` é aplicado de forma **condicional** (apenas em requisições HTTPS), garantindo que o ambiente de desenvolvimento local (`http://localhost`) permaneça funcional sem forçar certificados SSL inexistentes.
- **Typed DTOs**: Contratos de API 100% tipados no Gateway e nos Serviços, eliminando o uso de `object` ou `any`.
- **RowVersion (Planned)**: Suporte a concorrência otimista para evitar "race conditions" em atualizações simultâneas de estoque.

### 4. Korp.AI (Gemini Integration)

Integração direta com a AI do Google (Gemini 1.5) para analisar contextos de notas e sugerir itens/quantidades baseados nos produtos cadastrados.

---

## Endpoints & Swagger Documentation

Em ambiente de desenvolvimento, todos os microsserviços expõem documentação interativa via Swagger para facilitar o teste e integração:

| Microsserviço         | URL Base                | Link Swagger                                |
| :-------------------- | :---------------------- | :------------------------------------------ |
| **API Gateway**       | `http://localhost:5000` | [Swagger UI](http://localhost:5000/swagger) |
| **Inventory Service** | `http://localhost:5001` | [Swagger UI](http://localhost:5001/swagger) |
| **Billing Service**   | `http://localhost:5002` | [Swagger UI](http://localhost:5002/swagger) |

- **Gateway**: Centraliza as chamadas do Frontend. Utilize este endpoint para chamadas reais.
- **Serviços Internos**: Podem ser acessados diretamente para depuração e testes isolados.

---

## Como Executar

Cada serviço possui seu próprio Dockerfile otimizado:

```bash
# Executar testes unitários de todos os serviços
dotnet test
```

Os detalhes de variáveis de ambiente (ConnectionString, Gemini ApiKey) são configurados via arquivos `.env` ou injetados pelo Docker Compose.

---

**Tracked by Antigravity AI**

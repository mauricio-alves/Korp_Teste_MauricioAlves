# Korp Teste Tecnico - Sistema de Emissao de Notas Fiscais

Projeto desenvolvido como parte do processo seletivo para **Estagio de Desenvolvimento** na **Korp**.

---

## Sobre o Projeto

Sistema fullstack de emissao de notas fiscais construido com arquitetura de microsservicos.
O objetivo e gerenciar o ciclo completo: cadastro de produtos com controle de estoque, criacao de notas fiscais, e impressao com baixa automatica de saldo — com resiliencia real entre servicos.

### Funcionalidades Implementadas

- **CRUD de Produtos** — Cadastro, edicao, listagem e remocao de produtos com controle de saldo
- **Gestao de Notas Fiscais** — Criacao de notas com multiplos itens e status rastreavel
- **Impressao de Notas** — Fechamento da nota com debito automatico no estoque via HTTP
- **Resiliencia (Polly)** — Retry (3x) + Circuit Breaker entre microsservicos
- **Simulacao de Falha** — Endpoint dedicado para demonstrar o comportamento do circuit breaker
- **IA Aplicada** — Assistente que sugere produtos para a nota via Google Gemini API

---

## Arquitetura

```
Angular :4200  -->  API Gateway :5000  -->  Inventory Service :5001  -->  PostgreSQL (inventory_db)
                                       -->  Billing Service :5002   -->  PostgreSQL (billing_db)
                    API Gateway :5000  -->  Google Gemini API
```

Cada microsservico possui seu proprio banco de dados, respeitando a autonomia de dados da arquitetura de microsservicos.
Veja o mapa completo de dependencias em [CODEBASE.md](./CODEBASE.md).

---

## Monorepo Structure

```
Korp_Teste_MauricioAlves/
├── backend/    # 3 servicos C# .NET 8 + testes (ver backend/README.md)
├── frontend/   # Angular 18 + Angular Material (ver frontend/README.md)
├── docker/     # Scripts de inicializacao do banco
└── .github/    # CI/CD com GitHub Actions
```

- [Backend README](./backend/README.md) — Detalhes tecnicos do C#, LINQ, Polly, Swagger
- [Frontend README](./frontend/README.md) — Detalhes do Angular, RxJS, ciclos de vida, Material

---

## Como Executar

### Pre-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Node.js 20+](https://nodejs.org/)
- [Docker & Docker Compose](https://www.docker.com/)

### 1. Banco de Dados

```bash
# Copiar variaveis de ambiente
cp .env.example .env
# Editar .env com sua senha do PostgreSQL

# Subir PostgreSQL
docker-compose up -d
```

### 2. Backend

```bash
cd backend

# Inventory Service (terminal 1)
cd InventoryService
dotnet run

# Billing Service (terminal 2)
cd BillingService
dotnet run

# API Gateway (terminal 3)
cd ApiGateway
dotnet run
```

### 3. Frontend

```bash
cd frontend
npm install
npm start
# Acesse http://localhost:4200
```

---

## Qualidade de Codigo

| Projeto                | Cobertura                       | Ferramenta       |
| ---------------------- | ------------------------------- | ---------------- |
| InventoryService.Tests | > 80% (Services + Repositories) | coverlet + xUnit |
| BillingService.Tests   | > 80% (Services + Repositories) | coverlet + xUnit |
| Frontend               | > 80% (Services + Interceptors) | Istanbul / lcov  |

Praticas adotadas: **TDD**, **SOLID**, **Clean Architecture**, **Conventional Commits**, **CI/CD**.

---

## Stack Tecnologica

| Camada      | Tecnologia                                   |
| ----------- | -------------------------------------------- |
| Backend     | C# .NET 8, Entity Framework Core, PostgreSQL |
| Resiliencia | Polly (Retry + Circuit Breaker)              |
| IA          | Google Gemini API (free tier)                |
| Frontend    | Angular 18, Angular Material, RxJS           |
| Infra       | Docker, GitHub Actions                       |

---

Desenvolvido por [Mauricio Alves](https://www.linkedin.com/in/mauricio-oliveira-alves/) — Candidato a Estagio de Desenvolvimento

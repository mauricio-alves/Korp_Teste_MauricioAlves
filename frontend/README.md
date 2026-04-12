# Korp Note System - Frontend

Este é o portal de operação do sistema Korp, desenvolvido com **Angular 18** seguindo as melhores práticas de UI/UX e arquitetura reativa moderna.

## 🚀 Diferenciais Tecnológicos

- **Angular 18 (Standalone)**: Arquitetura sem módulos (No-NgModule), reduzindo complexidade e melhorando o tree-shaking.
- **State Management com Signals**: Utilização de `signals` e `computed` para gerenciamento de estado granular e reativo, eliminando problemas comuns de detecção de mudanças e garantindo performance superior.
- **Atomic Components**: Interface modularizada (Ex: `ItemSelector`, `ItemsGrid`, `InvoiceSummary`) para máxima reutilização e simplicidade (SRP).
- **Premium UI**: Design focado em produtividade, utilizando uma paleta de cores harmoniosa, tipografia moderna (Inter) e feedback visual em tempo real.

## 📁 Estrutura do Projeto

```bash
src/app/
├── core/               # Singleton Services, Models e Guards globais
│   ├── models/         # Interfaces de domínio (Product, Invoice, AI)
│   └── services/       # Proxy services para integração com API
├── features/           # Módulos de negócio (Faturamento, Catálogo, AI)
│   ├── invoices/       # Gestão de Notas com State Service via Signals
│   ├── products/       # Gestão de Catálogo de Produtos
│   └── ai-assistant/   # Integração com Korp AI (Gemini)
└── shared/             # Componentes, Pipes e Diretivas reutilizáveis
```

## 🛠 Comandos Disponíveis

- **`npm start`**: Inicia o servidor de desenvolvimento em `http://localhost:4200/`.
- **`npm test`**: Executa a suíte de testes unitários via Karma/Jasmine.
- **`npm run build`**: Gera o pacote de produção otimizado na pasta `dist/`.
- **`npm run lint`**: Executa a auditoria de estilo/estática de código.

## 🧪 Testes e Qualidade

O projeto utiliza o **Testing Pyramid** para garantir estabilidade:

- **Unit Tests**: Validação lógica de serviços e transformadores.
- **Component Tests**: Validação de renderização e interações de sub-componentes.
- **Validation**: O pipeline de CI no GitHub bloqueia automaticamente merges se a cobertura de testes falhar ou houver quebra de build.

---

**Tracked by Antigravity AI**

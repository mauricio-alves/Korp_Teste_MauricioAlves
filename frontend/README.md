# Korp Note System | Frontend & UX/UI

O frontend do **Korp Note System** é uma aplicação **Angular 18** de alta performance, desenhada para ser intuitiva, rápida e visualmente impactante.

---

## Reatividade com Angular Signals

Este projeto abandona o gerenciamento de estado complexo e verboso em favor dos **Angular Signals**.

- **InvoiceStateService**: Atua como o "Single Source of Truth" para o fluxo de faturamento.
- **Computed State**: Totais e resumos financeiros são recalculados automaticamente via `computed()`, garantindo precisão sem overhead de processamento.
- **Fine-grained Updates**: Apenas os elementos que realmente sofreram alteração são re-renderizados pelo Angular.

---

## Design System Customizado (Premium UI)

Não utilizamos frameworks CSS genéricos. O sistema utiliza **Vanilla CSS** com uma arquitetura de tokens modernos:

1. **Vibrant Dark Mode**: Baseado em tons de carvão e cinza-técnico com acentos em **Verde Ácido** e ciano.
2. **Glassmorphism**: Uso de transparências e `backdrop-filter` para dar profundidade à interface.
3. **Typography**: Foco em legibilidade com a fonte **Inter**, utilizando pesos variados para criar hierarquia visual clara.
4. **Grid Layout**: Sistema de grid customizado para o painel de faturamento, separando a seleção de itens do checkout financeiro de forma elegante.

---

## Korp.AI Terminal

A interface conta com um painel de IA integrado que se comporta como um terminal de diagnóstico:

- **Prompt Contextual**: A IA conhece os produtos disponíveis no estoque.
- **Sugestões Inteligentes**: Auxilia o operador a montar notas fiscais baseadas em descrições de linguagem natural.
- **Segurança**: Interpolação de texto segura para evitar execução de scripts maliciosos injetados por LLMs.

---

## Componentização (SRP)

O fluxo principal de faturamento foi decomposto em componentes especializados com responsabilidade única:

- `ItemSelector`: Busca e seleção de SKUs.
- `ItemsGrid`: Listagem reativa e remoção de itens.
- `InvoiceSummary`: Resumo financeiro e gatilho de fechamento da nota.
- `AiPanel`: Interface de conversa com o Gemini.

---

## Acessibilidade & Inclusão (A11y)

O sistema segue padrões básicos de acessibilidade para garantir que operadores com diferentes necessidades possam utilizar a ferramenta:

1.  **Semântica de Formulários**: Uso rigoroso de associações `label[for]` e `input[id]`.
2.  **Operação via Teclado**: Fluxos de faturamento otimizados para navegação sequencial.
3.  **Contraste Elevado**: O Dark Mode foi validado para garantir legibilidade agressiva (Brutalism Design).

---

## Como Executar

```bash
# Instalar dependências
npm install

# Iniciar ambiente de desenvolvimento
npm start
```

Acesse em: `http://localhost:4200`

---

**Desenvolvido com Antigravity AI**

# Frontend — Detalhamento Tecnico

Aplicacao Angular 18+ com standalone components, Angular Material e RxJS.

---

## Stack

| Tecnologia       | Versao | Uso                         |
| ---------------- | ------ | --------------------------- |
| Angular          | 18+    | Framework principal         |
| Angular Material | 18+    | Componentes de UI           |
| Angular CDK      | 18+    | Skeleton Loaders, Overlays  |
| RxJS             | 7.x    | Reatividade e chamadas HTTP |
| TypeScript       | 5.x    | Type safety                 |

---

## Gerenciamento de Dependencias (npm)

```bash
npm install            # Instala todas as dependencias
npm run start          # Servidor de desenvolvimento (:4200)
npm run build          # Build de producao
npm run test           # Testes com Karma/Jasmine
npm run lint           # ESLint
```

---

## Ciclos de Vida Angular Utilizados

| Lifecycle Hook  | Onde                                  | Para que                                                       |
| --------------- | ------------------------------------- | -------------------------------------------------------------- |
| `ngOnInit`      | Todos os componentes de lista/detalhe | Disparar chamada HTTP e carregar dados                         |
| `ngOnDestroy`   | Componentes com subscriptions manuais | Cancelar subscriptions e evitar memory leaks                   |
| `ngOnChanges`   | Componentes filho com `@Input`        | Reagir a mudancas de dados do componente pai                   |
| `AfterViewInit` | Tabelas com MatSort/MatPaginator      | Inicializar diretivas Material que dependem do DOM renderizado |

---

## RxJS — Operadores Utilizados

```typescript
// switchMap: encadeia chamadas (ex: confirmar produto antes de adicionar a nota)
this.productService.getById(productId).pipe(
  switchMap(product => this.invoiceService.addItem(invoiceId, { productId, quantity }))
);

// catchError: interceptor global de erros HTTP
return next(req).pipe(
  catchError((error: HttpErrorResponse) => {
    this.handleError(error);
    return throwError(() => error);
  })
);

// BehaviorSubject: estado compartilhado entre componentes
private productsSubject = new BehaviorSubject<Product[]>([]);
products$ = this.productsSubject.asObservable();

// debounceTime: filtro de busca com delay para evitar chamadas excessivas
this.searchControl.valueChanges.pipe(
  debounceTime(300),
  distinctUntilChanged(),
  switchMap(term => this.productService.search(term))
);

// takeUntilDestroyed: auto-unsubscribe (Angular 16+)
this.productService.getAll().pipe(
  takeUntilDestroyed(this.destroyRef)
).subscribe(products => this.products = products);
```

---

## Tratamento de Erros Global

**`error.interceptor.ts`** intercepta todos os erros HTTP e exibe feedback via MatSnackBar:

| Status      | Mensagem exibida                                         |
| ----------- | -------------------------------------------------------- |
| 503         | "Servico temporariamente indisponivel. Tente novamente." |
| 409         | "Saldo insuficiente para este produto."                  |
| 422         | Mensagem de validacao vinda do backend                   |
| 0 (network) | "Sem conexao com o servidor."                            |
| 500         | "Erro interno no servidor. Por favor, tente mais tarde." |

---

## Loading States

- **Skeleton Loaders** (Angular CDK): usados em tabelas de produtos e notas durante o carregamento inicial
- **MatProgressBar** (indeterminate): usado exclusivamente na acao de impressao de nota (acao pontual, nao carregamento de lista)

---

## Como Rodar

```bash
# Pre-requisito: backend rodando (ver backend/README.md)

cd frontend
npm install
npm start
# Acesse http://localhost:4200
```

## Rodar Testes

```bash
cd frontend
npm test                                    # Modo watch
npm run test -- --no-watch --code-coverage  # CI mode com coverage
```

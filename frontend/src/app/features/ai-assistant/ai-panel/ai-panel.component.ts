import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { AiService, AiSuggestionResponse } from '../../../core/services/ai.service';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';
import { finalize, switchMap } from 'rxjs';

@Component({
  selector: 'app-ai-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ai-panel.component.html',
  styleUrl: './ai-panel.component.scss',
})
export class AiPanelComponent {
  private readonly aiService = inject(AiService);
  private readonly productService = inject(ProductService);

  responseOutput = '';
  loading = false;
  isError = false;

  private readonly INITIAL_MESSAGE =
    '> INIT: CONEXÃO ESTABELECIDA. AGUARDANDO COMANDO DE VARREDURA DO OPERADOR.';

  get displayOutput(): string {
    return this.responseOutput || this.INITIAL_MESSAGE;
  }

  requestInventoryAnalysis() {
    this.loading = true;
    this.isError = false;
    this.responseOutput =
      'SINCRONIZANDO LINK DE REDE KORP...\n> EXTRAINDO MEMÓRIA VOLÁTIL DE INVENTÁRIO...';

    this.productService
      .getProducts()
      .pipe(
        switchMap((products: Product[]) => {
          const productList = products
            .map((p: Product) => `${p.code} (${p.description})`)
            .join(', ');

          if (!productList) {
            throw new Error('DATABASE_EMPTY');
          }

          return this.aiService.suggestProducts({
            context:
              'Gere uma análise objetiva, agressiva, brutalista e corporativa em parágrafos diretos analisando as fraquezas sistêmicas do inventário ou sugerindo combos de cross-sell e upsell altamente lucrativos. Comporte-se como um software militar de inteligência de Negócio brutal.',
            availableProducts: productList,
          });
        }),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: (res: AiSuggestionResponse) => {
          this.responseOutput =
            '> [KORP_NEURON_PROCESS_OK]\n\n' + res.suggestion;
        },
        error: (err: unknown) => {
          if (err instanceof Error && err.message === 'DATABASE_EMPTY') {
            this.responseOutput =
              'ERRO: Base de dados de produtos vazia. Adicione SKU no catálogo antes de usar o Módulo.';
          } else {
            this.responseOutput =
              'ERRO CRITICO_DE_CONEXAO_NEURAL: O ApiGateway ou o NÓ DO GEMINI RECUSOU COMUNICAÇÃO.';
            
            if (err instanceof HttpErrorResponse) {
              console.error(`[AI_PANEL] HTTP ${err.status}: ${err.message}`, err.error);
            }
          }
          this.isError = true;
        },
      });
  }
}

import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AiService } from '../../../core/services/ai.service';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-ai-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ai-panel.component.html',
  styleUrl: './ai-panel.component.scss',
})
export class AiPanelComponent {
  private aiService = inject(AiService);
  private productService = inject(ProductService);

  responseOutput = '';
  loading = false;

  requestInventoryAnalysis() {
    this.loading = true;
    this.responseOutput =
      'SINCRONIZANDO LINK DE REDE KORP...<br>> EXTRAINDO MEMÓRIA VOLÁTIL DE INVENTÁRIO...';

    this.productService.getProducts().subscribe({
      next: (products: Product[]) => {
        const productList = products
          .map((p: Product) => `${p.code} (${p.description})`)
          .join(', ');

        if (!productList) {
          this.responseOutput =
            '<span class="error">ERRO: Base de dados de produtos vazia. Adicione SKU no catálogo antes de usar o Módulo.</span>';
          this.loading = false;
          return;
        }

        this.aiService
          .suggestProducts({
            context:
              'Gere uma análise objetiva, agressiva, brutalista e corporativa em parágrafos diretos analisando as fraquezas sistêmicas do inventário ou sugerindo combos de cross-sell e upsell altamente lucrativos. Comporte-se como um software militar de inteligência de Negócio brutal.',
            availableProducts: productList,
          })
          .subscribe({
            next: (res: any) => {
              this.responseOutput =
                '> [KORP_NEURON_PROCESS_OK]<br><br>' +
                res.suggestion.replace(/\n/g, '<br>');
              this.loading = false;
            },
            error: () => {
              this.responseOutput =
                "<span class='error'>ERRO CRITICO_DE_CONEXAO_NEURAL: O ApiGateway ou o NÓ DO GEMINI RECUSOU COMUNICAÇÃO.</span>";
              this.loading = false;
            },
          });
      },
      error: () => (this.loading = false),
    });
  }
}

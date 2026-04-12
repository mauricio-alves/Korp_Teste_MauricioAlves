import { Component, inject, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InvoiceStateService } from '../services/invoice-state.service';

@Component({
  selector: 'app-invoice-summary',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card p-3 bg-dark-soft border-primary-vibrant h-100">
      <h3 class="text-primary-vibrant mb-4 text-uppercase tiny-text bold">
        Fechamento
      </h3>

      <div class="summary-details mb-4">
        <label class="tiny-text opacity-50 display-block mb-1 text-uppercase"
          >Montante Líquido</label
        >
        <div class="total-value text-primary-vibrant bold display-1">
          {{ totalAmount() | currency: 'BRL' }}
        </div>
      </div>

      <div class="ai-suggestion-placeholder mb-4">
        <div class="p-3 border-dashed opacity-25 text-center tiny-text italic">
          (Módulo Korp AI entrará aqui)
        </div>
      </div>

      <div class="actions">
        <button
          (click)="submit.emit()"
          class="k-btn-primary w-100 py-3 bold shadow-glow"
          [disabled]="!hasItems()"
        >
          TRANSMITIR NOTA FISCAL
        </button>
      </div>
    </div>
  `,
  styles: [
    `
      .display-1 {
        font-size: 2.5rem;
        letter-spacing: -2px;
      }
      .shadow-glow {
        box-shadow: 0 0 20px rgba(0, 255, 65, 0.2);
      }
      .border-dashed {
        border: 1px dashed rgba(0, 255, 65, 0.4);
        border-radius: 4px;
      }
    `,
  ],
})
export class InvoiceSummaryComponent {
  private readonly state = inject(InvoiceStateService);

  @Output() submit = new EventEmitter<void>();

  readonly totalAmount = this.state.totalAmount;
  readonly hasItems = this.state.hasItems;
}

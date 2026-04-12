import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InvoiceStateService } from '../services/invoice-state.service';

@Component({
  selector: 'app-items-grid',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card p-3 bg-dark-soft border-primary-vibrant h-100">
      <h3 class="text-primary-vibrant mb-3 text-uppercase tiny-text bold">
        Específicos da NFE
      </h3>
      <div class="table-responsive">
        <table class="table table-dark table-hover mb-0">
          <thead>
            <tr class="tiny-text opacity-50">
              <th>SKU</th>
              <th>QTD</th>
              <th>PREÇO</th>
              <th>TOT</th>
              <th class="text-center">ACT</th>
            </tr>
          </thead>
          <tbody>
            @for (item of items(); track $index) {
              <tr class="align-middle">
                <td>{{ item.productCode }}</td>
                <td>{{ item.quantity }} UN</td>
                <td>{{ item.unitPrice | currency: 'BRL' }}</td>
                <td class="text-primary-vibrant bold">
                  {{ item.total | currency: 'BRL' }}
                </td>
                <td class="text-center">
                  <button (click)="remove($index)" class="btn-delete-tiny">
                    ✕
                  </button>
                </td>
              </tr>
            } @empty {
              <tr>
                <td colspan="5" class="text-center p-5 opacity-25 italic">
                  Nenhum item lançado na operação.
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [
    `
      .btn-delete-tiny {
        background: rgba(255, 0, 0, 0.1);
        border: 1px solid rgba(255, 0, 0, 0.2);
        color: #ff4d4d;
        border-radius: 50%;
        width: 24px;
        height: 24px;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        transition: all 0.2s;
      }
      .btn-delete-tiny:hover {
        background: #ff4d4d;
        color: white;
      }
    `,
  ],
})
export class ItemsGridComponent {
  private readonly state = inject(InvoiceStateService);

  readonly items = this.state.items;

  remove(index: number) {
    this.state.removeItem(index);
  }
}

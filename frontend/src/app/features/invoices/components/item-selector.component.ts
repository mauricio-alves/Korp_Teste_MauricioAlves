import { Component, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InvoiceStateService } from '../services/invoice-state.service';
import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-item-selector',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="card p-3 mb-3 bg-dark-soft border-primary-vibrant">
      <h3 class="text-primary-vibrant mb-3 text-uppercase tiny-text bold">
        Lançar SKU na Operação
      </h3>
      <form
        [formGroup]="form"
        (ngSubmit)="addItem()"
        class="row g-2 align-items-end"
      >
        <div class="col-md-6">
          <label for="item-product" class="tiny-text opacity-50 mb-1"
            >Produto</label
          >
          <select
            id="item-product"
            formControlName="productId"
            class="k-input w-100"
          >
            <option value="">-- SELECIONE O PRODUTO --</option>
            @for (product of products; track product.id) {
              <option [value]="product.id">
                {{ product.code }} - {{ product.description }}
              </option>
            }
          </select>
        </div>
        <div class="col-md-2">
          <label for="item-qty" class="tiny-text opacity-50 mb-1">Qtd.</label>
          <input
            id="item-qty"
            type="number"
            formControlName="quantity"
            class="k-input w-100"
          />
        </div>
        <div class="col-md-2">
          <label for="item-price" class="tiny-text opacity-50 mb-1"
            >Preço Unit.</label
          >
          <input
            id="item-price"
            type="number"
            formControlName="unitPrice"
            class="k-input w-100"
          />
        </div>
        <div class="col-md-2">
          <button
            type="submit"
            class="k-btn-primary w-100"
            [disabled]="form.invalid"
          >
            ADICIONAR
          </button>
        </div>
      </form>
    </div>
  `,
})
export class ItemSelectorComponent {
  @Input() products: Product[] = [];

  private readonly fb = inject(FormBuilder);
  private readonly state = inject(InvoiceStateService);

  form = this.fb.group({
    productId: ['', Validators.required],
    quantity: [1, [Validators.required, Validators.min(1)]],
    unitPrice: [100, [Validators.required, Validators.min(0.01)]],
  });

  addItem() {
    if (this.form.valid) {
      const val = this.form.value;
      const product = this.products.find((p) => p.id === val.productId);
      if (product) {
        this.state.addItem({
          productId: product.id,
          productCode: product.code,
          productDescription: product.description || 'Sem Descrição',
          quantity: val.quantity!,
          unitPrice: val.unitPrice!,
          total: val.quantity! * val.unitPrice!,
        });
        this.form.reset({ productId: '', quantity: 1, unitPrice: 100 });
      }
    }
  }
}

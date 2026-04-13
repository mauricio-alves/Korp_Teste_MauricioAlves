import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { InvoiceService } from '../../../core/services/invoice.service';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';
import { CreateInvoiceDto } from '../../../core/models/invoice.model';
import { InvoiceStateService } from '../services/invoice-state.service';
import { ItemSelectorComponent } from '../components/item-selector.component';
import { ItemsGridComponent } from '../components/items-grid.component';
import { InvoiceSummaryComponent } from '../components/invoice-summary.component';

@Component({
  selector: 'app-invoice-create',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ItemSelectorComponent,
    ItemsGridComponent,
    InvoiceSummaryComponent,
  ],
  providers: [InvoiceStateService],
  templateUrl: './invoice-create.component.html',
  styleUrl: './invoice-create.component.scss',
})
export class InvoiceCreateComponent implements OnInit {
  private readonly invoiceService = inject(InvoiceService);
  private readonly productService = inject(ProductService);
  private readonly state = inject(InvoiceStateService);
  private readonly router = inject(Router);

  products: Product[] = [];
  loadingProducts = false;
  submitting = false;

  ngOnInit() {
    this.loadingProducts = true;
    this.productService.getProducts().subscribe({
      next: (pts: Product[]) => {
        this.products = pts;
        this.loadingProducts = false;
      },
      error: () => (this.loadingProducts = false),
    });
  }

  submitInvoice() {
    if (this.submitting) return;

    const items = this.state.items();
    if (items.length > 0) {
      this.submitting = true;
      const payload: CreateInvoiceDto = {
        items: items.map((i) => ({
          productId: i.productId,
          productCode: i.productCode,
          productDescription: i.productDescription,
          quantity: i.quantity,
        })),
      };
      this.invoiceService.createInvoice(payload).subscribe({
        next: () => {
          this.state.clear();
          this.router.navigate(['/invoices']);
        },
        error: () => (this.submitting = false),
      });
    }
  }
}

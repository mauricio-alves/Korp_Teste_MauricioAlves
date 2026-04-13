import { Routes } from '@angular/router';
import { ProductListComponent } from './features/products/product-list/product-list.component';
import { ProductFormComponent } from './features/products/product-form/product-form.component';
import { InvoiceListComponent } from './features/invoices/invoice-list/invoice-list.component';
import { InvoiceCreateComponent } from './features/invoices/invoice-create/invoice-create.component';
import { AiPanelComponent } from './features/ai-assistant/ai-panel/ai-panel.component';

export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  { path: 'products', component: ProductListComponent },
  { path: 'products/new', component: ProductFormComponent },
  { path: 'invoices', component: InvoiceListComponent },
  { path: 'invoices/new', component: InvoiceCreateComponent },
  { path: 'ai-assistant', component: AiPanelComponent },
];

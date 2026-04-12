import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice, AddInvoiceItemDto } from '../models/invoice.model';

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = '/api/invoices';

  getInvoices(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.API_URL);
  }

  createInvoice(payload: any): Observable<Invoice> {
    return this.http.post<Invoice>(this.API_URL, payload);
  }

  addItem(invoiceId: string, data: AddInvoiceItemDto): Observable<any> {
    return this.http.post(`${this.API_URL}/${invoiceId}/items`, data);
  }

  removeItem(invoiceId: string, itemId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.API_URL}/${invoiceId}/items/${itemId}`,
    );
  }

  printInvoice(invoiceId: string): Observable<void> {
    return this.http.post<void>(`${this.API_URL}/${invoiceId}/print`, {});
  }
}

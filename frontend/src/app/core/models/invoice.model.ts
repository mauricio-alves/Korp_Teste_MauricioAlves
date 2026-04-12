export type InvoiceStatus = 'Open' | 'Closed';

export interface InvoiceItem {
  id: string;
  productId: string;
  productCode: string;
  productDescription: string;
  quantity: number;
}

export interface Invoice {
  id: string;
  number: number;
  status: InvoiceStatus;
  createdAt: string;
  closedAt?: string;
  items: InvoiceItem[];
}

export interface AddInvoiceItemDto {
  productId: string;
  productCode: string;
  productDescription: string;
  quantity: number;
}

export interface CreateInvoiceDto {
  items: Array<{
    productId: string;
    productCode: string;
    productDescription: string;
    quantity: number;
  }>;
}

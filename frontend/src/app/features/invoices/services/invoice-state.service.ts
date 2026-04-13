import { Injectable, signal, computed } from '@angular/core';

export interface SelectedItem {
  productId: string;
  productCode: string;
  productDescription: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

@Injectable()
export class InvoiceStateService {
  private readonly _items = signal<SelectedItem[]>([]);

  readonly items = this._items.asReadonly();

  readonly totalAmount = computed(() =>
    this._items().reduce((acc, item) => acc + item.total, 0),
  );

  readonly hasItems = computed(() => this._items().length > 0);

  addItem(item: SelectedItem) {
    this._items.update((prev) => [...prev, item]);
  }

  removeItem(index: number) {
    this._items.update((prev) => {
      const next = [...prev];
      next.splice(index, 1);
      return next;
    });
  }

  clear() {
    this._items.set([]);
  }
}

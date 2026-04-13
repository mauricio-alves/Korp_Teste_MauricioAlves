import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CreateProductDto } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss',
})
export class ProductFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);

  form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['', [Validators.required, Validators.maxLength(200)]],
    balance: [0, [Validators.required, Validators.min(0)]],
  });

  submitting = false;

  submit() {
    if (this.form.valid) {
      this.submitting = true;
      this.productService
        .createProduct(this.form.value as CreateProductDto)
        .subscribe({
          next: () => {
            this.router.navigate(['/products']);
          },
          error: () => (this.submitting = false),
        });
    }
  }
}

import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss',
})
export class ProductFormComponent {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private router = inject(Router);

  form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['', [Validators.required, Validators.maxLength(200)]],
    initialBalance: [0, [Validators.required, Validators.min(0)]],
  });

  submitting = false;

  submit() {
    if (this.form.valid) {
      this.submitting = true;
      this.productService.createProduct(this.form.value as any).subscribe({
        next: () => {
          this.router.navigate(['/products']);
        },
        error: () => (this.submitting = false),
      });
    }
  }
}

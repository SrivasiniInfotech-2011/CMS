import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnChanges, OnDestroy, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { Customer, CustomerService } from '../customer.service';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './customer-form.component.html',
  styleUrl: './customer-form.component.scss',
})
export class CustomerFormComponent implements OnChanges, OnDestroy {
  @Input() customer: Customer | null = null;
  @Input() mode: 'create' | 'edit' | 'view' = 'create';
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  customerForm: FormGroup;
  isSaving = false;
  customerImage: File | null = null;
  imagePreviewUrl: string | null = null;
  imageError = '';
  saveError = '';

  constructor(
    private readonly fb: FormBuilder,
    private readonly customerService: CustomerService,
  ) {
    this.customerForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      middleName: [''],
      dob: ['', [Validators.required]],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['customer'] || changes['mode']) {
      this.clearSelectedImage();

      if (this.customer) {
        this.customerForm.patchValue({
          firstName: this.customer.firstName ?? '',
          lastName: this.customer.lastName ?? '',
          middleName: this.customer.middleName ?? '',
          dob: this.customer.dob ?? '',
        });
      } else {
        this.customerForm.reset({
          firstName: '',
          lastName: '',
          middleName: '',
          dob: '',
        });
      }

      if (this.mode === 'view') {
        this.customerForm.disable();
      } else {
        this.customerForm.enable();
      }
    }
  }

  ngOnDestroy(): void {
    this.revokeImagePreview();
  }

  openImagePicker(fileInput: HTMLInputElement): void {
    if (this.mode !== 'view') {
      fileInput.click();
    }
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      this.clearSelectedImage();
      this.imageError = 'Choose a JPEG, PNG, or WebP image.';
      input.value = '';
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.clearSelectedImage();
      this.imageError = 'The image must be 5 MB or smaller.';
      input.value = '';
      return;
    }

    this.revokeImagePreview();
    this.customerImage = file;
    this.imagePreviewUrl = URL.createObjectURL(file);
    this.imageError = '';
    this.saveError = '';
  }

  onSubmit(): void {
    if (this.customerForm.invalid || this.mode === 'view') {
      this.customerForm.markAllAsTouched();
      return;
    }

    const payload: Customer = {
      ...this.customerForm.getRawValue(),
      id: this.customer?.id,
    };

    let request$: Observable<Customer>;
    if (this.mode === 'edit' && this.customer?.id) {
      request$ = this.customerService.updateCustomer(this.customer.id, payload);
    } else if (this.mode === 'create' && this.customerImage) {
      request$ = this.customerService.createCustomer(payload, this.customerImage);
    } else {
      this.imageError = 'A customer image is required.';
      return;
    }

    this.isSaving = true;
    this.saveError = '';
    request$.subscribe({
      next: () => {
        this.isSaving = false;
        this.saved.emit();
      },
      error: (error: HttpErrorResponse) => {
        this.isSaving = false;
        this.saveError =
          typeof error.error === 'string'
            ? error.error
            : 'Unable to save the customer. Please try again.';
      },
    });
  }

  cancel(): void {
    this.cancelled.emit();
  }

  markEditable(): void {
    if (this.mode === 'view' && this.customer?.id) {
      this.mode = 'edit';
      this.customerForm.enable();
    }
  }

  private clearSelectedImage(): void {
    this.revokeImagePreview();
    this.customerImage = null;
    this.imageError = '';
    this.saveError = '';
  }

  private revokeImagePreview(): void {
    if (this.imagePreviewUrl) {
      URL.revokeObjectURL(this.imagePreviewUrl);
      this.imagePreviewUrl = null;
    }
  }
}

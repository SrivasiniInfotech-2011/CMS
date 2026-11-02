import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { Customer, CustomerService } from '../customer.service';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './customer-form.component.html',
  styleUrl: './customer-form.component.scss',
})
export class CustomerFormComponent implements OnChanges {
  @Input() customer: Customer | null = null;
  @Input() mode: 'create' | 'edit' | 'view' = 'create';
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  customerForm: FormGroup;
  isSaving = false;

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

  onSubmit(): void {
    if (this.customerForm.invalid || this.mode === 'view') {
      this.customerForm.markAllAsTouched();
      return;
    }

    const payload: Customer = {
      ...this.customerForm.getRawValue(),
      id: this.customer?.id,
    };

    this.isSaving = true;

    const request$ =
      this.mode === 'edit' && this.customer?.id
        ? this.customerService.updateCustomer(this.customer.id, payload)
        : this.customerService.createCustomer(payload);

    request$.subscribe({
      next: () => {
        this.isSaving = false;
        this.saved.emit();
      },
      error: () => {
        this.isSaving = false;
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
}

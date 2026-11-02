import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

import { CustomerFormComponent } from '../customer-form/customer-form.component';
import { Customer, CustomerService } from '../customer.service';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, CustomerFormComponent],
  templateUrl: './customer-list.component.html',
  styleUrl: './customer-list.component.scss',
})
export class CustomerListComponent implements OnInit {
  customers: Customer[] = [];
  currentPage = 1;
  readonly pageSize = 5;
  currentCustomer: Customer | null = null;
  formMode: 'create' | 'edit' | 'view' = 'create';
  showForm = false;

  constructor(private readonly customerService: CustomerService) {}

  ngOnInit(): void {
    this.loadCustomers();
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.customers.length / this.pageSize));
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, index) => index + 1);
  }

  get paginatedCustomers(): Customer[] {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return this.customers.slice(start, end);
  }

  loadCustomers(): void {
    this.customerService.getCustomers().subscribe({
      next: (customers) => {
        this.customers = Array.isArray(customers) ? customers : customers ? [customers] : [];
        this.currentPage = 1;
      },
      error: () => {
        this.customers = [];
        this.currentPage = 1;
      },
    });
  }

  goToPage(pageNumber: number): void {
    if (pageNumber >= 1 && pageNumber <= this.totalPages) {
      this.currentPage = pageNumber;
    }
  }

  openCreateForm(): void {
    this.currentCustomer = null;
    this.formMode = 'create';
    this.showForm = true;
  }

  openViewForm(customer: Customer): void {
    this.currentCustomer = customer;
    this.formMode = 'view';
    this.showForm = true;
  }

  openEditForm(customer: Customer): void {
    this.currentCustomer = customer;
    this.formMode = 'edit';
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.currentCustomer = null;
    this.formMode = 'create';
    this.loadCustomers();
  }
}

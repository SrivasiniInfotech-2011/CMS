import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

export interface Customer {
  id?: number;
  firstName?: string;
  lastName?: string;
  middleName?: string;
  dob?: string | null;
  imageUrl?: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  private readonly apiUrl = 'https://localhost:7232/api/Customer';
  private readonly authToken =
    'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzkzNzEwMzgxLCJleHAiOjE3OTM3MTM5ODEsImlhdCI6MTc5MzcxMDM4MSwiaXNzIjoiQ01TIiwiYXVkIjoiQ01TLUFwaSJ9._PqShkBwgE63muXnHgXyuVxCmdqsL0JGDMo4G6vXPC4';

  constructor(private readonly http: HttpClient) {}

  private buildHeaders(): HttpHeaders {
    return new HttpHeaders({
      Authorization: `Bearer ${this.authToken}`,
      Accept: '*/*',
    });
  }

  private isCustomerLike(value: unknown): value is Customer {
    if (!value || typeof value !== 'object') {
      return false;
    }

    const candidate = value as Record<string, unknown>;
    return (
      typeof candidate['firstName'] === 'string' ||
      typeof candidate['lastName'] === 'string' ||
      typeof candidate['id'] === 'number' ||
      typeof candidate['dob'] === 'string'
    );
  }

  private normalizeCustomers(response: unknown): Customer[] {
    if (!response) {
      return [];
    }

    if (Array.isArray(response)) {
      return response.filter((item) => this.isCustomerLike(item)) as Customer[];
    }

    if (typeof response === 'object') {
      const obj = response as Record<string, unknown>;

      for (const key of ['data', 'result', 'customers', 'items', 'value']) {
        if (obj[key] !== undefined) {
          return this.normalizeCustomers(obj[key]);
        }
      }

      const numericEntries = Object.values(obj)
        .filter((entry) => this.isCustomerLike(entry)) as Customer[];

      if (numericEntries.length > 0) {
        return numericEntries;
      }

      if (this.isCustomerLike(response)) {
        return [response as Customer];
      }
    }

    return [];
  }

  getCustomers(): Observable<Customer[]> {
    return this.http
      .get<Customer[] | Customer>(this.apiUrl, {
        headers: this.buildHeaders(),
      })
      .pipe(map((response) => this.normalizeCustomers(response)));
  }

  getCustomer(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.apiUrl}/${id}`, {
      headers: this.buildHeaders(),
    });
  }

  createCustomer(customer: Customer, customerImage: File): Observable<Customer> {
    const formData = new FormData();
    formData.append('FirstName', customer.firstName ?? '');
    formData.append('LastName', customer.lastName ?? '');
    formData.append('MiddleName', customer.middleName ?? '');
    if (customer.dob) {
      formData.append('Dob', customer.dob);
    }
    formData.append('customerImage', customerImage, customerImage.name);

    return this.http.post<Customer>(this.apiUrl, formData, {
      headers: this.buildHeaders(),
    });
  }

  updateCustomer(id: number, customer: Customer): Observable<Customer> {
    return this.http.put<Customer>(`${this.apiUrl}/${id}`, customer, {
      headers: this.buildHeaders(),
    });
  }
}

import { Component } from '@angular/core';

import { CustomerListComponent } from './customer-list/customer-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CustomerListComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}

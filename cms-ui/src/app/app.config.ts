import { provideHttpClient, withFetch } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, Routes } from '@angular/router';

import { CustomerListCanLoadGuard } from './auth/customer-list-can-load.guard';
import { LoginComponent } from './login/login.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: 'customers',
    loadComponent: () => import('./customer-list/customer-list.component').then((m) => m.CustomerListComponent),
    canLoad: [CustomerListCanLoadGuard],
  },
  { path: '**', redirectTo: 'login' },
];

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideHttpClient(withFetch()),
    provideRouter(routes),
  ],
};

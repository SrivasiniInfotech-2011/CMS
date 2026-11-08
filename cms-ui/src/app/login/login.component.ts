import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  template: `
    <section class="login-page">
      <div class="login-card">
        <h2>Login</h2>
        <p>Use this demo login to access the customer list route.</p>
        <button type="button" (click)="login()">Login to continue</button>
      </div>
    </section>
  `,
  styles: `
    :host {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 100vh;
      background: #f8fafc;
      font-family: Arial, sans-serif;
    }

    .login-card {
      width: min(420px, 90vw);
      background: #fff;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      box-shadow: 0 8px 24px rgba(15, 23, 42, 0.08);
      padding: 32px 24px;
      text-align: center;
    }

    h2 {
      margin: 0 0 12px;
      font-size: 2rem;
    }

    p {
      margin: 0 0 20px;
      color: #475569;
    }

    button {
      background: #1d4ed8;
      color: #fff;
      border: none;
      border-radius: 8px;
      padding: 12px 18px;
      cursor: pointer;
      font-size: 1rem;
    }
  `,
})
export class LoginComponent {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {}

  login(): void {
    this.authService.login();
    this.router.navigate(['/customers']);
  }
}

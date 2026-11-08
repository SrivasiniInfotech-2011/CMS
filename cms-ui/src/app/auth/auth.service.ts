import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly authKey = 'cms-authenticated';

  isAuthenticated(): boolean {
    return localStorage.getItem(this.authKey) === 'true';
  }

  login(): void {
    localStorage.setItem(this.authKey, 'true');
  }

  logout(): void {
    localStorage.removeItem(this.authKey);
  }
}

import { Injectable } from '@angular/core';
import { CanLoad, Route, Router, UrlSegment } from '@angular/router';

import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class CustomerListCanLoadGuard implements CanLoad {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {}

  canLoad(route: Route, segments: UrlSegment[]): boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    }

    if (this.router.url !== '/login') {
      this.router.navigate(['/login'], { replaceUrl: true });
    }

    return false;
  }
}

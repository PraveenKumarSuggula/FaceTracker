import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const token = this.authService.getToken();
    const userRole = this.authService.getRole();
    const allowedRoles = route.data['roles'] as string[];

    if (token && userRole && allowedRoles.includes(userRole)) {
      return true;
    }

    this.router.navigate(['/login']);
    return false;
  }
}

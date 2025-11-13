import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/auth/login'], { 
        queryParams: { returnUrl: state.url } 
      });
      return of(false);
    }

    if (this.authService.needsTokenRefresh()) {
      return this.authService.refreshToken().pipe(
        map(() => true),
        catchError(() => {
          this.authService.logout();
          this.router.navigate(['/auth/login'], { 
            queryParams: { returnUrl: state.url } 
          });
          return of(false);
        })
      );
    }

    return of(true);
  }
}

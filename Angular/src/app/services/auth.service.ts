import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { map, catchError, tap, shareReplay } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { 
  LoginRequest, 
  RegisterRequest, 
  AuthResponse, 
  RefreshTokenRequest, 
  AuthUser, 
  TokenValidationResponse 
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;
  
  private currentUserSubject = new BehaviorSubject<AuthUser | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  constructor() {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = this.getAccessToken();
    if (token && !this.isTokenExpired(token)) {
      this.validateToken().subscribe({
        next: (response) => {
          if (response.isValid) {
            this.setCurrentUser({
              id: response.userId,
              userName: response.userName,
              email: response.email,
              roles: response.roles,
              isActive: true,
              createdAt: new Date()
            });
          } else {
            this.logout();
          }
        },
        error: () => {
          this.logout();
        }
      });
    }
  }

  login(loginRequest: LoginRequest): Observable<AuthResponse> {
    this.setLoading(true);
    
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, loginRequest).pipe(
      tap(response => {
        this.setTokens(response.accessToken, response.refreshToken);
        this.setCurrentUser(response.user);
        this.setLoading(false);
      }),
      catchError(error => {
        this.setLoading(false);
        return throwError(() => error);
      }),
      shareReplay(1)
    );
  }

  register(registerRequest: RegisterRequest): Observable<AuthResponse> {
    this.setLoading(true);
    
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, registerRequest).pipe(
      tap(response => {
        this.setTokens(response.accessToken, response.refreshToken);
        this.setCurrentUser(response.user);
        this.setLoading(false);
      }),
      catchError(error => {
        this.setLoading(false);
        return throwError(() => error);
      }),
      shareReplay(1)
    );
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token available'));
    }

    const refreshRequest: RefreshTokenRequest = { refreshToken };
    
    return this.http.post<AuthResponse>(`${this.baseUrl}/refresh`, refreshRequest).pipe(
      tap(response => {
        this.setTokens(response.accessToken, response.refreshToken);
        this.setCurrentUser(response.user);
      }),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      }),
      shareReplay(1)
    );
  }

  logout(): Observable<any> {
    const refreshToken = this.getRefreshToken();
    
    if (refreshToken) {
      const refreshRequest: RefreshTokenRequest = { refreshToken };
      return this.http.post(`${this.baseUrl}/logout`, refreshRequest).pipe(
        tap(() => {
          this.clearAuth();
        }),
        catchError(() => {
          this.clearAuth();
          return of(null);
        })
      );
    } else {
      this.clearAuth();
      return of(null);
    }
  }

  validateToken(): Observable<TokenValidationResponse> {
    return this.http.get<TokenValidationResponse>(`${this.baseUrl}/validate`).pipe(
      catchError(() => of({ isValid: false } as TokenValidationResponse))
    );
  }

  getCurrentUser(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user ? user.roles.includes(role) : false;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    return user ? roles.some(role => user.roles.includes(role)) : false;
  }

  getAccessToken(): string | null {
    return localStorage.getItem('access_token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  private setTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem('access_token', accessToken);
    localStorage.setItem('refresh_token', refreshToken);
  }

  private setCurrentUser(user: AuthUser): void {
    this.currentUserSubject.next(user);
    this.isAuthenticatedSubject.next(true);
  }

  private clearAuth(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  private setLoading(loading: boolean): void {
    this.loadingSubject.next(loading);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Date.now() / 1000;
      return payload.exp < currentTime;
    } catch {
      return true;
    }
  }

  getTokenExpiration(): Date | null {
    const token = this.getAccessToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return new Date(payload.exp * 1000);
    } catch {
      return null;
    }
  }

  needsTokenRefresh(): boolean {
    const expiration = this.getTokenExpiration();
    if (!expiration) return true;

    const fiveMinutes = 5 * 60 * 1000; // 5 minutes in milliseconds
    return expiration.getTime() - Date.now() < fiveMinutes;
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, of, throwError } from 'rxjs';
import { map, catchError, tap, shareReplay } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Permission } from '../models/permission.model';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/permissions`;
  
  private permissionsCache$ = new BehaviorSubject<Permission[]>([]);
  private permissionsCacheLoaded = false;
  
  getUserPermissions(userId: number, forceRefresh = false): Observable<Permission[]> {
    if (this.permissionsCacheLoaded && !forceRefresh) {
      return this.permissionsCache$.asObservable();
    }

    return this.http.get<Permission[]>(`${this.baseUrl}/user/${userId}`).pipe(
      tap(permissions => {
        this.permissionsCache$.next(permissions);
        this.permissionsCacheLoaded = true;
      }),
      catchError(error => {
        console.error('Error fetching user permissions:', error);
        return of([]);
      }),
      shareReplay(1)
    );
  }

  getCachedPermissions(): Permission[] {
    return this.permissionsCache$.value;
  }

  hasPermission(permissionName: string): Observable<boolean> {
    const cachedPermissions = this.getCachedPermissions();
    if (this.permissionsCacheLoaded && cachedPermissions.length > 0) {
      const hasPermission = cachedPermissions.some(p => p.name === permissionName);
      return of(hasPermission);
    }

    console.warn('Permissions not cached. Ensure permissions are loaded during authentication.');
    return of(false);
  }

  hasPermissionSync(permissionName: string): boolean {
    const permissions = this.getCachedPermissions();
    return permissions.some(p => p.name === permissionName);
  }

  hasAnyPermission(permissionNames: string[]): Observable<boolean> {
    const cachedPermissions = this.getCachedPermissions();
    
    if (this.permissionsCacheLoaded && cachedPermissions.length > 0) {
      const hasAny = permissionNames.some(permName => 
        cachedPermissions.some(p => p.name === permName)
      );
      return of(hasAny);
    }

    console.warn('Permissions not cached. Ensure permissions are loaded during authentication.');
    return of(false);
  }

  hasAnyPermissionSync(permissionNames: string[]): boolean {
    const permissions = this.getCachedPermissions();
    return permissionNames.some(permName => 
      permissions.some(p => p.name === permName)
    );
  }

  hasAllPermissions(permissionNames: string[]): Observable<boolean> {
    const cachedPermissions = this.getCachedPermissions();
    
    if (this.permissionsCacheLoaded && cachedPermissions.length > 0) {
      const hasAll = permissionNames.every(permName => 
        cachedPermissions.some(p => p.name === permName)
      );
      return of(hasAll);
    }

    console.warn('Permissions not cached. Ensure permissions are loaded during authentication.');
    return of(false);
  }

  hasAllPermissionsSync(permissionNames: string[]): boolean {
    const permissions = this.getCachedPermissions();
    return permissionNames.every(permName => 
      permissions.some(p => p.name === permName)
    );
  }

  hasResourceActionPermission(resource: string, action: string): boolean {
    const permissionName = `${resource}_${action}`;
    return this.hasPermissionSync(permissionName);
  }

  getResourcePermissions(resource: string): Permission[] {
    const permissions = this.getCachedPermissions();
    return permissions.filter(p => p.resource === resource);
  }

  clearCache(): void {
    this.permissionsCache$.next([]);
    this.permissionsCacheLoaded = false;
  }

  refreshPermissions(userId: number): Observable<Permission[]> {
    return this.getUserPermissions(userId, true);
  }

  getAllPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(this.baseUrl).pipe(
      catchError(error => {
        console.error('Error fetching all permissions:', error);
        return throwError(() => error);
      })
    );
  }

  getPermissionsByRole(roleId: number): Observable<Permission[]> {
    return this.http.get<Permission[]>(`${this.baseUrl}/role/${roleId}`).pipe(
      catchError(error => {
        console.error('Error fetching role permissions:', error);
        return throwError(() => error);
      })
    );
  }
}


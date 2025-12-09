import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { GenericCrudService, PaginationParams, PaginatedResponse, ApiResponse } from './generic-crud.service';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService extends GenericCrudService<User> {
  constructor(http: HttpClient) {
    super(http, 'Users');
  }

  getUsersByRole(role: string, params?: PaginationParams): Observable<PaginatedResponse<User>> {
    const filters = { ...params?.filters, roles: role };
    return this.search(filters, params);
  }

  getActiveUsers(params?: PaginationParams): Observable<PaginatedResponse<User>> {
    const filters = { ...params?.filters, isActive: true };
    return this.search(filters, params);
  }

  getInactiveUsers(params?: PaginationParams): Observable<PaginatedResponse<User>> {
    const filters = { ...params?.filters, isActive: false };
    return this.search(filters, params);
  }

  getUserByEmail(email: string): Observable<User> {
    return this.http.get<ApiResponse<User>>(`${this.baseUrl}/email/${email}`)
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  getUserByUsername(username: string): Observable<User> {
    return this.http.get<ApiResponse<User>>(`${this.baseUrl}/username/${username}`)
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  assignRoles(userId: number, roles: string[]): Observable<User> {
    return this.http.post<ApiResponse<User>>(`${this.baseUrl}/${userId}/assign-roles`, { roles })
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  removeRoles(userId: number, roles: string[]): Observable<User> {
    return this.http.post<ApiResponse<User>>(`${this.baseUrl}/${userId}/remove-roles`, { roles })
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  changePassword(userId: number, currentPassword: string, newPassword: string): Observable<User> {
    return this.http.post<ApiResponse<User>>(`${this.baseUrl}/${userId}/change-password`, {
      currentPassword,
      newPassword
    })
    .pipe(
      map(response => response.data),
      catchError(this.handleError.bind(this))
    );
  }

  resetPassword(userId: number): Observable<string> {
    return this.http.post<ApiResponse<string>>(`${this.baseUrl}/${userId}/reset-password`, {})
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  activateUser(userId: number): Observable<User> {
    return this.update(userId, { isActive: true });
  }

  deactivateUser(userId: number): Observable<User> {
    return this.update(userId, { isActive: false });
  }

  getUsersWithLastLoginAfter(date: Date, params?: PaginationParams): Observable<PaginatedResponse<User>> {
    const filters = { ...params?.filters, lastLoginAfter: date.toISOString() };
    return this.search(filters, params);
  }

  getUsersCreatedBetween(startDate: Date, endDate: Date, params?: PaginationParams): Observable<PaginatedResponse<User>> {
    const filters = { 
      ...params?.filters, 
      createdAfter: startDate.toISOString(),
      createdBefore: endDate.toISOString()
    };
    return this.search(filters, params);
  }

  getUsersBySearchTerm(searchTerm: string, params?: PaginationParams): Observable<PaginatedResponse<User>> {
    const searchParams = { ...params, searchTerm };
    return this.getAll(searchParams);
  }

  importUsersFromCsv(file: File): Observable<{ success: number; errors: string[] }> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<ApiResponse<{ success: number; errors: string[] }>>(`${this.baseUrl}/import`, formData)
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  getUserStats(): Observable<{
    totalUsers: number;
    activeUsers: number;
    inactiveUsers: number;
    newUsersThisMonth: number;
    usersByRole: { [key: string]: number };
  }> {
    return this.http.get<ApiResponse<{
      totalUsers: number;
      activeUsers: number;
      inactiveUsers: number;
      newUsersThisMonth: number;
      usersByRole: { [key: string]: number };
    }>>(`${this.baseUrl}/stats`)
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  sendWelcomeEmail(userId: number): Observable<boolean> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/${userId}/send-welcome-email`, {})
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  bulkAssignRoles(userIds: number[], roles: string[]): Observable<User[]> {
    return this.http.post<ApiResponse<User[]>>(`${this.baseUrl}/bulk-assign-roles`, {
      userIds,
      roles
    })
    .pipe(
      map(response => response.data),
      catchError(this.handleError.bind(this))
    );
  }

  bulkRemoveRoles(userIds: number[], roles: string[]): Observable<User[]> {
    return this.http.post<ApiResponse<User[]>>(`${this.baseUrl}/bulk-remove-roles`, {
      userIds,
      roles
    })
    .pipe(
      map(response => response.data),
      catchError(this.handleError.bind(this))
    );
  }

  bulkActivateUsers(userIds: number[]): Observable<User[]> {
    return this.http.post<ApiResponse<User[]>>(`${this.baseUrl}/bulk-activate`, { userIds })
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  bulkDeactivateUsers(userIds: number[]): Observable<User[]> {
    return this.http.post<ApiResponse<User[]>>(`${this.baseUrl}/bulk-deactivate`, { userIds })
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }
}
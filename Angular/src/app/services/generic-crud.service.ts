import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface PaginationParams {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  searchTerm?: string;
  filters?: { [key: string]: any };
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
}

export interface CrudEntity {
  id?: number;
  [key: string]: any;
}

@Injectable()
export class GenericCrudService<T extends CrudEntity> {
  protected readonly baseUrl: string;
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private errorSubject = new BehaviorSubject<string | null>(null);

  public loading$ = this.loadingSubject.asObservable();
  public error$ = this.errorSubject.asObservable();

  constructor(
    protected http: HttpClient,
    @Inject('ENDPOINT') endpoint: string
  ) {
    this.baseUrl = `${environment.apiUrl}/${endpoint}`;
  }

  getAll(params?: PaginationParams): Observable<PaginatedResponse<T>> {
    this.setLoading(true);

    let httpParams = new HttpParams();

    if (params) {
      if (params.page !== undefined) httpParams = httpParams.set('pageNumber', params.page.toString());
      if (params.pageSize !== undefined) httpParams = httpParams.set('pageSize', params.pageSize.toString());
      if (params.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
      if (params.sortDirection) httpParams = httpParams.set('sortDirection', params.sortDirection);
      if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);

      if (params.filters) {
        Object.keys(params.filters).forEach(key => {
          if (params.filters![key] !== null && params.filters![key] !== undefined && params.filters![key] !== '') {
            httpParams = httpParams.set(key, params.filters![key].toString());
          }
        });
      }
    }

    return this.http.get<ApiResponse<PaginatedResponse<T>>>(this.baseUrl, { params: httpParams })
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  getAllSimple(): Observable<T[]> {
    this.setLoading(true);

    return this.http.get<ApiResponse<T[]>>(`${this.baseUrl}/all`)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  getById(id: number): Observable<T> {
    this.setLoading(true);

    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${id}`)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  create(item: Partial<T>): Observable<T> {
    this.setLoading(true);

    return this.http.post<ApiResponse<T>>(this.baseUrl, item)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  update(id: number, item: Partial<T>): Observable<T> {
    this.setLoading(true);

    return this.http.put<ApiResponse<T>>(`${this.baseUrl}/${id}`, item)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  delete(id: number): Observable<boolean> {
    this.setLoading(true);

    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  bulkCreate(items: Partial<T>[]): Observable<T[]> {
    this.setLoading(true);

    return this.http.post<ApiResponse<T[]>>(`${this.baseUrl}/bulk`, items)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  bulkUpdate(items: Partial<T>[]): Observable<T[]> {
    this.setLoading(true);

    return this.http.put<ApiResponse<T[]>>(`${this.baseUrl}/bulk`, items)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  bulkDelete(ids: number[]): Observable<number> {
    this.setLoading(true);

    return this.http.delete<ApiResponse<number>>(`${this.baseUrl}/bulk`, { body: ids })
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  exportToCsv(params?: { searchTerm?: string; sortBy?: string; sortDirection?: string }): Observable<string> {
    this.setLoading(true);

    let httpParams = new HttpParams();
    if (params?.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params?.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
    if (params?.sortDirection) httpParams = httpParams.set('sortDirection', params.sortDirection);

    return this.http.get(`${this.baseUrl}/export`, {
      params: httpParams,
      responseType: 'text'
    })
      .pipe(
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  exists(id: number): Observable<boolean> {
    this.setLoading(true);

    return this.http.get<ApiResponse<boolean>>(`${this.baseUrl}/${id}/exists`)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  getCount(searchTerm?: string): Observable<number> {
    this.setLoading(true);

    let httpParams = new HttpParams();
    if (searchTerm) httpParams = httpParams.set('searchTerm', searchTerm);

    return this.http.get<ApiResponse<number>>(`${this.baseUrl}/count`, { params: httpParams })
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  getByIds(ids: number[]): Observable<T[]> {
    this.setLoading(true);

    return this.http.post<ApiResponse<T[]>>(`${this.baseUrl}/by-ids`, ids)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  uploadFile(file: File, additionalData?: { [key: string]: any }): Observable<any> {
    this.setLoading(true);

    const formData = new FormData();
    formData.append('file', file);

    if (additionalData) {
      Object.keys(additionalData).forEach(key => {
        formData.append(key, additionalData[key]);
      });
    }

    return this.http.post<ApiResponse<any>>(`${this.baseUrl}/upload`, formData)
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  downloadFile(id: number, filename?: string): Observable<Blob> {
    this.setLoading(true);

    return this.http.get(`${this.baseUrl}/${id}/download`, { responseType: 'blob' })
      .pipe(
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  search(filters: { [key: string]: any }, params?: PaginationParams): Observable<PaginatedResponse<T>> {
    this.setLoading(true);

    let httpParams = new HttpParams();

    if (params) {
      if (params.page !== undefined) httpParams = httpParams.set('pageNumber', params.page.toString());
      if (params.pageSize !== undefined) httpParams = httpParams.set('pageSize', params.pageSize.toString());
      if (params.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
      if (params.sortDirection) httpParams = httpParams.set('sortDirection', params.sortDirection);
    }

    Object.keys(filters).forEach(key => {
      if (filters[key] !== null && filters[key] !== undefined && filters[key] !== '') {
        httpParams = httpParams.set(key, filters[key].toString());
      }
    });

    return this.http.get<ApiResponse<PaginatedResponse<T>>>(`${this.baseUrl}/search`, { params: httpParams })
      .pipe(
        map(response => response.data),
        tap(() => this.setLoading(false)),
        catchError(this.handleError.bind(this))
      );
  }

  clearError(): void {
    this.errorSubject.next(null);
  }

  private setLoading(loading: boolean): void {
    this.loadingSubject.next(loading);
  }

  protected handleError(error: any): Observable<never> {
    this.setLoading(false);

    let errorMessage = 'An error occurred';

    if (error.error?.message) {
      errorMessage = error.error.message;
    } else if (error.message) {
      errorMessage = error.message;
    } else if (error.status === 0) {
      errorMessage = 'Unable to connect to server';
    } else if (error.status === 404) {
      errorMessage = 'Resource not found';
    } else if (error.status === 403) {
      errorMessage = 'Access denied';
    } else if (error.status === 401) {
      errorMessage = 'Unauthorized access';
    } else if (error.status >= 500) {
      errorMessage = 'Server error occurred';
    }

    this.errorSubject.next(errorMessage);
    return throwError(() => error);
  }
}

export function createCrudService<T extends CrudEntity>(
  http: HttpClient,
  endpoint: string
): GenericCrudService<T> {
  return new GenericCrudService<T>(http, endpoint);
}

export function CrudService(endpoint: string) {
  return function <T extends CrudEntity>(target: new (...args: any[]) => T) {
    return target;
  };
}

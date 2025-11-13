import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError, of } from 'rxjs';
import { map, catchError, tap, retry, timeout } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface ApiConfig {
  baseUrl?: string;
  timeout?: number;
  retryAttempts?: number;
  defaultHeaders?: { [key: string]: string };
}

export interface ApiRequest {
  method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';
  url: string;
  body?: any;
  params?: { [key: string]: any };
  headers?: { [key: string]: string };
  timeout?: number;
  retry?: number;
}

export interface ApiResponse<T = any> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
  statusCode?: number;
  timestamp?: string;
}

export interface ApiError {
  message: string;
  status: number;
  statusText: string;
  errors?: string[];
  timestamp: string;
}

@Injectable({
  providedIn: 'root'
})
export class GenericApiService {
  private readonly baseUrl: string;
  private readonly defaultTimeout: number;
  private readonly defaultRetryAttempts: number;
  private readonly defaultHeaders: { [key: string]: string };

  private loadingSubject = new BehaviorSubject<boolean>(false);
  private errorSubject = new BehaviorSubject<ApiError | null>(null);
  private requestCountSubject = new BehaviorSubject<number>(0);

  public loading$ = this.loadingSubject.asObservable();
  public error$ = this.errorSubject.asObservable();
  public requestCount$ = this.requestCountSubject.asObservable();

  constructor(
    private http: HttpClient,
    config?: ApiConfig
  ) {
    this.baseUrl = config?.baseUrl || environment.apiUrl;
    this.defaultTimeout = config?.timeout || 30000; // 30 seconds
    this.defaultRetryAttempts = config?.retryAttempts || 3;
    this.defaultHeaders = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      ...config?.defaultHeaders
    };
  }

  request<T = any>(request: ApiRequest): Observable<ApiResponse<T>> {
    this.setLoading(true);
    this.incrementRequestCount();

    const url = this.buildUrl(request.url);
    const options = this.buildRequestOptions(request);

    return this.http.request<ApiResponse<T>>(request.method, url, options)
      .pipe(
        timeout(request.timeout || this.defaultTimeout),
        retry(request.retry || this.defaultRetryAttempts),
        map(response => this.handleSuccessResponse(response)),
        tap(() => {
          this.setLoading(false);
          this.decrementRequestCount();
        }),
        catchError(error => this.handleError(error, request))
      );
  }

  get<T = any>(url: string, params?: { [key: string]: any }, options?: Partial<ApiRequest>): Observable<ApiResponse<T>> {
    return this.request<T>({
      method: 'GET',
      url,
      params,
      ...options
    });
  }

  post<T = any>(url: string, body?: any, options?: Partial<ApiRequest>): Observable<ApiResponse<T>> {
    return this.request<T>({
      method: 'POST',
      url,
      body,
      ...options
    });
  }

  put<T = any>(url: string, body?: any, options?: Partial<ApiRequest>): Observable<ApiResponse<T>> {
    return this.request<T>({
      method: 'PUT',
      url,
      body,
      ...options
    });
  }

  patch<T = any>(url: string, body?: any, options?: Partial<ApiRequest>): Observable<ApiResponse<T>> {
    return this.request<T>({
      method: 'PATCH',
      url,
      body,
      ...options
    });
  }

  delete<T = any>(url: string, options?: Partial<ApiRequest>): Observable<ApiResponse<T>> {
    return this.request<T>({
      method: 'DELETE',
      url,
      ...options
    });
  }

  uploadFile<T = any>(
    url: string, 
    file: File, 
    additionalData?: { [key: string]: any },
    options?: Partial<ApiRequest>
  ): Observable<ApiResponse<T>> {
    this.setLoading(true);
    this.incrementRequestCount();

    const formData = new FormData();
    formData.append('file', file);
    
    if (additionalData) {
      Object.keys(additionalData).forEach(key => {
        formData.append(key, additionalData[key]);
      });
    }

    const fullUrl = this.buildUrl(url);
    const headers = new HttpHeaders({
      ...this.defaultHeaders
    });
    delete headers['Content-Type'];

    return this.http.post<ApiResponse<T>>(fullUrl, formData, { headers })
      .pipe(
        timeout(options?.timeout || this.defaultTimeout),
        retry(options?.retry || this.defaultRetryAttempts),
        map(response => this.handleSuccessResponse(response)),
        tap(() => {
          this.setLoading(false);
          this.decrementRequestCount();
        }),
        catchError(error => this.handleError(error, { method: 'POST', url, ...options }))
      );
  }

  downloadFile(url: string, filename?: string, options?: Partial<ApiRequest>): Observable<Blob> {
    this.setLoading(true);
    this.incrementRequestCount();

    const fullUrl = this.buildUrl(url);
    const requestOptions = this.buildRequestOptions({ method: 'GET', url, ...options });

    return this.http.get(fullUrl, { ...requestOptions, responseType: 'blob' })
      .pipe(
        timeout(options?.timeout || this.defaultTimeout),
        retry(options?.retry || this.defaultRetryAttempts),
        tap(() => {
          this.setLoading(false);
          this.decrementRequestCount();
        }),
        catchError(error => this.handleError(error, { method: 'GET', url, ...options }))
      );
  }

  batch<T = any>(requests: ApiRequest[]): Observable<ApiResponse<T>[]> {
    this.setLoading(true);
    this.incrementRequestCount();

    const batchRequests = requests.map(req => this.request<T>(req));
    
    return new Observable(observer => {
      let completed = 0;
      const results: ApiResponse<T>[] = [];
      const errors: any[] = [];

      batchRequests.forEach((request, index) => {
        request.subscribe({
          next: (response) => {
            results[index] = response;
            completed++;
            if (completed === requests.length) {
              this.setLoading(false);
              this.decrementRequestCount();
              observer.next(results);
              observer.complete();
            }
          },
          error: (error) => {
            errors[index] = error;
            completed++;
            if (completed === requests.length) {
              this.setLoading(false);
              this.decrementRequestCount();
              observer.error(errors);
            }
          }
        });
      });
    });
  }

  healthCheck(): Observable<ApiResponse<{ status: string; timestamp: string }>> {
    return this.get<{ status: string; timestamp: string }>('/health');
  }

  clearError(): void {
    this.errorSubject.next(null);
  }

  get isLoading(): boolean {
    return this.loadingSubject.value;
  }

  get currentError(): ApiError | null {
    return this.errorSubject.value;
  }

  get currentRequestCount(): number {
    return this.requestCountSubject.value;
  }

  private buildUrl(url: string): string {
    if (url.startsWith('http')) {
      return url;
    }
    return `${this.baseUrl}${url.startsWith('/') ? url : '/' + url}`;
  }

  private buildRequestOptions(request: ApiRequest): any {
    const options: any = {
      headers: new HttpHeaders({
        ...this.defaultHeaders,
        ...request.headers
      })
    };

    if (request.params) {
      let httpParams = new HttpParams();
      Object.keys(request.params).forEach(key => {
        if (request.params![key] !== null && request.params![key] !== undefined) {
          httpParams = httpParams.set(key, request.params![key].toString());
        }
      });
      options.params = httpParams;
    }

    if (request.body) {
      options.body = request.body;
    }

    return options;
  }

  private handleSuccessResponse<T>(response: ApiResponse<T>): ApiResponse<T> {
    this.clearError();
    return response;
  }

  private handleError(error: HttpErrorResponse, request: ApiRequest): Observable<never> {
    this.setLoading(false);
    this.decrementRequestCount();

    const apiError: ApiError = {
      message: this.getErrorMessage(error),
      status: error.status,
      statusText: error.statusText,
      errors: this.getErrorDetails(error),
      timestamp: new Date().toISOString()
    };

    this.errorSubject.next(apiError);

    return throwError(() => apiError);
  }

  private getErrorMessage(error: HttpErrorResponse): string {
    if (error.error?.message) {
      return error.error.message;
    }
    if (error.message) {
      return error.message;
    }
    if (error.status === 0) {
      return 'Unable to connect to server. Please check your internet connection.';
    }
    if (error.status === 404) {
      return 'The requested resource was not found.';
    }
    if (error.status === 403) {
      return 'Access denied. You do not have permission to perform this action.';
    }
    if (error.status === 401) {
      return 'Unauthorized access. Please log in again.';
    }
    if (error.status >= 500) {
      return 'Server error occurred. Please try again later.';
    }
    return 'An unexpected error occurred.';
  }

  private getErrorDetails(error: HttpErrorResponse): string[] {
    if (error.error?.errors && Array.isArray(error.error.errors)) {
      return error.error.errors;
    }
    if (error.error?.message && typeof error.error.message === 'string') {
      return [error.error.message];
    }
    return [];
  }

  private setLoading(loading: boolean): void {
    this.loadingSubject.next(loading);
  }

  private incrementRequestCount(): void {
    this.requestCountSubject.next(this.requestCountSubject.value + 1);
  }

  private decrementRequestCount(): void {
    const current = this.requestCountSubject.value;
    this.requestCountSubject.next(Math.max(0, current - 1));
  }
}

// API Service Factory
export class ApiServiceFactory {
  static create(config?: ApiConfig): GenericApiService {
    return new GenericApiService(null as any, config);
  }
}

// Decorator for API endpoints
export function ApiEndpoint(endpoint: string) {
  return function (target: any, propertyKey: string, descriptor: PropertyDescriptor) {
    const originalMethod = descriptor.value;
    
    descriptor.value = function (...args: any[]) {
      const apiService = this.apiService || this.http;
      const url = endpoint.startsWith('/') ? endpoint : `/${endpoint}`;
      
      return originalMethod.apply(this, [url, ...args]);
    };
    
    return descriptor;
  };
}

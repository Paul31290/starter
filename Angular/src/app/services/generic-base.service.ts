import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseCrudServiceClass } from './base-crud.service';
import { PagedList } from '../common/paged-list.model';
import { PageSelectorModel } from '../common/page-selector.model';
import { environment } from '../../environments/environment';

@Injectable()
export abstract class GenericBaseService<T> extends BaseCrudServiceClass<T> {
  protected abstract apiUrl: string;

  constructor(protected http: HttpClient) {
    super();
  }

  getAll(): Observable<T[]> {
    return this.http.get<T[]>(this.apiUrl);
  }

  getPaged(pageSelector: PageSelectorModel): Observable<PagedList<T>> {
    let params = new HttpParams()
      .set('pageNumber', pageSelector.pageNumber.toString())
      .set('pageSize', pageSelector.pageSize.toString());

    if (pageSelector.sortBy) {
      params = params.set('sortBy', pageSelector.sortBy);
    }
    if (pageSelector.sortDirection) {
      params = params.set('sortDirection', pageSelector.sortDirection);
    }
    if (pageSelector.searchTerm) {
      params = params.set('searchTerm', pageSelector.searchTerm);
    }

    return this.http.get<PagedList<T>>(`${this.apiUrl}/paged`, { params });
  }

  getById(id: number): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${id}`);
  }

  create(item: T): Observable<T> {
    return this.http.post<T>(this.apiUrl, item);
  }

  update(id: number, item: T): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${id}`, item);
  }

  delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }

  exportCsv(searchTerm?: string, sortBy?: string, sortDirection?: string): Observable<Blob> {
    let params = new HttpParams();

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    if (sortBy) {
      params = params.set('sortBy', sortBy);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get(`${this.apiUrl}/export/csv`, {
      params,
      responseType: 'blob'
    });
  }
} 
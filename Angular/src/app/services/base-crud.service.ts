import { Observable } from 'rxjs';
import { PagedList } from '../common/paged-list.model';
import { PageSelectorModel } from '../common/page-selector.model';

export interface BaseCrudService<T> {
  getAll(): Observable<T[]>;
  getPaged(pageSelector: PageSelectorModel): Observable<PagedList<T>>;
  getById(id: number): Observable<T>;
  create(item: T): Observable<T>;
  update(id: number, item: T): Observable<T>;
  delete(id: number): Observable<boolean>;
  exportCsv(searchTerm?: string, sortBy?: string, sortDirection?: string): Observable<Blob>;
}

export abstract class BaseCrudServiceClass<T> implements BaseCrudService<T> {
  abstract getAll(): Observable<T[]>;
  abstract getPaged(pageSelector: PageSelectorModel): Observable<PagedList<T>>;
  abstract getById(id: number): Observable<T>;
  abstract create(item: T): Observable<T>;
  abstract update(id: number, item: T): Observable<T>;
  abstract delete(id: number): Observable<boolean>;
  abstract exportCsv(searchTerm?: string, sortBy?: string, sortDirection?: string): Observable<Blob>;
} 
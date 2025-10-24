import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { FieldOption } from '../common/field-config.model';

export interface OptionServiceConfig {
  endpoint: string;
  valueField: string;
  labelField: string;
  queryParams?: { [key: string]: any };
}

@Injectable()
export abstract class BaseOptionService {
  protected apiUrl = environment.apiUrl;

  constructor(protected http: HttpClient) {}

  /**
   * Get options for a specific entity type
   */
  protected getOptions(config: OptionServiceConfig): Observable<FieldOption[]> {
    let url = `${this.apiUrl}/api/${config.endpoint}`;
    
    if (config.queryParams) {
      const params = new URLSearchParams();
      Object.keys(config.queryParams).forEach(key => {
        if (config.queryParams![key] !== null && config.queryParams![key] !== undefined) {
          params.append(key, config.queryParams![key].toString());
        }
      });
      if (params.toString()) {
        url += `?${params.toString()}`;
      }
    }

    return this.http.get<any[]>(url).pipe(
      map(items => items.map(item => ({
        value: { id: item.id, name: item[config.labelField] },
        label: item[config.labelField]
      }))),
      catchError(error => {
        return of([]);
      })
    );
  }

  /**
   * Abstract method that each feature must implement to provide its specific options
   */
  abstract getOptionsForField(fieldKey: string): Observable<FieldOption[]>;
} 
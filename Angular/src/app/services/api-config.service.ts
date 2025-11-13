import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  
  constructor(private translateService: TranslateService) { }

  getEnvironmentInfo(): any {
    return {
      production: environment.production,
      baseUrl: environment.apiUrl,
      apiVersion: 'v1',
      appName: this.translateService.instant('COMMON.APP_NAME'),
      version: '1.0.0'
    };
  }

  getApiUrl(endpoint: string): string {
    return `${environment.apiUrl}/${endpoint}`;
  }

  getBaseUrl(): string {
    return environment.apiUrl;
  }
} 
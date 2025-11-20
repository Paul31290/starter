import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { GenericCrudService, PaginationParams, PaginatedResponse, ApiResponse } from './generic-crud.service';
import { User, NewUser} from '../models/user.model';
import { AuthUser} from '../models/auth.model';


@Injectable({
  providedIn: 'root'
})
export class SettingsService extends GenericCrudService<User> {

  constructor(http: HttpClient) {
    super(http, 'Settings');
  }

  ChangeUserName(userId: number, newUserNameRequest: NewUser): Observable<User> {
    return this.http.put<ApiResponse<User>>(`${this.baseUrl}/${userId}/change-username`, newUserNameRequest)
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  changeProfilePicture(userId: number, newProfilePictureRequest: NewUser): Observable<User> {
    return this.http.put<ApiResponse<User>>(`${this.baseUrl}/${userId}/change-profile-picture`, newProfilePictureRequest )
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }

  getProfilePicture(userId: number): Observable<string>{
    return this.http.get(`${this.baseUrl}/${userId}/get-profile-picture`, { responseType: 'text' }) 
      .pipe(
        map(response => response),
        catchError(this.handleError.bind(this))
      );
  }

  changeStatus(userId: number, newStatus: boolean): Observable<User> {
    return this.http.put<ApiResponse<User>>(`${this.baseUrl}/${userId}/change-status`, {
      newStatus
    })
      .pipe(
        map(response => response.data),
        catchError(this.handleError.bind(this))
      );
  }
}
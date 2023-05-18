import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ILoginInfo } from '../account/models/ILoginInfo';
import { environment } from 'src/environments/environment';
import { IRegister } from '../account/models/IRegister';
import { IForgotPassword } from '../account/models/IForgotPassword';
import { IResetPassword } from '../account/models/IResetPassword';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  apiUrl: string = environment.apiUrl;

  constructor(
    private http: HttpClient,) {
  }

  setToken(token: string | null): void {
    localStorage.setItem('token', token != null ? token : '');
  }

  getToken() {
    return localStorage.getItem('token')?.toString();
  }

  setEmail(email: string | null): void {
    localStorage.setItem('email', email != null ? email : '');
  }

  getEmail() {
    return localStorage.getItem('email')?.toString();
  }

  removeToken() {
    localStorage.removeItem('token');
  }

  setUserRoles(roles: string[]): void {
    localStorage.setItem('userRoles', JSON.stringify(roles));
  }

  getUserRoles(): string[] {
    const rolesJson = localStorage.getItem('userRoles');
    return rolesJson ? JSON.parse(rolesJson) : [];
  }

  register(register: IRegister): Observable<any> {
    return this.http
      .post(this.apiUrl + "Account/register ", register)
      .pipe(catchError(this.handleError));
  }

  resetPassword(resetPassword: IResetPassword): Observable<any> {
    console.log(resetPassword);
    return this.http
      .post(this.apiUrl + "Account/reset-password ", resetPassword)
      .pipe(catchError(this.handleError));
  }


  login(loginInfo: ILoginInfo): Observable<any> {

    return this.http
      .post(this.apiUrl + "Account/authenticate", loginInfo)
      .pipe(catchError(this.handleError));
  }

  forgotPassword(forgotPassword: IForgotPassword): Observable<any> {
    return this.http
      .post(this.apiUrl + "Account/forgot-password", forgotPassword)
      .pipe(catchError(this.handleError));
  }



  handleError(error: HttpErrorResponse) {
    const status: number = error.status;

    let errorMessage: string = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = ` client-side error` + error?.error?.message;
    } else {
      errorMessage = `Error Code: ${error.status} Message:${error?.error?.Message}`;
    }
    return throwError(() => errorMessage);
  }
}

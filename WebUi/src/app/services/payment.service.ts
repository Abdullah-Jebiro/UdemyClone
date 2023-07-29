import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PaymentDto } from '../checkout/models/PaymentDto';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  apiUrl: string = environment.apiUrl;

  constructor(
    private http: HttpClient) {

  }

  charge(paymentDto: PaymentDto): Observable<any> {
    return this.http
      .post(this.apiUrl + "Payment", paymentDto)
      .pipe(catchError(this.handleError));
  }

  handleError(error: HttpErrorResponse) {
    const status: number = error.status;

    let errorMessage: string = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = ` client-side error` + error?.error?.message;
    } 
    else {
      errorMessage = `Error Code: ${error.status == undefined ? 500 : error.status}
      Message : ${error?.error?.Message == undefined ? error?.message : error?.error?.Message}`;
    }
    console.log(errorMessage); 
    return throwError(() => errorMessage);
  }

}

import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CartItemDto } from '../cart/models/CartItemDto';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getCountItems = this.http.get<number>(this.apiUrl + "Carts/Count").pipe(
    tap((number) => console.log(number)),
    catchError(this.handleError)
  );

  getItems = this.http.get<CartItemDto[]>(this.apiUrl + "Carts").pipe(
    tap((number) => console.log(number)),
    catchError(this.handleError)
  );

  addToCart(itemId: number): Observable<any> {
    return this.http
      .post(this.apiUrl + "Carts?courseId=" + itemId, null)
      .pipe(catchError(this.handleError));
  }

  RemoveItem(itemId: number): Observable<any> {
    return this.http.delete(this.apiUrl + "Carts/" + itemId).pipe(
      tap((data) => console.log(data)),
      catchError(this.handleError)
    );
  }


  handleError(error: HttpErrorResponse) {
    let errorMessage: string = '';
    if (error.error instanceof ErrorEvent) {
      // client-side error
      errorMessage = ` client-side error` + error.error.message;
    } else {
      // server-side error
      errorMessage = `server-side error ${error.status} Message:${error.message}`;
    }
    return throwError(() => errorMessage);
  }
}

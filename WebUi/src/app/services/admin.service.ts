import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ILanguage } from '../shared/models/ILanguage';
import { ILevel } from '../shared/models/ILevel';
import { IInstructors } from '../shared/models/IInstructors';

@Injectable({
  providedIn: 'root'
})
export class AdminService {


  apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }


  Levels = this.http.get<ILevel[]>(this.apiUrl + "Levels").pipe(
    tap((data) => console.log(JSON.stringify(data))),
    catchError(this.handleError)
  );
  Languages = this.http.get<ILanguage[]>(this.apiUrl + "Languages").pipe(
    tap((data) => console.log(JSON.stringify(data))),
    catchError(this.handleError)
  );

  Instructors = this.http.get<IInstructors[]>(this.apiUrl + "Instructors").pipe(
    tap((data) => console.log(JSON.stringify(data))),
    catchError(this.handleError)
  );



  handleError(error: HttpErrorResponse) {
    const status: number = error.status;

    let errorMessage: string = '';
    if (error.error instanceof ErrorEvent) {

      errorMessage = ` client-side error` + error?.error?.message;
    } else {
      errorMessage = `Error Code: ${error.status} Message:${error?.message}`;
    }
    return throwError(() => errorMessage);
  }
}

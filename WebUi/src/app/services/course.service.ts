import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ICourse } from '../home/models/ICourse';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { ICategory } from '../home/models/ICategory';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { IVideoDto } from '../home/models/IVideoDto';
import { IVideoInfoDto } from '../instructor/models/IVideoInfoDto';
import { ICoursesWithDetailDto } from '../home/models/ICoursesWithDetailDto';
import { ICoursesWithStatusDto } from '../home/models/ICoursesWithStatusDto';


@Injectable({
  providedIn: 'root'
})
export class CourseService {

  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }


  getCourse(courseId: number): Observable<ICoursesWithDetailDto> {
    return this.http.get<ICoursesWithDetailDto>(this.apiUrl + "Courses/" + courseId).pipe(
      tap((data) => console.log(JSON.stringify(data))),
      catchError(this.handleError)
    );
  }

  getCourses(pageNumber: number, pageSize: number, categoryId?: number, languageId?: number, InstructorId?: number,

    levelId?: number, name?: string, onlyUserCourses?: boolean, maxPrice?: number, minPrice?: number): Observable<HttpResponse<ICoursesWithStatusDto[]>> {


    let url = `${this.apiUrl}Courses?pageNumber=${pageNumber}&pageSize=${pageSize}`;

    if (categoryId) {
      url += `&categoryId=${categoryId}`;
    }

    if (languageId) {
      url += `&languageId=${languageId}`;
    }
    if (levelId) {
      url += `&levelId=${levelId}`;
    }
    if (InstructorId) {
      url += `&instructorId=${InstructorId}`;
    }
    if (name) {
      url += `&name=${name}`;
    }
    if (onlyUserCourses) {
      url += `&onlyUserCourses=${true}`;
    }
    if (maxPrice) {
      url += `&maxPrice=${maxPrice}`;
    }
    if (minPrice) {
      url += `&minPrice=${minPrice}`;
    }


    const httpOptions = {
      withCredentials: true
    };
    return this.http.get<ICoursesWithStatusDto[]>(url, { observe: 'response', ...httpOptions }).pipe(
      tap(response => {
        console.log('paginationData : ' + response.headers.get('x-pagination'));
      }),
      catchError(this.handleError)
    );
  }



  getVideos(courseId: number): Observable<IVideoDto[]> {
    console.log(courseId);
    return this.http.get<IVideoDto[]>(this.apiUrl + `Courses/${courseId}/Videos`).pipe(
      tap((data) => console.log(JSON.stringify(data))),
      catchError(this.handleError)
    );
  }

  getInfoVideos(courseId: number): Observable<IVideoInfoDto[]> {
    console.log(courseId);
    return this.http.get<IVideoInfoDto[]>(this.apiUrl + `Courses/${courseId}/Videos/InfoVideos`).pipe(
      tap((data) => console.log(JSON.stringify(data))),
      catchError(this.handleError)
    );
  }

  getCategories(minCourseCount: number = 1): Observable<ICategory[]> {
    return this.http.get<ICategory[]>(this.apiUrl + "Categories?minCourseCount=" + minCourseCount).pipe(
      tap((data) => console.table(JSON.stringify(data)),
        catchError(this.handleError)
      ))
  }

  handleError(error: HttpErrorResponse) {
    let errorMessage: string = '';
    if (error.error instanceof ErrorEvent) {
      // client-side error
      errorMessage = ` client-side error` + error.error.message;
    } else {
      errorMessage = `Error Code: ${error.status == undefined ? 500 : error.status}
      Message : ${error?.error?.Message == undefined ? error?.message : error?.error?.Message}`;
    }
    console.log(errorMessage); 
    return throwError(() => errorMessage);
  }
}

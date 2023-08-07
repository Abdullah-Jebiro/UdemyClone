import { HttpClient, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Injectable, OnDestroy, OnInit } from '@angular/core';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ICourseForCreateDto } from '../instructor/models/ICourseForCreateDto';
import { ICourseForInstructorDto } from '../instructor/models/ICourseForInstructorDto';
import { IUserForUpdateDto } from '../instructor/models/IInstructorDto';
import { IVideoForEditDto } from '../instructor/models/IVideoForEditDto';
import { IVideoDto } from '../home/models/IVideoDto';

@Injectable({
  providedIn: 'root',
})
export class InstructorService {
  apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient, private router: Router) { }

  updateInfo(dto: IUserForUpdateDto): Observable<any> {
    return this.http
      .put(this.apiUrl + 'Account/update-info', dto)
      .pipe(catchError(this.handleError));
  }

  getInfo(): Observable<any> {
    return this.http
      .get(this.apiUrl + 'Account/get-info')
      .pipe(catchError(this.handleError));
  }

  createCourse(Course: ICourseForCreateDto): Observable<any> {
    console.log(Course);
    return this.http
      .post(this.apiUrl + 'Courses', Course)
      .pipe(catchError(this.handleError));
  }
  updateCourse(Course: ICourseForCreateDto, courseId: number): Observable<any> {
    return this.http
      .put(this.apiUrl + 'Courses/' + courseId, Course)
      .pipe(catchError(this.handleError));
  }



  getCourses(pageNumber: number, pageSize: number): Observable<HttpResponse<ICourseForInstructorDto[]>> {
    return this.http.get<ICourseForInstructorDto[]>
      (this.apiUrl + 'Courses/Instructor?pageNumber=' + pageNumber + '&pageSize=' + pageSize,
        { observe: 'response' }).pipe(
          tap((data) => console.table(data)),
          catchError(this.handleError));
  }

  getCourse(courseId: number): Observable<ICourseForCreateDto> {
    return this.http.get<ICourseForCreateDto>(this.apiUrl + 'Courses/Instructor/' + courseId).pipe(
      tap((data) => console.table(data)),
      catchError(this.handleError));
  }

  getVideos(courseId: number): Observable<IVideoDto[]> {
    console.log(courseId);
    return this.http.get<any[]>(this.apiUrl + `Courses/${courseId}/Videos`).pipe(
      tap((data) => console.log(JSON.stringify(data))),
      catchError(this.handleError)
    );
  }

  createVideo(video: IVideoForEditDto): Observable<any> {
    return this.http.post<IVideoForEditDto[]>(this.apiUrl + `Courses/${video.courseId}/Videos`, video).pipe(
      tap((data) => console.log(JSON.stringify(data))),
      catchError(this.handleError)
    );
  }

  getVideo(courseId: number, videoId: number): Observable<IVideoForEditDto> {
    return this.http.get<IVideoForEditDto>(this.apiUrl + `Courses/${courseId}/Videos/${videoId}`).pipe(
      tap((data) => console.table(data)),
      catchError(this.handleError));
  }



  updateVideo(video: IVideoForEditDto, courseId: number, videoId: number): Observable<any> {
    return this.http.put<IVideoForEditDto[]>(this.apiUrl + `Courses/${courseId}/Videos/${videoId}`, video).pipe(
      tap((data) => console.log(JSON.stringify(data))),
      catchError(this.handleError)
    );
  }


  deleteVideo(courseId: number, videoId: number): Observable<any> {
    return this.http.delete(this.apiUrl + `Courses/${courseId}/Videos/${videoId}`).pipe(
      catchError(this.handleError)
    );
  }


  deleteCourse(courseId: number): Observable<any> {
    return this.http.delete(this.apiUrl + `Courses/${courseId}`).pipe(
      catchError(this.handleDeleteCourseError)
    );
  }

  handleDeleteCourseError(error: HttpErrorResponse) {
    return throwError(() => error);
  }

  handleError(error: HttpErrorResponse) {
    let errorMessage: string = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = ` client - side error` + error?.error?.message;
    } 
    else {
      errorMessage = `Error Code: ${error.status == undefined ? 500 : error.status}
      Message : ${error?.error?.Message == undefined ? error?.message : error?.error?.Message}`;
    }
    console.log(errorMessage); 
    return throwError(() => errorMessage);
  }
}

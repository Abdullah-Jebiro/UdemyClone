import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpEventType,
} from '@angular/common/http';
import { catchError, map } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { FileType } from '../shared/models/FileType';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})

export class FileUploadService {
  apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  /**
   * Uploads a file to the API endpoint.
   * @param file The file to be uploaded.
   * @param fileName The name of the file to be used on the server.
   * @param fileType  Specifies the appropriate endpoint for uploading the file Example: Files/UploadImage
   * @param directory The directory where the file will be stored on the server.
   * @returns An observable that emits progress events or the response body of the upload request.
   */
  uploadFile(
    file: File,
    fileName: string,
    fileType: FileType,
    directory: string
  ): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, fileName);

    return this.http
      .post(
        this.apiUrl + 'Files/Upload' + fileType + '?directory=' + directory, // API endpoint URL
        formData, // Request body
        { reportProgress: true, observe: 'events' } // Request options
      )
      .pipe(
        map((event) => {
          switch (event.type) {
            case HttpEventType.UploadProgress:
              let total = typeof event.total == 'number' ? event.total : 1;
              if (total > 134217728) {
                // Maximum size of 125 MB exceeded
                throw Error(
                  'Failed to read the application form. The multipart body length limit of 125 megabytes, has been exceeded.'
                );
              }
              const progress = Math.round((100 * event.loaded) / total);
              return { status: 'progress', message: progress };

            case HttpEventType.Response:
              return event.body;

            default:
              return `Unhandled event: ${event.type}`;
          }
        }),
        catchError(this.handleError)
      );
  }

  handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage: string = '';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Failed to upload file: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code : ${error.status == undefined ? 500 : error.status
        }
        Message : ${error?.error?.Message == undefined
          ? error?.message
          : error?.error?.Message
        }`;
    }
    return throwError(() => errorMessage);
  }
}

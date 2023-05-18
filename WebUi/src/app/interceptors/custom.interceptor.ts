import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserService } from '../services/user.service';


@Injectable()
export class CustomInterceptor implements HttpInterceptor {

  constructor(private userService: UserService) { }
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let modifiedRequest = request.clone({ setHeaders: { 'Authorization': `Bearer ${this.userService.getToken()} ` } })
    return next.handle(modifiedRequest);
  }
}

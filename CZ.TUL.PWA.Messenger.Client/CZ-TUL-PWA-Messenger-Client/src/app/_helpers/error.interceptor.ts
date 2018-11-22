import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

import { AuthenticationService } from '../_services';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthenticationService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (err.status === 401) {
                return this.authenticationService.refresh()
                        .pipe(
                            switchMap((res: string) => {
                                if (res) {
                                    const currentUser = JSON.parse(JSON.stringify(res));

                                    localStorage.setItem('currentUser', JSON.stringify(res));

                                    request = request.clone({
                                        setHeaders: {
                                            Authorization: `Bearer ${currentUser.token}`
                                        }
                                    });
                                }

                                return next.handle(request);
                            })
                        );
            } else {
                const error = err.error;
                return throwError(error);
            }
        }));
    }
}

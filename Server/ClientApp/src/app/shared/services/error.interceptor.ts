import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(
        private _injector: Injector,
    ) {}

    public intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(req)
            .pipe(
                catchError((error: HttpErrorResponse) => {
                    const toastrService = this._injector.get(ToastrService);
                    toastrService.error(this._getMessage(error));

                    return throwError(error);
                }),
            );
    }

    private _getMessage(error: HttpErrorResponse): string {
        switch (error.error) {
            case 'CONNECTION_ERROR':
                return 'Błąd połączenia. Spróbuj ponownie później.';
            default:
                return 'Wystąpił nieznany błąd.';
        }
    }
}

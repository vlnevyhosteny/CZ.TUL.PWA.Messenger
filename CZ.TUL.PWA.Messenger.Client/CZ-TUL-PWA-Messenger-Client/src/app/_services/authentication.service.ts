import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../../cz-tul-pwa-messenger-client/src/environments/environment';
import { ReplaySubject, throwError, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {

    private baseUrl = environment.messengerApi;

    constructor(private http: HttpClient) { }

    login(username: string, password: string) {
        return this.http.post<any>(`${this.baseUrl}/auth/login`, { username, password })
            .pipe(map(user => {
                if (user && user.token) {
                    localStorage.setItem('currentUser', JSON.stringify(user));
                }

                return user;
            }));
    }

    refresh(): Observable<any> {
        const Token = this.getAuthToken();
        const RefreshToken = this.getRefreshToken();

        const refreshResponse =  this.http.post<any>(`${this.baseUrl}/auth/refresh`, { Token, RefreshToken });

        const refreshSubject = new ReplaySubject<any>(1);
        refreshSubject.subscribe((res: any) => {
            localStorage.setItem('currentUser', JSON.stringify(res));
        }, error => {
            return throwError(error);
        });

        refreshResponse.subscribe(refreshSubject);

        return refreshResponse;
    }

    logout() {
        localStorage.removeItem('currentUser');
    }

    getAuthToken(): string {
        const currentUser = localStorage.getItem('currentUser');

        if (currentUser === undefined) {
            return undefined;
        }

        const jsonObject = JSON.parse(currentUser);
        if (jsonObject.token === undefined) {
            return undefined;
        } else {
            return jsonObject.token;
        }
    }

    getRefreshToken(): string {
        const currentUser = localStorage.getItem('currentUser');

        if (currentUser === undefined) {
            return undefined;
        }

        const jsonObject = JSON.parse(currentUser);
        if (jsonObject.refreshToken === undefined) {
            return undefined;
        } else {
            return jsonObject.refreshToken;
        }
    }
}

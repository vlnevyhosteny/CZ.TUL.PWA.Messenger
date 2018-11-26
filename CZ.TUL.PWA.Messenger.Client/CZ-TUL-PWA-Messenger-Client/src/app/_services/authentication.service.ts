import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../../cz-tul-pwa-messenger-client/src/environments/environment';
import { ReplaySubject, throwError, Observable } from 'rxjs';
import { User } from '../_models/user';

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

    refresh(): Observable<string> {
        const Token = this.getAuthToken();
        const RefreshToken = this.getRefreshToken();

        return this.http.post<any>(`${this.baseUrl}/auth/refresh`, { Token, RefreshToken })
                    .pipe(
                        map(res => {
                            if (res) {
                                localStorage.setItem('currentUser', JSON.stringify(res));
                            }

                            return res;
                        })
                    );
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

    getCurrentUser(): User {
        const currentUser = localStorage.getItem('currentUser');

        if (currentUser === undefined) {
            return undefined;
        }

        const jsonObject = JSON.parse(currentUser);
        if (jsonObject.user === undefined) {
            return undefined;
        } else {
            const user = new User();
            user.id = jsonObject.user.id;
            user.name = jsonObject.user.name;
            user.userName = jsonObject.user.userName;

            return user;
        }
    }
}

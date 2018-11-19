import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../../../cz-tul-pwa-messenger-client/src/environments/environment';

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

    logout() {
        localStorage.removeItem('currentUser');
    }

    getAuthToken(): string {
        const currentUser = localStorage.getItem('currentUser');

        if (currentUser === undefined) {
            return undefined;
        }

        const jsonObject = JSON.parse(currentUser);
        if (jsonObject.Token === undefined) {
            return undefined;
        } else {
            return jsonObject.Token;
        }
    }

    getRefreshToken(): string {
        const currentUser = localStorage.getItem('currentUser');

        if (currentUser === undefined) {
            return undefined;
        }

        const jsonObject = JSON.parse(currentUser);
        if (jsonObject.RefreshToken === undefined) {
            return undefined;
        } else {
            return jsonObject.RefreshToken;
        }
    }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { User } from '../_models';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class UserService {

    private baseUrl = environment.messengerApi;

    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<User[]>(`${config.apiUrl}/users`);
    }

    public registerUser(userName: string, password: string, name: string): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/auth/login`, { userName, password, name });
    }
}

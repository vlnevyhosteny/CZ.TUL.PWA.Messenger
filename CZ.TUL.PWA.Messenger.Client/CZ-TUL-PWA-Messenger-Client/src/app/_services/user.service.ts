import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { User } from '../_models';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';
import { SelectedUser } from '../_models/selectedUser';

@Injectable({ providedIn: 'root' })
export class UserService {

    private baseUrl = environment.messengerApi;

    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<User[]>(`${this.baseUrl}/Users`);
    }

    public registerUser(userName: string, password: string, name: string): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/Users`, { userName, password, name });
    }

    public getUserNameContains(contains: string) {
        return this.http.get<User[]>(`${this.baseUrl}/Users/UserNameContains/` + contains);
    }

    public getUserNameContainsLimited(contains: string, limit?: number, offset?: number) {
        const params = new HttpParams()
                        .set('limit', limit.toString())
                        .set('offset', offset.toString());

        return this.http.get<User[]>(`${this.baseUrl}/Users/UserNameContains/` + contains,
            { params: params });
    }
}

import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Conversation } from '../_models/conversation';
import { map, catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ConversationService {

    private baseUrl = environment.messengerApi + '/Conversations';

    constructor(private http: HttpClient) { }

    getConversations() {
        return this.http.get<Array<Conversation>>(this.baseUrl);
    }

    getConversation(conversationId) {
        return this.http.get<Conversation>(this.baseUrl + '/' + conversationId);
    }

}

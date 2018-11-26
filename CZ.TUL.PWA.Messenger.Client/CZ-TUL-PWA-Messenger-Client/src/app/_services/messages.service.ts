import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Message } from '../_models/message';

@Injectable({ providedIn: 'root' })
export class MessagesService {

    private baseUrl = environment.messengerApi + '/Messages';

    constructor(private http: HttpClient) { }

    getMessagesForConversation(conversationId: number) {
        return this.http.get<Array<Message>>(this.baseUrl + '/Conversation/' + conversationId);
    }

}

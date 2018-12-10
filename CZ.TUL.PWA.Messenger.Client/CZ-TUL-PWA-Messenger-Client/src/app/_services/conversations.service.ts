import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Conversation } from '../_models/conversation';
import { map, catchError } from 'rxjs/operators';
import { SelectedUser } from '../_models/selectedUser';
import { UserConversation } from '../_models/userConversation';
import { User } from '../_models/user';

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

    addUser(conversation: Conversation, user: User, isOwner: boolean) {
        const model = new UserConversation();
        model.conversationId = conversation.conversationId;
        model.userId = user.id;
        model.isOwner = isOwner;
        model.notRead = false;
        model.notReadCount = 0;

        return this.http.post(environment.messengerApi + '/UserConversations', model);
    }

    add(newConversationName: string) {
        const model = new Conversation();
        model.name = newConversationName;

        return this.http.post(this.baseUrl, model);
    }

}

import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { Message } from '../_models/message';
import { Conversation } from '../_models/conversation';
import { ConversationService } from './conversations.service';
import { AuthenticationService } from './authentication.service';
import { FlattenMessage } from '../_models/flattenMessage';
import { User } from '../_models';

@Injectable({ providedIn: 'root' })
export class MessengerHubService {
    private baseUrl = environment.messenger + '/chat';

    constructor(private http: HttpClient,
                private conversationService: ConversationService,
                private authenticationService: AuthenticationService) { }

    initializeHubConnection(): HubConnection {
        const accessToken = this.authenticationService.getAuthToken();

        const connection = new HubConnectionBuilder()
            .withUrl(this.baseUrl, { accessTokenFactory: () => accessToken })
            .build();

        return connection;
    }

    send(message: Message, hubConnection: HubConnection) {
        hubConnection.invoke('Send',
            {
                ConversationId: message.conversation.conversationId,
                UserId: message.owner.id,
                Content: message.content
            });
    }

    invokeConversationChange(conversation: Conversation, hubConnection: HubConnection) {
        hubConnection.invoke('ConversationChange',
        {
            ConversationId: conversation.conversationId
        });
    }
}

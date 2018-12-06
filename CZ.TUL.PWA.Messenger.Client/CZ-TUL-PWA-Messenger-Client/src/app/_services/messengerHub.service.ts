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

    receive(
        flattenMessage: FlattenMessage,
        selectedConversation: Conversation,
        selectedMessages: Message[],
        conversations: Conversation[]) {

        if (flattenMessage.conversationId === selectedConversation.conversationId) {
            const message = this.toMessage(flattenMessage, selectedConversation);

            selectedMessages.push(message);
        } else {
            // if (!conversations.some(c => c.conversationId === message.conversation.conversationId)) {
            //     conversations.push(message.conversation);
            // }

            // TODO: unread message
            // const conversation = conversations.filter(c => c.conversationId === message.conversation.conversationId)[0];
            // conversation
        }
    }

    private toMessage(flattenMessage: FlattenMessage, conversation: Conversation): Message {
        const message = new Message();
        message.content = flattenMessage.content;
        message.conversation = conversation;
        message.dataSent = flattenMessage.dataSent;
        message.messageId = flattenMessage.messageId;
        message.owner = new User();
        message.owner.id = flattenMessage.ownerId;
        message.owner.name = flattenMessage.name;
        message.owner.userName = flattenMessage.userName;

        return message;
    }
}

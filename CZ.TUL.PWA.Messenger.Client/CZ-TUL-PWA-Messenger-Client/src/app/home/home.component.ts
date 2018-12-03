import { Component, OnInit } from '@angular/core';
import { UserService } from '../_services';
import { Conversation } from '../_models/conversation';
import { ConversationService } from '../_services/conversations.service';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { AuthenticationService } from '../_services/authentication.service';
import { MessagesService } from '../_services/messages.service';

@Component({templateUrl: 'home.component.html'})
export class HomeComponent implements OnInit {
    conversations: Conversation[] = [];
    selectedConversation: Conversation = new Conversation();
    selectedMessages: Message[];
    user: User;
    sidebarCollapsed = false;

    constructor(private userService: UserService,
        private conversationService: ConversationService,
        private authenticationService: AuthenticationService,
        private messagesService: MessagesService) {}

    ngOnInit() {
        this.user = this.authenticationService.getCurrentUser();
        this.selectedConversation.name = 'new conversation';

        this.conversationService.getConversations()
            .subscribe((result: Array<Conversation>) => {
                this.conversations = result;

                if (this.conversations.length > 0) {
                    this.selectedConversation = this.conversations[0];
                }
            },
            (err) => console.error(err),
            () => this.getMessages());
    }

    getMessages() {
        if (this.selectedConversation.conversationId !== undefined) {
            this.messagesService
                .getMessagesForConversation(this.selectedConversation.conversationId)
                .subscribe((messages: Array<Message>) => {
                    this.selectedMessages = messages;
                });
        }
    }

    isCurrentUserOwnerOfMessage(message: Message) {
        if (message.owner.id === this.user.id) {
            return true;
        } else {
            return false;
        }
    }

    sidebarCollapse() {
        this.sidebarCollapsed = !this.sidebarCollapsed;
    }

    switchConversation(conversation: Conversation) {
        this.selectedConversation = conversation;

        this.getMessages();
    }
}

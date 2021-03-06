﻿import { Component, OnInit } from '@angular/core';
import { UserService } from '../_services';
import { Conversation } from '../_models/conversation';
import { ConversationService } from '../_services/conversations.service';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { AuthenticationService } from '../_services/authentication.service';
import { MessagesService } from '../_services/messages.service';
import { HubConnection } from '@aspnet/signalr';
import { MessengerHubService } from '../_services/messengerHub.service';
import { FlattenMessage } from '../_models/flattenMessage';
import { ModalService } from '../_services/modal.service';
import { SelectedUser } from '../_models/selectedUser';

@Component({templateUrl: 'home.component.html'})
export class HomeComponent implements OnInit {
    conversations: Conversation[] = [];
    selectedConversation: Conversation = new Conversation();
    selectedMessages: Message[];
    user: User;
    sidebarCollapsed = false;
    newMessage: string;
    hubConnection: HubConnection;
    suggestedAddresses: SelectedUser[] = [];
    newConversationName: string;

    constructor(private userService: UserService,
        private conversationService: ConversationService,
        private authenticationService: AuthenticationService,
        private messagesService: MessagesService,
        private messangerHubService: MessengerHubService,
        private modalService: ModalService) {}

    ngOnInit() {
        this.hubConnection = this.messangerHubService.initializeHubConnection();
        this.hubConnection.on('broadcastMessage', (message: FlattenMessage) => {
            this.receive(message);
        });
        this.hubConnection.on('broadcastConversation', (conversation: Conversation) => {
            this.updateConversations(conversation);
        });

        this.hubConnection.start();

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

        this.selectedConversation.unread = false;

        this.getMessages();
    }

    send() {
        if (this.newMessage !== undefined && this.newMessage !== '') {
            const message = new Message();
            message.content = this.newMessage;
            message.conversation = this.selectedConversation;
            message.owner = this.user;

            this.messangerHubService.send(message, this.hubConnection);

            this.clearNewMessage();
        }
    }

    clearNewMessage() {
        this.newMessage = '';
    }

    openAddAddresseModal() {
        this.modalService.openWithParams('add-addresse', [ this.selectedConversation ]);
    }

    closeAddAddresseModal() {
        this.modalService.close('add-addresse');
    }

    onAddresseInputChange(addresseValue: string) {
        if (addresseValue) {
            this.userService.getUserNameContainsLimited(addresseValue, 5, 0)
                            .subscribe((users: Array<SelectedUser>) => {
                                this.suggestedAddresses = [];

                                for (const user of users) {
                                    const selectedUser = new SelectedUser();
                                    selectedUser.id = user.id;
                                    selectedUser.name = user.name;
                                    selectedUser.selected = false;
                                    selectedUser.userName = user.userName;

                                    this.suggestedAddresses.push(selectedUser);
                                }
                            });
        }
    }

    async addAddressesToSelectedConversation() {
        const selectedUsers = this.suggestedAddresses.filter(x => x.selected !== false);

        if (selectedUsers.length > 0) {
            for (const selected of selectedUsers) {
                await this.conversationService.addUser(this.selectedConversation, selected, false).toPromise();
            }

            this.messangerHubService.invokeConversationChange(this.selectedConversation, this.hubConnection);
            this.closeAddAddresseModal();
        }
    }

    async addNewConversation() {
        await this.conversationService.add(this.newConversationName)
                                      .subscribe((conversation: Conversation) => {
                                            this.conversationService.addUser(conversation, this.user, true).toPromise();
                                            this.conversations.push(conversation);
                                            this.selectedConversation = conversation;
                                      });

        this.newConversationName = '';
    }

    // Nothing to be proud of. Should be separated in service.
    async receive(flattenMessage: FlattenMessage) {
        if (flattenMessage.conversationId === this.selectedConversation.conversationId) {
            const message = this.toMessage(flattenMessage, this.selectedConversation);

            this.selectedMessages.push(message);
        } else {
            if (!this.conversations.some(c => c.conversationId === flattenMessage.conversationId)) {
                const newConversation = await this.conversationService.getConversation(flattenMessage.conversationId)
                    .toPromise();
                this.conversations.push(newConversation);
            }

            const conversation = this.conversations.filter(c => c.conversationId === flattenMessage.conversationId)[0];
            conversation.unread = true;
        }
    }

    updateConversations(conversation: Conversation) {
        conversation.unread = true;

        this.conversations = this.conversations.filter(c => c.conversationId !== conversation.conversationId);

        this.conversations.push(conversation);
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

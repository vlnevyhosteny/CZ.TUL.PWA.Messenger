import { Component, OnInit } from '@angular/core';
import { UserService } from '../_services';
import { Conversation } from '../_models/conversation';
import { ConversationService } from '../_services/conversations.service';

@Component({templateUrl: 'home.component.html'})
export class HomeComponent implements OnInit {
    conversations: Conversation[] = [];

    constructor(private userService: UserService,
        private conversationService: ConversationService) {}

    ngOnInit() {
        this.conversationService.getConversationForUser('9e2b97e8-5c50-4ab4-a60f-709f16d96cfc')
            .subscribe((result: Array<Conversation>) => {
                this.conversations = result;
            });
    }
}

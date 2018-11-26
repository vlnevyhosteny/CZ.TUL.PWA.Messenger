import { User } from './user';
import { Conversation } from './conversation';

export class Message {
    messageId: number;
    owner: User;
    conversation: Conversation;
    content: string;
    dataSent: Date;
}

import { Component } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'CZ-TUL-PWA-Messenger-Client';

  public _hubConnection: HubConnection;
  nick = '';
  message = '';
  messages: string[] = [];

  ngOnInit() {
    this.nick = window.prompt('Your name:', 'Maskicz');

    this._hubConnection = new HubConnectionBuilder()
        .withUrl('http://localhost:5000/chat')
        .build();;
          
    this._hubConnection
      .start()
      .then(() => console.log('Connection started!'))
        .catch(err => console.log('Error while establishing connection :('));

    this._hubConnection.on('sendToAll', (nick: string, receivedMessage: string) => {
        const text = `${nick}: ${receivedMessage}`;
        this.messages.push(text);
    });
  }

  public sendMessage(): void {
    this._hubConnection
      .invoke('sendToAll', this.nick, this.message)
      .catch(err => console.error(err));
  }
}

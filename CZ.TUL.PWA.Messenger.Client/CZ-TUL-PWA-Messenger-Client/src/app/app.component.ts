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
  message = '';
  messages: string[] = [];

  public sendMessage(): void {
    this._hubConnection.invoke("SendToAll", this.message);
    this.message = "";
  }

  ngOnInit() {
    let builder = new HubConnectionBuilder()

    this._hubConnection = builder.withUrl('https://localhost:5001/chat').build();

    this._hubConnection.on('Send', (receivedMessage: string) => {
        this.messages.push(receivedMessage);
    });

    this._hubConnection.start();
  }
}

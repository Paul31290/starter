import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection | undefined;

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/notification') // Adjust port if needed
      .build();

    this.hubConnection
      .start()
      .then(() => {
      })
      .catch(err => {
      });
  }

  public addNotificationListener(callback: (message: string) => void) {
    this.hubConnection?.on('ReceiveMessage', callback);
  }

  public sendMessage(message: string) {
    this.hubConnection?.invoke('SendMessage', message);
  }
} 
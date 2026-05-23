import { Injectable, NgZone, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

export interface NewOpinionPayload {
  teleconsultoriaId: string;
  opinionId: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private connection?: signalR.HubConnection;
  readonly lastOpinionNotification = signal<NewOpinionPayload | null>(null);

  constructor(private auth: AuthService, private zone: NgZone) {}

  handleNewOpinion(payload: NewOpinionPayload) {
    this.zone.run(() => this.lastOpinionNotification.set(payload));
  }

  connect() {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        accessTokenFactory: () => this.auth.getToken() ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('NewOpinion', (payload: NewOpinionPayload) => {
      this.handleNewOpinion(payload);
    });

    this.connection.start().catch(err => console.error('SignalR error:', err));
  }

  disconnect() {
    this.connection?.stop();
    this.connection = undefined;
  }
}

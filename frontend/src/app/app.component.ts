import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { NotificationService } from './core/services/notification.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: '<router-outlet />'
})
export class AppComponent implements OnInit {
  constructor(private auth: AuthService, private notifications: NotificationService) {}

  ngOnInit() {
    if (this.auth.isLoggedIn()) {
      this.notifications.connect();
    }
  }
}

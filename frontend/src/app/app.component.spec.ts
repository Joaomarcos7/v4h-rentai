import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AppComponent } from './app.component';
import { AuthService } from './core/services/auth.service';
import { NotificationService } from './core/services/notification.service';

describe('AppComponent', () => {
  let authSpy: jasmine.SpyObj<AuthService>;
  let notifSpy: jasmine.SpyObj<NotificationService>;

  beforeEach(() => {
    authSpy = jasmine.createSpyObj('AuthService', ['isLoggedIn']);
    notifSpy = jasmine.createSpyObj('NotificationService', ['connect']);

    TestBed.configureTestingModule({
      imports: [AppComponent, RouterTestingModule, HttpClientTestingModule],
      providers: [
        { provide: AuthService, useValue: authSpy },
        { provide: NotificationService, useValue: notifSpy },
      ]
    });
  });

  it('should call connect() on init when user is already logged in', () => {
    authSpy.isLoggedIn.and.returnValue(true);
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    expect(notifSpy.connect).toHaveBeenCalledTimes(1);
  });

  it('should NOT call connect() on init when user is not logged in', () => {
    authSpy.isLoggedIn.and.returnValue(false);
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    expect(notifSpy.connect).not.toHaveBeenCalled();
  });
});

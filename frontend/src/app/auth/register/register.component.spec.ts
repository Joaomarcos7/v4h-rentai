import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { RegisterComponent } from './register.component';
import { environment } from '../../../environments/environment';

describe('RegisterComponent', () => {
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterComponent, HttpClientTestingModule, RouterTestingModule],
    }).compileComponents();
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('should send role as number even when DOM select coerces value to string', () => {
    const fixture = TestBed.createComponent(RegisterComponent);
    const component = fixture.componentInstance;

    component.name = 'Test User';
    component.email = 'test@test.com';
    component.password = 'secret123';
    (component as any).role = '2'; // simulates [value]="2" DOM string coercion via ngModel

    component.submit();

    const req = http.expectOne(`${environment.apiUrl}/auth/register`);
    expect(typeof req.request.body['role']).toBe('number');
    expect(req.request.body['role']).toBe(2);
    req.flush({});
  });
});

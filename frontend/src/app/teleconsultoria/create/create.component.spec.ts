import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { CreateComponent } from './create.component';
import { environment } from '../../../environments/environment';

describe('CreateComponent', () => {
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateComponent, HttpClientTestingModule, RouterTestingModule],
    }).compileComponents();
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('should send specialty as number (not string) in create request', () => {
    const fixture = TestBed.createComponent(CreateComponent);
    const component = fixture.componentInstance;

    component.form.patientName = 'Paciente Teste';
    component.form.birthDate = '1990-01-01';
    component.form.diagnosticHypothesis = 'Hipótese';
    component.form.clinicalHistory = 'Histórico';
    (component.form as any).specialty = '3'; // simulates [value] DOM string coercion

    component.submit();

    const req = http.expectOne(`${environment.apiUrl}/teleconsultorias`);
    expect(typeof req.request.body['specialty']).toBe('number');
    expect(req.request.body['specialty']).toBe(3);
    req.flush({ id: 'new-id' });
  });
});

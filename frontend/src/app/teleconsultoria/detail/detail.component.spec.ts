import { ApplicationRef } from '@angular/core';
import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { DetailComponent } from './detail.component';
import { NotificationService } from '../../core/services/notification.service';
import { environment } from '../../../environments/environment';

const TC_ID = 'tc-abc-123';

const mockTc = {
  id: TC_ID,
  patientName: 'João',
  birthDate: '1990-01-01',
  specialty: 'Cardiologia',
  diagnosticHypothesis: 'Hipótese',
  clinicalHistory: 'Histórico',
  status: 'EmAndamento',
  requesterName: 'Dr. Ana',
  createdAt: '2026-05-23T10:00:00Z',
  updatedAt: '2026-05-23T10:00:00Z',
  documents: [],
  opinions: []
};

describe('DetailComponent', () => {
  let http: HttpTestingController;
  let notificationService: NotificationService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailComponent, HttpClientTestingModule, RouterTestingModule],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: () => TC_ID } } }
        }
      ]
    }).compileComponents();
    http = TestBed.inject(HttpTestingController);
    notificationService = TestBed.inject(NotificationService);
  });

  afterEach(() => http.verify());

  function createAndLoad() {
    const fixture = TestBed.createComponent(DetailComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    const req = http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`);
    req.flush(mockTc);
    fixture.detectChanges();
    return { fixture, component };
  }

  it('should initialize newStatus as integer matching current tc status', () => {
    const { component } = createAndLoad();
    expect(component.newStatus).toBe(2);
  });

  it('should send status as number (not string) in updateStatus request', () => {
    const { component } = createAndLoad();

    (component as any).newStatus = '3';

    component.updateStatus();

    const putReq = http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}/status`);
    expect(typeof putReq.request.body['status']).toBe('number');
    expect(putReq.request.body['status']).toBe(3);
    putReq.flush({});

    const reloadReq = http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`);
    reloadReq.flush(mockTc);
  });

  it('should reload tc data when onNotification is called with matching teleconsultoria id', () => {
    const { component } = createAndLoad();

    component.onNotification({ teleconsultoriaId: TC_ID, opinionId: 'opinion-1' });

    const reloadReq = http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`);
    reloadReq.flush({ ...mockTc, opinions: [{ id: 'opinion-1', specialistName: 'Dr. X', content: 'Parecer', createdAt: '2026-05-23T11:00:00Z' }] });
  });

  it('should NOT reload when onNotification is called with a different teleconsultoria id', () => {
    createAndLoad();

    // Get a fresh component instance to call onNotification
    const fixture2 = TestBed.createComponent(DetailComponent);
    const component2 = fixture2.componentInstance;
    fixture2.detectChanges();
    // flush initial load for this instance
    http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`).flush(mockTc);

    component2.onNotification({ teleconsultoriaId: 'other-tc-id', opinionId: 'opinion-99' });

    http.expectNone(`${environment.apiUrl}/teleconsultorias/${TC_ID}`);
  });

  it('should NOT reload when onNotification is called with null', () => {
    const { component } = createAndLoad();

    component.onNotification(null);

    http.expectNone(`${environment.apiUrl}/teleconsultorias/${TC_ID}`);
  });
});

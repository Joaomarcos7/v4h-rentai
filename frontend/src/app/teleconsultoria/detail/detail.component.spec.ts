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

  it('should display opinions sorted ascending by createdAt (oldest first, newest at bottom)', () => {
    const fixture = TestBed.createComponent(DetailComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`).flush({
      ...mockTc,
      opinions: [
        { id: 'op-2', specialistName: 'Dr. X', content: 'Segundo', createdAt: '2026-05-23T12:00:00Z' },
        { id: 'op-1', specialistName: 'Dr. X', content: 'Primeiro', createdAt: '2026-05-23T10:00:00Z' },
      ]
    });
    fixture.detectChanges();
    const sorted = component.sortedOpinions;
    expect(sorted[0].id).toBe('op-1');
    expect(sorted[1].id).toBe('op-2');
  });

  it('should set newOpinionAlert when onNotification called with matching id', () => {
    const { component } = createAndLoad();
    expect(component.newOpinionAlert()).toBeFalse();

    component.onNotification({ teleconsultoriaId: TC_ID, opinionId: 'op-x' });

    expect(component.newOpinionAlert()).toBeTrue();
    // flush reload
    http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`).flush(mockTc);
  });

  it('should clear newOpinionAlert when dismissAlert() is called', () => {
    const { component } = createAndLoad();
    component.onNotification({ teleconsultoriaId: TC_ID, opinionId: 'op-x' });
    http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`).flush(mockTc);

    component.dismissAlert();

    expect(component.newOpinionAlert()).toBeFalse();
  });

  it('should reload when notification signal fires with matching id (effect path)', fakeAsync(() => {
    const { fixture } = createAndLoad();
    const appRef = TestBed.inject(ApplicationRef);

    notificationService.handleNewOpinion({ teleconsultoriaId: TC_ID, opinionId: 'op-99' });
    tick();
    appRef.tick();
    fixture.detectChanges();

    const reloadReq = http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`);
    reloadReq.flush(mockTc);
  }));
});

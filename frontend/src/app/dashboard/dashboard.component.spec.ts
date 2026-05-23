import { ApplicationRef } from '@angular/core';
import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { DashboardComponent } from './dashboard.component';
import { NotificationService } from '../core/services/notification.service';
import { environment } from '../../environments/environment';

const LIST_URL = `${environment.apiUrl}/teleconsultorias`;

const mockItems = [
  { id: 'tc-1', patientName: 'João', specialty: 'Cardiologia', status: 'Pendente', requesterName: 'Dr. Ana', createdAt: '2026-05-23T10:00:00Z' },
  { id: 'tc-2', patientName: 'Maria', specialty: 'Odontologia', status: 'EmAndamento', requesterName: 'Dr. Ana', createdAt: '2026-05-23T11:00:00Z' },
];

describe('DashboardComponent', () => {
  let http: HttpTestingController;
  let notificationService: NotificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [DashboardComponent, HttpClientTestingModule, RouterTestingModule],
    });
    http = TestBed.inject(HttpTestingController);
    notificationService = TestBed.inject(NotificationService);
  });

  afterEach(() => http.verify());

  function createAndLoad() {
    const fixture = TestBed.createComponent(DashboardComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    http.expectOne(LIST_URL).flush(mockItems);
    fixture.detectChanges();
    return { fixture, component };
  }

  it('should set pendingOpinionTcId when handleNewOpinion fires', fakeAsync(() => {
    const { fixture } = createAndLoad();
    const appRef = TestBed.inject(ApplicationRef);
    const component = fixture.componentInstance;
    expect(component.pendingOpinionTcId()).toBeNull();

    notificationService.handleNewOpinion({ teleconsultoriaId: 'tc-1', opinionId: 'op-1' });
    tick();
    appRef.tick();
    fixture.detectChanges();

    expect(component.pendingOpinionTcId()).toBe('tc-1');
    http.expectOne(LIST_URL).flush(mockItems);
    tick(4000); // flush showToast setTimeout
  }));

  it('should clear pendingOpinionTcId when dismissDashboardAlert() is called', fakeAsync(() => {
    const { fixture } = createAndLoad();
    const appRef = TestBed.inject(ApplicationRef);
    const component = fixture.componentInstance;

    notificationService.handleNewOpinion({ teleconsultoriaId: 'tc-1', opinionId: 'op-1' });
    tick();
    appRef.tick();
    fixture.detectChanges();
    http.expectOne(LIST_URL).flush(mockItems);
    tick(4000); // flush showToast setTimeout

    component.dismissDashboardAlert();

    expect(component.pendingOpinionTcId()).toBeNull();
  }));
});

import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { DetailComponent } from './detail.component';
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
    // status "EmAndamento" = 2
    expect(component.newStatus).toBe(2);
  });

  it('should send status as number (not string) in updateStatus request', () => {
    const { component } = createAndLoad();

    (component as any).newStatus = '3'; // simulates [value] DOM string coercion

    component.updateStatus();

    const putReq = http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}/status`);
    expect(typeof putReq.request.body['status']).toBe('number');
    expect(putReq.request.body['status']).toBe(3);
    putReq.flush({});

    // success handler calls load() → flush the reload GET
    const reloadReq = http.expectOne(`${environment.apiUrl}/teleconsultorias/${TC_ID}`);
    reloadReq.flush(mockTc);
  });
});

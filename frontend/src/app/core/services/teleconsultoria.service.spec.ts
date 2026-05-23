import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TeleconsultoriaService } from './teleconsultoria.service';
import { environment } from '../../../environments/environment';

describe('TeleconsultoriaService', () => {
  let service: TeleconsultoriaService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(TeleconsultoriaService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  describe('exportPdf', () => {
    it('should request PDF via HttpClient (not direct URL) so auth interceptor adds token', () => {
      const id = 'abc-123';
      const fakeBlob = new Blob(['%PDF'], { type: 'application/pdf' });
      let result: Blob | undefined;

      service.exportPdf(id).subscribe(blob => (result = blob));

      const req = http.expectOne(`${environment.apiUrl}/teleconsultorias/${id}/export/pdf`);
      expect(req.request.method).toBe('GET');
      expect(req.request.responseType).toBe('blob');
      req.flush(fakeBlob);

      expect(result).toBeInstanceOf(Blob);
    });
  });
});

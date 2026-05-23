import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { NgZone } from '@angular/core';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
  let service: NotificationService;
  let zone: NgZone;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
    });
    service = TestBed.inject(NotificationService);
    zone = TestBed.inject(NgZone);
  });

  it('should update lastOpinionNotification inside Angular zone so effects trigger', fakeAsync(() => {
    const payload = { teleconsultoriaId: 'tc-1', opinionId: 'op-1' };
    let isInsideZone = false;

    // Subscribe to signal change and check if we're inside zone
    const originalSet = service.lastOpinionNotification.set.bind(service.lastOpinionNotification);
    spyOn(service.lastOpinionNotification, 'set').and.callFake((val) => {
      isInsideZone = NgZone.isInAngularZone();
      originalSet(val);
    });

    // Simulate SignalR callback firing OUTSIDE zone (as it does in production)
    zone.runOutsideAngular(() => {
      (service as any).handleNewOpinion(payload);
    });

    tick();

    expect(isInsideZone).toBe(true);
    expect(service.lastOpinionNotification()).toEqual(payload);
  }));
});

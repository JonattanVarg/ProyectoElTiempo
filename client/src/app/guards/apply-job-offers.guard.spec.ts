import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { applyJobOffersGuard } from './apply-job-offers.guard';

describe('applyJobOffersGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => applyJobOffersGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});

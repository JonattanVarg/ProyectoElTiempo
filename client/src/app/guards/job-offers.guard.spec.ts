import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { jobOffersGuard } from './job-offers.guard';

describe('jobOffersGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => jobOffersGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});

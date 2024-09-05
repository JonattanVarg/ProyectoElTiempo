import { TestBed } from '@angular/core/testing';

import { ApplyJobOffersService } from './apply-job-offers.service';

describe('ApplyJobOffersService', () => {
  let service: ApplyJobOffersService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApplyJobOffersService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

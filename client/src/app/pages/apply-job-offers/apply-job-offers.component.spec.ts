import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplyJobOffersComponent } from './apply-job-offers.component';

describe('ApplyJobOffersComponent', () => {
  let component: ApplyJobOffersComponent;
  let fixture: ComponentFixture<ApplyJobOffersComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ApplyJobOffersComponent]
    });
    fixture = TestBed.createComponent(ApplyJobOffersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

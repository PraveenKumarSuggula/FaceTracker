import { TestBed } from '@angular/core/testing';

import { FaceRegistrationService } from './face-registration.service';

describe('FaceRegistrationService', () => {
  let service: FaceRegistrationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FaceRegistrationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

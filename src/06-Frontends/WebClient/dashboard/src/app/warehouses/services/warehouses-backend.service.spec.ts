import { TestBed } from '@angular/core/testing';

import { WarehousesBackendService } from './warehouses-backend.service';

describe('WarehousesBackendService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: WarehousesBackendService = TestBed.get(WarehousesBackendService);
    expect(service).toBeTruthy();
  });
});

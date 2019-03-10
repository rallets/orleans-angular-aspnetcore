import { TestBed } from '@angular/core/testing';

import { WarehousesStoreService } from './warehouses-store.service';

describe('WarehousesStoreService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: WarehousesStoreService = TestBed.get(WarehousesStoreService);
    expect(service).toBeTruthy();
  });
});

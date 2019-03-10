import { TestBed } from '@angular/core/testing';

import { ProductsBackendService } from './products-backend.service';

describe('ProductsBackendService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ProductsBackendService = TestBed.get(ProductsBackendService);
    expect(service).toBeTruthy();
  });
});

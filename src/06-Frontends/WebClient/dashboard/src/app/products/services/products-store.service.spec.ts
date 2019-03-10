import { TestBed } from '@angular/core/testing';

import { ProductsStoreService } from './products-store.service';

describe('ProductsStoreService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ProductsStoreService = TestBed.get(ProductsStoreService);
    expect(service).toBeTruthy();
  });
});

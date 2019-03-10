import { TestBed } from '@angular/core/testing';

import { OrdersStoreService } from './orders-store.service';

describe('OrdersStoreService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OrdersStoreService = TestBed.get(OrdersStoreService);
    expect(service).toBeTruthy();
  });
});

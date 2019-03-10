import { TestBed } from '@angular/core/testing';

import { OrdersBackendService } from './orders-backend.service';

describe('OrdersBackendService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OrdersBackendService = TestBed.get(OrdersBackendService);
    expect(service).toBeTruthy();
  });
});

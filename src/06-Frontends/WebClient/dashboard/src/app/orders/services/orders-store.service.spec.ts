import { TestBed } from '@angular/core/testing';

import { OrdersStoreService } from './orders-store.service';
import { sharedServiceImports } from 'src/app/testing/testing.shared';

describe('OrdersStoreService', () => {
	beforeEach(() => TestBed.configureTestingModule({
		imports: [sharedServiceImports],
	}));

	it('should be created', () => {
		const service: OrdersStoreService = TestBed.get(OrdersStoreService);
		expect(service).toBeTruthy();
	});
});

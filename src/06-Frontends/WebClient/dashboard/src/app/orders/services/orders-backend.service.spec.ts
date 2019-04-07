import { TestBed } from '@angular/core/testing';

import { OrdersBackendService } from './orders-backend.service';
import { sharedServiceImports } from 'src/app/testing/testing.shared';

describe('OrdersBackendService', () => {
	beforeEach(() => TestBed.configureTestingModule({
		imports: [sharedServiceImports],
	}));

	it('should be created', () => {
		const service: OrdersBackendService = TestBed.get(OrdersBackendService);
		expect(service).toBeTruthy();
	});
});

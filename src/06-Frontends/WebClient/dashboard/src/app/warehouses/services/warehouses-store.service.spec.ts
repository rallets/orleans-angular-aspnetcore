import { TestBed } from '@angular/core/testing';

import { WarehousesStoreService } from './warehouses-store.service';
import { sharedServiceImports } from 'src/app/testing/testing.shared';

describe('WarehousesStoreService', () => {
	beforeEach(() => TestBed.configureTestingModule({
		imports: [sharedServiceImports],
	}));

	it('should be created', () => {
		const service: WarehousesStoreService = TestBed.get(WarehousesStoreService);
		expect(service).toBeTruthy();
	});
});

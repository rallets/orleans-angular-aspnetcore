import { TestBed } from '@angular/core/testing';

import { WarehousesBackendService } from './warehouses-backend.service';
import { sharedServiceImports } from 'src/app/testing/testing.shared';

describe('WarehousesBackendService', () => {
	beforeEach(() => TestBed.configureTestingModule({
		imports: [sharedServiceImports],
	}));

	it('should be created', () => {
		const service: WarehousesBackendService = TestBed.get(WarehousesBackendService);
		expect(service).toBeTruthy();
	});
});

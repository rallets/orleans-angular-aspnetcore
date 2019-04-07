import { TestBed } from '@angular/core/testing';

import { ProductsBackendService } from './products-backend.service';
import { sharedServiceImports } from 'src/app/testing/testing.shared';

describe('ProductsBackendService', () => {
	beforeEach(() => TestBed.configureTestingModule({
		imports: [sharedServiceImports],
	}));

	it('should be created', () => {
		const service: ProductsBackendService = TestBed.get(ProductsBackendService);
		expect(service).toBeTruthy();
	});
});

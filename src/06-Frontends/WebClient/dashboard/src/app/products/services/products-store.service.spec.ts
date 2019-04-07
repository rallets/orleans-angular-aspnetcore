import { TestBed } from '@angular/core/testing';

import { ProductsStoreService } from './products-store.service';
import { sharedServiceImports } from 'src/app/testing/testing.shared';

describe('ProductsStoreService', () => {
	beforeEach(() => TestBed.configureTestingModule({
		imports: [sharedServiceImports],
	}));

	it('should be created', () => {
		const service: ProductsStoreService = TestBed.get(ProductsStoreService);
		expect(service).toBeTruthy();
	});
});

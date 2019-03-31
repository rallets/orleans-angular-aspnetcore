import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { sharedComponentImports, sharedComponentProviders } from '@app/prislister/testing.shared';
import { ProductCreateDataModel } from '../../models/products.model';
import { ProductCreateComponent } from './product-create.component';

describe('ProductCreateComponent', () => {
	let component: ProductCreateComponent;
	let fixture: ComponentFixture<ProductCreateComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [ProductCreateComponent],
			imports: [sharedComponentImports],
			providers: [sharedComponentProviders],
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(ProductCreateComponent);
		component = fixture.componentInstance;
		component.data = new ProductCreateDataModel('', '');
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { OrderCreateDataModel } from '../../models/orders.model';
import { OrderCreateComponent } from './order-create.component';
import { sharedComponentImports, sharedComponentProviders } from 'src/app/testing/testing.shared';

describe('OrderCreateComponent', () => {
	let component: OrderCreateComponent;
	let fixture: ComponentFixture<OrderCreateComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [OrderCreateComponent],
			imports: [sharedComponentImports],
			providers: [sharedComponentProviders],
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(OrderCreateComponent);
		component = fixture.componentInstance;
		component.data = new OrderCreateDataModel('', '');
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { WarehouseCreateComponent } from './warehouse-create.component';
import { WarehouseCreateDataModel } from '../../models/warehouses.model';
import { sharedComponentImports, sharedComponentProviders } from 'src/app/testing/testing.shared';

describe('WarehouseCreateComponent', () => {
	let component: WarehouseCreateComponent;
	let fixture: ComponentFixture<WarehouseCreateComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [WarehouseCreateComponent],
			imports: [sharedComponentImports],
			providers: [sharedComponentProviders],
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(WarehouseCreateComponent);
		component = fixture.componentInstance;
		component.data = new WarehouseCreateDataModel('', '');
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});

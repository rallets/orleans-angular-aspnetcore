import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuComponent } from './menu.component';
import { sharedComponentImports, sharedComponentProviders } from '../testing/testing.shared';

describe('MenuComponent', () => {
	let component: MenuComponent;
	let fixture: ComponentFixture<MenuComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [MenuComponent],
			imports: [sharedComponentImports],
			providers: [sharedComponentProviders],
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(MenuComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});

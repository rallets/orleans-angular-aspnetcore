import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IndexComponent } from './index.component';
import { sharedComponentImports, sharedComponentProviders } from 'src/app/testing/testing.shared';

describe('IndexComponent', () => {
	let component: IndexComponent;
	let fixture: ComponentFixture<IndexComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [IndexComponent],
			imports: [sharedComponentImports],
			providers: [sharedComponentProviders],
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(IndexComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});

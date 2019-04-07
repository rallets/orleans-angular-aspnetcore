import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoadingSpinnerComponent } from './loading-spinner.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

describe('LoadingSpinnerComponent', () => {
	let component: LoadingSpinnerComponent;
	let fixture: ComponentFixture<LoadingSpinnerComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [LoadingSpinnerComponent],
			imports: [FontAwesomeModule],
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(LoadingSpinnerComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});

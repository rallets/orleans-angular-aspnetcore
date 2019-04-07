import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmationComponent } from './confirmation.component';
import { NgbModalModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationModalDataModel } from '../../models/confirmation-modal-data.model';

describe('ConfirmationComponent', () => {
	let component: ConfirmationComponent;
	let fixture: ComponentFixture<ConfirmationComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [ConfirmationComponent],
			imports: [FormsModule, ReactiveFormsModule, NgbModalModule],
			providers: [NgbActiveModal]
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(ConfirmationComponent);
		component = fixture.componentInstance;
		component.data = new ConfirmationModalDataModel('body with html <br/>');
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});

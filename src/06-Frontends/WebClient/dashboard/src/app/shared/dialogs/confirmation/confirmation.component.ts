import { Component, OnInit, Input } from '@angular/core';
import { BaseModalComponent } from '../base-modal.component';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationModalDataModel } from '../../models/confirmation-modal-data.model';
import { nameofFactory } from '../../helpers/nameof-factory';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
	selector: 'ts-confirmation',
	templateUrl: './confirmation.component.html',
	styleUrls: ['./confirmation.component.scss']
})
export class ConfirmationComponent extends BaseModalComponent implements OnInit {

	@Input() data: ConfirmationModalDataModel;

	private nameof = nameofFactory<ConfirmationComponent>();

	constructor(protected activeModal: NgbActiveModal) {
		super(activeModal);
	}

	ngOnInit() {
		this.data.title = this.data.title || 'Confirmation request';
	}

	confirm() {
		this.activeModal.close(true);
	}
}

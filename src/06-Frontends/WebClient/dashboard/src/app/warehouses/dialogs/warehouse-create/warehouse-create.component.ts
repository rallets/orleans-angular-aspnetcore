import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { nameofFactory } from '../../../shared/helpers/nameof-factory';
import { Observable } from 'rxjs';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { LoadingStatusService } from '../../../shared/services/loading-status.service';
import { WarehouseCreateDataModel } from '../../models/warehouses.model';
import { WarehouseCreateRequest } from '../../models/warehouses.model';
import { BaseModalComponent } from 'src/app/shared/dialogs/base-modal.component';
import { intMaxValue } from 'src/app/shared/constants/types.const';
import { WarehousesStoreService } from '../../services/warehouses-store.service';

@Component({
	selector: 'app-warehouse-create',
	templateUrl: './warehouse-create.component.html',
	styleUrls: ['./warehouse-create.component.scss']
})
export class WarehouseCreateComponent extends BaseModalComponent implements OnInit, OnDestroy {
	@Input() data: WarehouseCreateDataModel;

	loading$: Observable<boolean>;

	private nameof = nameofFactory<WarehouseCreateComponent>();

	get code() { return this.form.controls[this.nameof('code')]; }
	get name() { return this.form.controls[this.nameof('name')]; }
	get description() { return this.form.controls[this.nameof('description')]; }

	form = new FormGroup({
		code: new FormControl({ value: '', disabled: false }, [Validators.required]),
		name: new FormControl({ value: '', disabled: false }, [Validators.required, Validators.maxLength(255)]),
		description: new FormControl({ value: '', disabled: false }, [Validators.maxLength(1024)]),
	});

	constructor(
		protected activeModal: NgbActiveModal,
		private modalService: NgbModal,
		public store: WarehousesStoreService,
		private loadingStatus: LoadingStatusService,
	) {
		super(activeModal);
	}

	ngOnInit() {
		this.loading$ = this.loadingStatus.loading$;
	}

	ngOnDestroy() {
		// needed by untilDestroyed
	}

	async save() {
		const name = this.name.value as string;
		const code = this.code.value as string;
		if (!this.data || !code || !name) {
			return;
		}
		if (!this.form.valid) {
			console.warn('Invalid form', this.form.errors);
			return;
		}

		const request = new WarehouseCreateRequest();
		request.code = code;
		request.name = name;
		request.description = this.description.value;

		const delay = new Promise((resolve, _reject) => setTimeout(() => resolve(), this.DELAY_CLOSE_MS));

		try {
			const success = await this.store.createWarehouse(request);

			if (success) {
				delay.then(_ => this.activeModal.close());
			}
		} catch {
			// Action failed
		}
	}

}

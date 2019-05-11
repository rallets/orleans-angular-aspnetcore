import { Component, OnInit } from '@angular/core';
import { Warehouse, WarehouseCreateDataModel } from '../models/warehouses.model';
import { Observable, combineLatest } from 'rxjs';
import { WarehousesStoreService } from '../services/warehouses-store.service';
import { LoadingStatusService } from 'src/app/shared/services/loading-status.service';
import { NotificationStoreService } from 'src/app/shared/services/notification-store.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';
import { orderBy } from 'lodash-es';
import { WarehouseCreateComponent } from '../dialogs/warehouse-create/warehouse-create.component';
import { nameofFactory } from 'src/app/shared/helpers/nameof-factory';
import { guid } from 'src/app/shared/types/guid.type';

@Component({
	selector: 'app-warehouses-index',
	templateUrl: './index.component.html',
	styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {
	loading$: Observable<boolean>;
	warehouses$: Observable<Warehouse[]>;

	private _warehouses$: Observable<Warehouse[]>;
	nameofWarehouse = nameofFactory<Warehouse>();

	constructor(
		private loadingStatus: LoadingStatusService,
		private store: WarehousesStoreService,
		private notification: NotificationStoreService,
		private modalService: NgbModal,
	) { }

	ngOnInit() {
		this.loading$ = this.loadingStatus.loadingDelayed$;
		this._warehouses$ = this.store.warehouses$;
		this.warehouses$ = this.createView(this._warehouses$);

		this.store.getWarehouses();
	}

	onGetWarehouses() {
		this.store.getWarehouses();
	}

	onGetInventory(warehouseGuid: guid) {
		this.store.getInventory(warehouseGuid);
	}

	async onCreateWarehouse() {
		const modalRef = this.modalService.open(WarehouseCreateComponent, { size: 'lg' });
		const ci = modalRef.componentInstance;

		ci.data = new WarehouseCreateDataModel(
			'Create a new warehouse',
			'New warehouse'
		);

		try {
			await modalRef.result;
		} catch { /* form closed */ }
	}

	private createView(
		warehouses$: Observable<Warehouse[]>,
	) {
		const combinedStreams = combineLatest(warehouses$);
		return combinedStreams.pipe(
			// debounceTime(200),
			// distinctUntilChanged(),
			map(([warehouses]) => {
				const items = orderBy(warehouses, this.nameofWarehouse('creationDate'), 'desc') as Warehouse[];
				return items.slice(0, 10);
			})
		);
	}
}

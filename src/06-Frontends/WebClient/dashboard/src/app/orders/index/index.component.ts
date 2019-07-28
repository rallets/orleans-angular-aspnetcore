import { Component, OnInit } from '@angular/core';
import { OrdersBackendService } from '../services/orders-backend.service';
import { Order, OrderCreateDataModel, OrdersStats, OrderEvent } from '../models/orders.model';
import { faCoffee } from '@fortawesome/free-solid-svg-icons';
import { Observable, combineLatest } from 'rxjs';
import { OrdersStoreService } from '../services/orders-store.service';
import { LoadingStatusService } from 'src/app/shared/services/loading-status.service';
import { NotificationStoreService } from 'src/app/shared/services/notification-store.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime, map } from 'rxjs/operators';
import { orderBy } from 'lodash-es';
import { OrderCreateComponent } from '../dialogs/order-create/order-create.component';
import { nameofFactory } from 'src/app/shared/helpers/nameof-factory';
import { guid } from 'src/app/shared/types/guid.type';

@Component({
	selector: 'app-orders-index',
	templateUrl: './index.component.html',
	styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {
	loading$: Observable<boolean>;
	orders$: Observable<Order[]>;
	stats$: Observable<OrdersStats>;
	events$: Observable<string[]>;

	private _orders$: Observable<Order[]>;
	nameofOrder = nameofFactory<Order>();
	nameofOrderEvent = nameofFactory<OrderEvent>();

	constructor(
		private loadingStatus: LoadingStatusService,
		private store: OrdersStoreService,
		private notification: NotificationStoreService,
		private modalService: NgbModal,
	) { }

	ngOnInit() {
		this.loading$ = this.loadingStatus.loadingDelayed$;
		this._orders$ = this.store.orders$;
		this.orders$ = this.createView(this._orders$);
		this.stats$ = this.store.stats$;

		this.store.getOrders();
		this.store.getStats(5000);
	}

	onGetOrders() {
		this.store.getOrders();
	}

	onGetEvents(orderGuid: guid) {
		this.store.getOrderEvents(orderGuid);
	}

	async onCreateOrder() {
		const modalRef = this.modalService.open(OrderCreateComponent, { size: 'lg' });
		const ci = modalRef.componentInstance;

		ci.data = new OrderCreateDataModel(
			'Create a new order',
			'New order'
		);

		try {
			await modalRef.result;
		} catch { /* form closed */ }
	}

	private createView(
		orders$: Observable<Order[]>,
	) {
		const combinedStreams = combineLatest(orders$);
		return combinedStreams.pipe(
			// debounceTime(200),
			// distinctUntilChanged(),
			map(([orders]) => {
				const items = orderBy(orders, this.nameofOrder('date'), 'desc') as Order[];
				items.forEach(item => {
					item.events = orderBy(item.events, this.nameofOrderEvent('date'), 'asc') as OrderEvent[];
				});
				return items.slice(0, 100);
			})
		);
	}
}

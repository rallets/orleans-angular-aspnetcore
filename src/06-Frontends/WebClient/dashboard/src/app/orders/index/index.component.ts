import { Component, OnInit } from '@angular/core';
import { OrdersBackendService } from '../services/orders-backend.service';
import { Order, OrderCreateDataModel } from '../models/orders.model';
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

@Component({
  selector: 'app-orders-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {
  loading$: Observable<boolean>;
  orders$: Observable<Order[]>;

  private _orders$: Observable<Order[]>;
  nameofOrder = nameofFactory<Order>();

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

    this.store.getOrders();
  }

  onGetOrders() {
    this.store.getOrders();
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

  // onAddOrder() {
  //   const request = new OrderRequest();
  //   request.name = 'order';
  //   request.totalAmount = 100;
  //   request.items = [{
  //     productId: '5cfab5ac-8640-4955-a541-7031166c6817',
  //     quantity: 1
  //   } as OrderItemRequest];

  //   const result = this.backendService.postOrder(request);
  //   result.subscribe(data => {
  //     console.log(data);
  //     this.orders.push(data);
  //   });
  // }

  private createView(
    orders$: Observable<Order[]>,
  ) {
    const combinedStreams = combineLatest(orders$);
    return combinedStreams.pipe(
      // debounceTime(200),
      // distinctUntilChanged(),
      map(([orders]) => {
        const items = orderBy(orders, this.nameofOrder('date'), 'desc') as Order[];
        return items.slice(0, 10);
      })
    );
  }
}

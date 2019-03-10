import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Order, OrderCreateRequest } from '../models/orders.model';
import { OrdersBackendService } from './orders-backend.service';
import { NotificationStoreService } from 'src/app/shared/services/notification-store.service';
import { handleHttpError } from 'src/app/shared/helpers/http/http-error.helpers';
import { NotificationLevel } from 'src/app/shared/models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class OrdersStoreService implements OnDestroy {

  private _orders: BehaviorSubject<Order[]> = new BehaviorSubject([]);

  constructor(
    private backend: OrdersBackendService,
    private notification: NotificationStoreService,
  ) { }

  ngOnDestroy() {
    // needed by untilDestroyed
  }

  get orders$() {
    return this._orders.asObservable();
  }

  getOrders() {
    const items = this.backend.getOrders();
    items.subscribe(result => {
      this._orders.next(result.orders);
      console.log(this._orders);
    }, error => {
      handleHttpError(error, this.notification);
      this._orders.next([]);
    });
  }

  async createOrder(request: OrderCreateRequest) {
    try {
      const response = await this.backend.createOrder(request);
      this.notification.dispatch(NotificationLevel.success, 'Order created');

      const orders = this._orders.getValue();
      orders.push(response);
      this._orders.next(orders);
      return true;
    } catch (error) {
      handleHttpError(error, this.notification);
    }
  }
}

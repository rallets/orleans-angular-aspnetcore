import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap, share } from 'rxjs/operators';
import { Deserialize } from 'cerialize';
import { Orders, Order, OrderCreateRequest, OrdersStats, OrderEvents } from '../models/orders.model';
import { guid } from 'src/app/shared/types/guid.type';

const httpOptions = {
	headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
	providedIn: 'root'
})
export class OrdersBackendService {

	private baseUrl = environment.apiOrders;

	constructor(
		private http: HttpClient
	) { }

	getOrders(): Observable<Orders> {
		const url = `${this.baseUrl}`;

		return this.http.get<Orders>(url).pipe(
			map(response => Deserialize(response, Orders)),
			share()
		);
	}

	getOrderEvents(orderGuid: guid): Observable<OrderEvents> {
		const url = `${this.baseUrl}/${orderGuid}/events`;

		return this.http.get<OrderEvents>(url).pipe(
			map(response => Deserialize(response, OrderEvents)),
			share()
		);
	}

	getStats(): Observable<OrdersStats> {
		const url = `${this.baseUrl}/stats`;

		return this.http.get<OrdersStats>(url).pipe(
			map(response => Deserialize(response, OrdersStats)),
			share()
		);
	}

	async createOrder(request: OrderCreateRequest): Promise<Order> {
		const url = `${this.baseUrl}`;

		return this.http.post<Order>(url, request).pipe(
			map(response => Deserialize(response, Order)),
		).toPromise();
	}

}

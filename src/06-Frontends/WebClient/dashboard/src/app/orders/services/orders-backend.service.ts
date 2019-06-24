import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap, share } from 'rxjs/operators';
import { Deserialize } from 'cerialize';
import { Orders, Order, OrderCreateRequest, OrdersStats } from '../models/orders.model';

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

	getStats(): Observable<OrdersStats> {
		console.log('Getting new stats...');
		const url = `${this.baseUrl}/stats`;

		return this.http.get<OrdersStats>(url).pipe(
			map(response => Deserialize(response, OrdersStats)),
			share()
		);
	}

	async createOrder(request: OrderCreateRequest): Promise<Order> {
		const url = `${this.baseUrl}`;

		console.log(request);
		return this.http.post<Order>(url, request).pipe(
			map(response => Deserialize(response, Order)),
		).toPromise();
	}

}

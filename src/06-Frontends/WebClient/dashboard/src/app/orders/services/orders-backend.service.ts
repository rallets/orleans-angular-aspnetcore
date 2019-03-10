import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap, share } from 'rxjs/operators';
import { Deserialize } from 'cerialize';
import { Orders, Order, OrderCreateRequest } from '../models/orders.model';

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

	// getOrders(): Observable<Orders> {
	//   return this.http.get<Orders>(this.baseUrl)
	//     .pipe(
	//       map(response => Deserialize(response, Orders)),
	//       tap(orders => this.log(`fetched orders`, orders)),
	//       catchError(this.handleError('getOrders', ''))
	//     );
	// }

	// getOrder(id: guid): Observable<Order> {
	//   const url = `${this.baseUrl}/${id}`;
	//   return this.http.get<Order>(url)
	//     .pipe(
	//       map(response => Deserialize(response, Order)),
	//       tap(order => this.log(`fetched order`, order)),
	//       catchError(this.handleError('getOrder', ''))
	//     );
	// }

	// postOrder(request: OrderRequest): Observable<Order> {
	//   return this.http.post<Order>(this.baseUrl, request)
	//     .pipe(
	//       map(response => Deserialize(response, Order)),
	//       tap(order => this.log(`post order`, order)),
	//       catchError(this.handleError('postOrder', ''))
	//     );
	// }

	async createOrder(request: OrderCreateRequest): Promise<Order> {
		const url = `${this.baseUrl}`;

		console.log(request);
		return this.http.post<Order>(url, request).pipe(
			map(response => Deserialize(response, Order)),
		).toPromise();
	}

	// private log(text: string, ...args: any) {
	//   console.log(text, args);
	// }

	// private handleError<T>(operation = 'operation', result?: T) {
	//   return (error: any): Observable<T> => {

	//     // TODO: send the error to remote logging
	//     console.error(error); // log to console instead

	//     // TODO: better job of transforming error for user consumption
	//     this.log(`${operation} failed: ${error.message}`);

	//     // Let the app keep running by returning an empty result.
	//     return of(result as T);
	//   };
	// }

}

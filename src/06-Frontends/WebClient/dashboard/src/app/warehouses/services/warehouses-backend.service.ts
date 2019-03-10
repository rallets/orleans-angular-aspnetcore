import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap, share } from 'rxjs/operators';
import { Deserialize } from 'cerialize';
import { Warehouses, Warehouse, WarehouseCreateRequest } from '../models/warehouses.model';

const httpOptions = {
	headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
	providedIn: 'root'
})
export class WarehousesBackendService {

	private baseUrl = environment.apiWarehouses;

	constructor(
		private http: HttpClient
	) { }

	getWarehouses(): Observable<Warehouses> {
		const url = `${this.baseUrl}`;

		return this.http.get<Warehouses>(url).pipe(
			map(response => Deserialize(response, Warehouses)),
			share()
		);
	}

	async createWarehouse(request: WarehouseCreateRequest): Promise<Warehouse> {
		const url = `${this.baseUrl}`;

		console.log(request);
		return this.http.post<Warehouse>(url, request).pipe(
			map(response => Deserialize(response, Warehouse)),
		).toPromise();
	}

}

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap, share } from 'rxjs/operators';
import { Deserialize } from 'cerialize';
import { Warehouses, Warehouse, WarehouseCreateRequest } from '../models/warehouses.model';
import { Inventory } from '../models/inventories.model';
import { guid } from 'src/app/shared/types/guid.type';

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
			map(response => {
				const result = Deserialize(response, Warehouses) as Warehouses;
				result.warehouses.forEach(warehouse => {
					warehouse.inventory = new Inventory();
				});
				return result;
			}),
			share()
		);
	}

	getInventory(warehouseGuid: guid): Observable<Inventory> {
		const url = `${this.baseUrl}/${warehouseGuid}/inventory`;

		return this.http.get<Inventory>(url).pipe(
			map(response => Deserialize(response, Inventory)),
			share()
		);
	}

	async createWarehouse(request: WarehouseCreateRequest): Promise<Warehouse> {
		const url = `${this.baseUrl}`;

		return this.http.post<Warehouse>(url, request).pipe(
			map(response => {
				var result = Deserialize(response, Warehouse);
				result.inventory = new Inventory();
				return result;
			}),
		).toPromise();
	}

}

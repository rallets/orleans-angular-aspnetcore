import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap, share } from 'rxjs/operators';
import { Deserialize } from 'cerialize';
import { Products, Product, ProductCreateRequest } from '../models/products.model';

const httpOptions = {
	headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
	providedIn: 'root'
})
export class ProductsBackendService {

	private baseUrl = environment.apiProducts;

	constructor(
		private http: HttpClient
	) { }

	getProducts(): Observable<Products> {
		const url = `${this.baseUrl}`;

		return this.http.get<Products>(url).pipe(
			map(response => Deserialize(response, Products)),
			share()
		);
	}

	async createProduct(request: ProductCreateRequest): Promise<Product> {
		const url = `${this.baseUrl}`;

		return this.http.post<Product>(url, request).pipe(
			map(response => Deserialize(response, Product)),
		).toPromise();
	}

}

import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product, ProductCreateRequest } from '../models/products.model';
import { ProductsBackendService } from './products-backend.service';
import { NotificationStoreService } from 'src/app/shared/services/notification-store.service';
import { handleHttpError } from 'src/app/shared/helpers/http/http-error.helpers';
import { NotificationLevel } from 'src/app/shared/models/notification.model';

@Injectable({
	providedIn: 'root'
})
export class ProductsStoreService implements OnDestroy {

	private _products: BehaviorSubject<Product[]> = new BehaviorSubject([]);

	constructor(
		private backend: ProductsBackendService,
		private notification: NotificationStoreService,
	) { }

	ngOnDestroy() {
		// needed by untilDestroyed
	}

	get products$() {
		return this._products.asObservable();
	}

	getProducts() {
		const items = this.backend.getProducts();
		items.subscribe(result => {
			this._products.next(result.products);
		}, error => {
			handleHttpError(error, this.notification);
			this._products.next([]);
		});
	}

	async createProduct(request: ProductCreateRequest) {
		try {
			const response = await this.backend.createProduct(request);
			this.notification.dispatch(NotificationLevel.success, 'Product created');

			const products = this._products.getValue();
			products.push(response);
			this._products.next(products);
			return true;
		} catch (error) {
			handleHttpError(error, this.notification);
		}
	}
}

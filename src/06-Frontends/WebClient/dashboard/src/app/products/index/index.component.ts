import { Component, OnInit } from '@angular/core';
import { Product, ProductCreateDataModel } from '../models/products.model';
import { Observable, combineLatest } from 'rxjs';
import { ProductsStoreService } from '../services/products-store.service';
import { LoadingStatusService } from 'src/app/shared/services/loading-status.service';
import { NotificationStoreService } from 'src/app/shared/services/notification-store.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';
import { orderBy } from 'lodash-es';
import { ProductCreateComponent } from '../dialogs/product-create/product-create.component';
import { nameofFactory } from 'src/app/shared/helpers/nameof-factory';

@Component({
	selector: 'app-products-index',
	templateUrl: './index.component.html',
	styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {
	loading$: Observable<boolean>;
	products$: Observable<Product[]>;

	private _products$: Observable<Product[]>;
	nameofProduct = nameofFactory<Product>();

	constructor(
		private loadingStatus: LoadingStatusService,
		private store: ProductsStoreService,
		private notification: NotificationStoreService,
		private modalService: NgbModal,
	) { }

	ngOnInit() {
		this.loading$ = this.loadingStatus.loadingDelayed$;
		this._products$ = this.store.products$;
		this.products$ = this.createView(this._products$);

		this.store.getProducts();
	}

	onGetProducts() {
		this.store.getProducts();
	}

	async onCreateProduct() {
		const modalRef = this.modalService.open(ProductCreateComponent, { size: 'lg' });
		const ci = modalRef.componentInstance;

		ci.data = new ProductCreateDataModel(
			'Create a new product',
			'New product'
		);

		try {
			await modalRef.result;
		} catch { /* form closed */ }
	}

	private createView(
		products$: Observable<Product[]>,
	) {
		const combinedStreams = combineLatest(products$);
		return combinedStreams.pipe(
			// debounceTime(200),
			// distinctUntilChanged(),
			map(([products]) => {
				const items = orderBy(products, this.nameofProduct('code'), 'asc') as Product[];
				return items.slice(0, 100);
			})
		);
	}
}

import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { nameofFactory } from '../../../shared/helpers/nameof-factory';
import { Observable } from 'rxjs';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { LoadingStatusService } from '../../../shared/services/loading-status.service';
import { OrderCreateDataModel } from '../../models/orders.model';
import { OrderCreateRequest } from '../../models/orders.model';
import { BaseModalComponent } from 'src/app/shared/dialogs/base-modal.component';
import { intMaxValue } from 'src/app/shared/constants/types.const';
import { OrdersStoreService } from '../../services/orders-store.service';
import { ProductsStoreService } from 'src/app/products/services/products-store.service';
import { Product } from 'src/app/products/models/products.model';
import { guid } from 'src/app/shared/types/guid.type';
import { map } from 'rxjs/operators';
import { orderBy } from 'lodash-es';

@Component({
	selector: 'app-order-create',
	templateUrl: './order-create.component.html',
	styleUrls: ['./order-create.component.scss']
})
export class OrderCreateComponent extends BaseModalComponent implements OnInit, OnDestroy {
	@Input() data: OrderCreateDataModel;

	loading$: Observable<boolean>;
	products$: Observable<Product[]>;
	numProducts: number = 0;
	intMaxValue = intMaxValue;

	private nameof = nameofFactory<OrderCreateComponent>();
	private nameofProduct = nameofFactory<Product>();

	get name() { return this.form.controls[this.nameof('name')]; }
	get productId() { return this.form.controls[this.nameof('productId')]; }
	get quantity() { return this.form.controls[this.nameof('quantity')]; }

	form = new FormGroup({
		name: new FormControl({ value: 'New order', disabled: false }, [Validators.required, Validators.maxLength(255)]),
		productId: new FormControl({ value: '', disabled: false }, [Validators.required]),
		quantity: new FormControl({ value: 1, disabled: false }, [Validators.min(1), Validators.max(intMaxValue)]),
	});

	constructor(
		protected activeModal: NgbActiveModal,
		private modalService: NgbModal,
		public store: OrdersStoreService,
		public productsStore: ProductsStoreService,
		private loadingStatus: LoadingStatusService,
	) {
		super(activeModal);
	}

	ngOnInit() {
		this.loading$ = this.loadingStatus.loading$;
		this.products$ = this.productsStore.products$.pipe(
			map((products) => {
				this.numProducts = products.length;
				const items = orderBy(products, this.nameofProduct('creationDate'), 'desc') as Product[];
				return items.slice(0, 10);
			})
		);

		this.productsStore.getProducts();
	}

	ngOnDestroy() {
		// needed by untilDestroyed
	}

	async save() {
		const name = this.name.value as string;
		const pid = this.productId.value as guid;
		const qty = this.quantity.value as number;
		if (!this.data || !name || !pid || !qty) {
			return;
		}
		if (!this.form.valid) {
			console.warn('Invalid form', this.form.errors);
			return;
		}

		const request = new OrderCreateRequest();
		request.name = name;
		request.items = [{ productId: pid, quantity: qty }];

		const delay = new Promise((resolve, _reject) => setTimeout(() => resolve(), this.DELAY_CLOSE_MS));

		try {
			const success = await this.store.createOrder(request);

			if (success) {
				delay.then(_ => this.activeModal.close());
			}
		} catch {
			// Action failed
		}
	}

}

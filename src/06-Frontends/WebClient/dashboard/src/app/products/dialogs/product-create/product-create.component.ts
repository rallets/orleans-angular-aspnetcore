import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { nameofFactory } from '../../../shared/helpers/nameof-factory';
import { Observable } from 'rxjs';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { LoadingStatusService } from '../../../shared/services/loading-status.service';
import { ProductCreateDataModel } from '../../models/products.model';
import { ProductCreateRequest } from '../../models/products.model';
import { BaseModalComponent } from 'src/app/shared/dialogs/base-modal.component';
import { intMaxValue, decimalMaxValue, decimalMinValue } from 'src/app/shared/constants/types.const';
import { ProductsStoreService } from '../../services/products-store.service';

@Component({
  selector: 'app-product-create',
  templateUrl: './product-create.component.html',
  styleUrls: ['./product-create.component.scss']
})
export class ProductCreateComponent extends BaseModalComponent implements OnInit, OnDestroy {
  @Input() data: ProductCreateDataModel;

  loading$: Observable<boolean>;

  private nameof = nameofFactory<ProductCreateComponent>();

  get code() { return this.form.controls[this.nameof('code')]; }
  get name() { return this.form.controls[this.nameof('name')]; }
  get description() { return this.form.controls[this.nameof('description')]; }
  get price() { return this.form.controls[this.nameof('price')]; }

  form = new FormGroup({
    code: new FormControl({ value: '', disabled: false }, [Validators.required]),
    name: new FormControl({ value: '', disabled: false }, [Validators.required, Validators.maxLength(255)]),
    description: new FormControl({ value: '', disabled: false }, [Validators.maxLength(1024)]),
    price: new FormControl({ value: 0, disabled: false }, [
      Validators.required,
      Validators.max(decimalMaxValue),
      Validators.min(decimalMinValue)
    ]),
  });

  constructor(
    protected activeModal: NgbActiveModal,
    private modalService: NgbModal,
    public store: ProductsStoreService,
    private loadingStatus: LoadingStatusService,
  ) {
    super(activeModal);
  }

  ngOnInit() {
    this.loading$ = this.loadingStatus.loading$;
  }

  ngOnDestroy() {
    // needed by untilDestroyed
  }

  async save() {
    const name = this.name.value as string;
    const code = this.code.value as string;
    const price = this.price.value as number;
    if (!this.data || !code || !name) {
      return;
    }
    if (!this.form.valid) {
      console.warn('Invalid form', this.form.errors);
      return;
    }

    const request = new ProductCreateRequest();
    request.code = code;
    request.name = name;
    request.description = this.description.value;
    request.price = price;

    const delay = new Promise((resolve, _reject) => setTimeout(() => resolve(), this.DELAY_CLOSE_MS));

    try {
      const success = await this.store.createProduct(request);

      if (success) {
        delay.then(_ => this.activeModal.close());
      }
    } catch {
      // Action failed
    }
  }

}

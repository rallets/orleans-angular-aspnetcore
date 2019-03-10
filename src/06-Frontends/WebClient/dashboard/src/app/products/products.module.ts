import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';

import { ProductsRoutingModule } from './products-routing.module';
import { IndexComponent } from './index/index.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ProductCreateComponent } from './dialogs/product-create/product-create.component';
import { LoadingSpinnerComponent } from '../shared/components/loading-spinner/loading-spinner.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    // CommonModule,
    // FontAwesomeModule,
    ProductsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgbModule,
    NgSelectModule,
    SharedModule,
  ],
  declarations: [
    IndexComponent,
    ProductCreateComponent,
    // LoadingSpinnerComponent,
  ],
  entryComponents: [
    // ConfirmationComponent,
    // MessageComponent,
    ProductCreateComponent,
  ]
})
export class ProductsModule { }

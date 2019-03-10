import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';

import { HomeRoutingModule } from './home-routing.module';
import { IndexComponent } from './index/index.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    // CommonModule,
    // FontAwesomeModule,
    HomeRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgbModule,
    NgSelectModule,
    SharedModule,
  ],
  declarations: [
    IndexComponent,
  ],
  entryComponents: [
    // ConfirmationComponent,
    // MessageComponent,
    // AddOrderComponent,
  ]
})
export class HomeModule { }

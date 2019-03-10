import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { registerLocaleData } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import localeNb from '@angular/common/locales/nb';
registerLocaleData(localeNb);

import { LoadingBarHttpClientModule } from '@ngx-loading-bar/http-client';
import { LoadingBarRouterModule } from '@ngx-loading-bar/router';
import { LoadingBarModule } from '@ngx-loading-bar/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgSelectModule } from '@ng-select/ng-select';
import { SimpleNotificationsModule, Options, NotificationAnimationType } from 'angular2-notifications';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { IndexComponent } from './orders/index/index.component';
import { MenuComponent } from './menu/menu.component';
import { OrdersModule } from './orders/orders.module';
import { HomeModule } from './home/home.module';
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner.component';
import { WarehousesModule } from './warehouses/warehouses.module';
import { ProductsModule } from './products/products.module';
import { SharedModule } from './shared/shared.module';

const notificationOptions: Options = {
  animate: NotificationAnimationType.FromRight,
  position: ['bottom', 'right'],
  maxLength: 4096,
  maxStack: 4,
  timeOut: 3000,
  showProgressBar: false,
  pauseOnHover: true,
  clickToClose: true,
  clickIconToClose: true,
  // theClass: 'ts-simple-notify' // Add our own class to make it more specific than default styles
};

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    // LoadingSpinnerComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    SharedModule,
    // FontAwesomeModule,
    HttpClientModule,
    LoadingBarHttpClientModule,
    LoadingBarRouterModule,
    LoadingBarModule,
    NgbModule,
    NgSelectModule,
    SimpleNotificationsModule.forRoot(notificationOptions),
    HomeModule,
    OrdersModule,
    ProductsModule,
    WarehousesModule,
    AppRoutingModule, // order matters: AppRoutingModule must be the latest module to be imported
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'nb' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

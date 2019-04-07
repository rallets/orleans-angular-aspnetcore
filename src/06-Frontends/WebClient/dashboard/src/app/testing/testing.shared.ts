import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { NgbActiveModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';

export const sharedComponentDeclarations = [
];

export const sharedComponentImports = [
	FormsModule,
	HttpClientTestingModule,
	NgbModule,
	NgSelectModule,
	ReactiveFormsModule,
	RouterTestingModule,
	SharedModule,
	SimpleNotificationsModule.forRoot({}),
];

export const sharedComponentProviders = [
	LoadingBarService,
	NgbActiveModal,
];

export const sharedServiceImports = [
	HttpClientTestingModule,
	SimpleNotificationsModule.forRoot({}),
];

export const sharedServiceProviders = [
];

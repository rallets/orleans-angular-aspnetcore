import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ConfirmationComponent } from './dialogs/confirmation/confirmation.component';
import { MessageComponent } from './dialogs/message/message.component';

@NgModule({
	declarations: [
		ConfirmationComponent,
		LoadingSpinnerComponent,
		MessageComponent,
	],
	imports: [
		CommonModule,
		FontAwesomeModule,
	],
	entryComponents: [
	], // Dialogs must be in entrycomponents.
	providers: [
	],
	exports: [
		CommonModule,
		ConfirmationComponent,
		FontAwesomeModule,
		LoadingSpinnerComponent,
		MessageComponent,
	]
})

export class SharedModule {
	constructor() { }
}

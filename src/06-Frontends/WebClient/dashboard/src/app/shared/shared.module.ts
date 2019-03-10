import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@NgModule({
  declarations: [
    LoadingSpinnerComponent,
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
    FontAwesomeModule,
    LoadingSpinnerComponent,
  ]
})

export class SharedModule {
  constructor() { }
}

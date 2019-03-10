import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ErrorComponent } from './error/error.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'home' },
  { path: '**', component: ErrorComponent }
];

const routeOptions = {
  // enableTracing: !environment.production
};

@NgModule({
  imports: [RouterModule.forRoot(routes, routeOptions)],
  declarations: [ErrorComponent],
  exports: [RouterModule]
})
export class AppRoutingModule { }

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { SharedModule } from './shared/shared.module';
import { CustomInterceptor } from './interceptors/custom.interceptor';
import { AuthGuard } from './shared/guards/auth.guard';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { HeaderComponent } from './shared/components/header/header.component';

@NgModule({
  declarations: [AppComponent],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CustomInterceptor,
      multi: true,
    },

  ],
  bootstrap: [AppComponent],
  imports: [
    BrowserModule,
    SharedModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    RouterModule.forRoot([
      {
        path: 'Home',
        loadChildren: () =>
          import('./home/home.module').then((h) => h.HomeModule),
        canLoad: [AuthGuard],
      },
      {
        path: 'Instructor',
        loadChildren: () =>
          import('./instructor/instructor.module').then(
            (i) => i.InstructorModule
          ),
        canLoad: [AuthGuard],
      },
      {
        path: 'Cart',
        loadChildren: () =>
          import('./cart/cart.module').then((c) => c.CartModule),
        canLoad: [AuthGuard],
      },
      {
        path: 'Checkout',
        loadChildren: () =>
          import('./checkout/checkout.module').then((c) => c.CheckoutModule),
        canLoad: [AuthGuard],
      },
      {
        path: 'Admin',
        loadChildren: () =>
          import('./admin/admin.module').then((a) => a.AdminModule),
        canLoad: [AuthGuard],
      },
      {
        path: 'Account',
        loadChildren: () =>
          import('./account/account.module').then((a) => a.AccountModule),
      },
      { path: '', redirectTo: 'Home', pathMatch: 'full' },
      { path: '**', redirectTo: 'Home', pathMatch: 'full' },
    ]),
  ],
})
export class AppModule { }

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './components/login/login.component';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { RegisterComponent } from './components/register/register.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';



@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([
      {
        path: 'Login', component: LoginComponent,
      },
      {
        path: 'Register', component: RegisterComponent,
      },
      {
        path: 'ForgotPassword', component: ForgotPasswordComponent,
      },
      {
        path: 'ResetPassword', component: ResetPasswordComponent,
      },
      { path: '', redirectTo: 'Login', pathMatch: 'full' },
      { path: '**', redirectTo: 'Login', pathMatch: 'full' },
    ])
  ]
})
export class AccountModule { }

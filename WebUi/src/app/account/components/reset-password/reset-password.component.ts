import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/services/user.service';
import { SubSink } from 'subsink';
import { IRegister } from '../../models/IRegister';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {
  isLoading: boolean = false;
  private subs = new SubSink();

  constructor(
    private userService: UserService,
    private router: Router,
    private fb: FormBuilder,
    private toastrService: ToastrService
  ) { }

  resetPasswordForm: FormGroup = this.fb.group(
    {
      email: new FormControl(this.userService.getEmail()),
      code: new FormControl(null, [Validators.required]),
      password: new FormControl(null, [Validators.required, Validators.pattern(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,20}$/)]),
      ConfirmPassword: new FormControl(null, [Validators.required]),
    },
    { validators: this.customPassword }
  );

  customPassword(resetPasswordForm: any) {
    let password = resetPasswordForm.get('password');
    let ConfirmPassword = resetPasswordForm.get('ConfirmPassword');
    if (password?.value !== ConfirmPassword?.value) {
      ConfirmPassword.setErrors({ math: 'math' });
      return { math: 'math' };
    } else {
      return null;
    }
  }

  onSubmit() {
    this.isLoading = true;
    this.subs.sink = this.userService.resetPassword(this.resetPasswordForm.value).subscribe({
      next: (result) => {
        this.subs.sink = this.userService.login({
          email: localStorage.getItem('email')!,
          password: this.resetPasswordForm.get('password')?.value
        }).subscribe({
          next: (result) => {
            this.userService.setToken(result.data.jwToken);
            this.router.navigate(['/Home']);
          },
          error: (err) => {
            this.router.navigate(['/Account/Login']);
          },
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        console.log(err);
        this.toastrService.error('', err, {
          timeOut: 3000,
        });
      },
    });
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}

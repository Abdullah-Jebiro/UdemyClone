import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserService } from 'src/app/services/user.service';
import { SubSink } from 'subsink';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  isLoading: boolean = false;


  private subs = new SubSink();

  constructor(
    private userService: UserService,
    private router: Router,
    private fb: FormBuilder,
    private toastrService: ToastrService
  ) { }

  ForgotPasswordForm: FormGroup = this.fb.group(
    {
      email: new FormControl(null, [Validators.required, Validators.email]),
    },
  );
  onSubmit() {
    this.subs.sink = this.userService.forgotPassword(this.ForgotPasswordForm.value).subscribe({
      next: (result) => {
        this.router.navigate(['/Account/ResetPassword']);
        localStorage.setItem('email', this.ForgotPasswordForm.value)
        this.toastrService.success(result.message, '', {
          timeOut: 9000,
        });
        this.userService.setEmail(this.ForgotPasswordForm.get('email')?.value)
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

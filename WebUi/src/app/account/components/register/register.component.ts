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
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  isLoading: boolean = false;
  private subs = new SubSink();

  constructor(
    private userService: UserService,
    private router: Router,
    private fb: FormBuilder,
    private toastrService: ToastrService
  ) { }

  registerForm: FormGroup = this.fb.group(
    {
      userName: new FormControl(null, [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(20),
        Validators.pattern(/^[a-zA-Z0-9]*$/),
      ]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [
        Validators.required,
        Validators.pattern(
          /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,20}$/
        ),
      ]),
      ConfirmPassword: new FormControl(null, [Validators.required]),
    },
    { validators: this.customPassword }
  );

  customPassword(registerForm: any) {
    let password = registerForm.get('password');
    let ConfirmPassword = registerForm.get('ConfirmPassword');
    if (password?.value !== ConfirmPassword?.value) {
      ConfirmPassword.setErrors({ math: 'math' });
      return { math: 'math' };
    } else {
      return null;
    }
  }

  onSubmit() {
    this.isLoading = true;
    this.subs.sink = this.userService
      .register(this.registerForm.value)
      .subscribe({
        next: (result) => {
          this.router.navigate(['/Account/login']);
          this.toastrService.success(result.message, '', {
            timeOut: 9000,
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

import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserService } from 'src/app/services/user.service';
import { SubSink } from 'subsink';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;


  private subs = new SubSink();

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  constructor(
    private userService: UserService,
    private router: Router,
    private fb: FormBuilder,
    private toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }



  onSubmit(): void {
    //This function only works when the from is valid
    this.subs.sink = this.userService.login(this.loginForm.value).subscribe({
      next: (result) => {
        this.userService.setToken(result.data.jwToken);
        this.userService.setUserRoles(result.data.roles);
        this.router.navigate(['./Home']);
      },
      error: (err) => {
        this.toastrService.error("The username or password is incorrect, please try again.", '', {
          timeOut: 5000,
        });
      },
    });
  }
}

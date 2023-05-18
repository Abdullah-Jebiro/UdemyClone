import { Directive, ElementRef, OnInit } from '@angular/core';
import { UserService } from 'src/app/services/user.service';

@Directive({
  selector: '[isAdmin]'
})
export class IsAdminDirective implements OnInit {
  constructor(
    private elementRef: ElementRef,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    const userRoles = this.userService.getUserRoles();
    if (!userRoles.includes('Admin') && !userRoles.includes('SuperAdmin')) {
      this.elementRef.nativeElement.style.display = 'none';
      this.elementRef.nativeElement.style.color = 'red';
    }
  }
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { InstructorService } from 'src/app/services/instructor.service';
import { UserService } from 'src/app/services/user.service';
import { SubSink } from 'subsink';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {

  searchInput: string = '';
  isMobileMenuActive: boolean = false;
  isSearchBarActive: boolean = false;
  @Input() countItems!: number;
  @Input() search: boolean = true;
  @Input() filter: boolean = true;
  @Output() searchTerm = new EventEmitter<string>()!;
  @Output() filterStatusChange = new EventEmitter()!;

  private subs = new SubSink();


  constructor(
    private instructorService: InstructorService,
    private router: Router,
    private userService: UserService,
  ) { }

  toggleMobileMenu() {
    this.isMobileMenuActive = !this.isMobileMenuActive;
  }

  toggleSearchBar() {
    this.isSearchBarActive = !this.isSearchBarActive;
    console.log(this.isSearchBarActive);

  }

  toggleFilter() {
    this.filterStatusChange.emit();
  }

  performSearch() {
    this.searchTerm.emit(this.searchInput);
    this.searchInput = '';
  }

  signOut() {
    this.router.navigate(['Account/Login']);
    this.userService.removeToken();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
}

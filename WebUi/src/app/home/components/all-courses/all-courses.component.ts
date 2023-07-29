import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder,FormGroup } from '@angular/forms';
import { HttpResponse } from '@angular/common/http';
import { SubSink } from 'subsink';

import { CourseService } from 'src/app/services/course.service';
import { AdminService } from 'src/app/services/admin.service';
import { ISelect } from 'src/app/shared/models/ISelect';
import { CartService } from 'src/app/services/cart.service';
import { ICoursesWithStatusDto } from '../../models/ICoursesWithStatusDto';
import { ICoursesWithDetailDto } from '../../models/ICoursesWithDetailDto';
import { ActivatedRoute } from '@angular/router';
import { ICourse } from '../../models/ICourse';

@Component({
  selector: 'app-all-courses',
  templateUrl: './all-courses.component.html',
  styleUrls: ['./all-courses.component.css'],
})
export class AllCoursesComponent implements OnInit, OnDestroy {

  private subs = new SubSink();
  isLoading: boolean = false;
  courses: ICoursesWithStatusDto[] = [];
  coursesForUser: ICourse[] = [];
  totalPages!: number;
  currentPage: number = 1;
  PageSize: number = 7;
  filterStatus: boolean = false
  countItemsOfCart!: number;
  onlyUserCourses: boolean = false;

  // Dropdown options
  languages!: ISelect[];
  instructors!: ISelect[];
  levels!: ISelect[];
  categories!: ISelect[];

  // Form for filtering courses
  filterForm: FormGroup = this.fb.group({
    levelId: [-1],
    categoryId: [-1],
    languageId: [-1],
    instructorId: [-1],
    minPrice: [null],
    maxPrice: [null],
  });

  constructor(
    private courseService: CourseService,
    private adminService: AdminService,
    private cartService: CartService,
    private route: ActivatedRoute,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {

    // Load courses and dropdown options
    this.loadCourses();
    this.loadDropdownOptions();
    // Load count of cart items
    this.loadCountOfCartItems();
    // Subscribe to changes in filter form values
    this.filterForm.valueChanges.subscribe(_ => {
      this.currentPage = 1;
      this.loadCourses()
    });

    this.route.queryParamMap.subscribe(params => {
      this.onlyUserCourses = Boolean(params.get('MyCourses'));
      this.currentPage = 1;
      this.loadCourses()
    });
  }

  // Load all courses
  loadCourses(pageNumber: number = this.currentPage, pageSize: number = this.PageSize, name: string = '',
    removeAllPages: boolean = true): void {
    this.isLoading = true;
    const languageId = this.filterForm.get('languageId')?.value == -1 ? undefined : this.filterForm.get('languageId')?.value;
    const levelId = this.filterForm.get('levelId')?.value == -1 ? undefined : this.filterForm.get('levelId')?.value;
    const categoryId = this.filterForm.get('categoryId')?.value == -1 ? undefined : this.filterForm.get('categoryId')?.value;
    const instructorId = this.filterForm.get('instructorId')?.value == -1 ? undefined : this.filterForm.get('instructorId')?.value;
    const maxPrice = this.filterForm.get('maxPrice')?.value;
    const minPrice = this.filterForm.get('minPrice')?.value;

    this.subs.sink = this.courseService.getCourses(pageNumber, pageSize, categoryId, languageId, instructorId, levelId, name, this.onlyUserCourses, maxPrice, minPrice).subscribe({
      next: (result: HttpResponse<ICoursesWithStatusDto[]>) => {
        if (removeAllPages) {
          this.courses = []; // remove all pages from the array
        }
        // add the new page to the array
        this.courses = [...this.courses, ...result.body!]
        // Get the total number of pages
        const paginationData = JSON.parse(result.headers.get('x-pagination')!);
        console.table(paginationData);
        this.totalPages = paginationData.TotalPageCount
        this.currentPage = paginationData.CurrentPage;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });

  }


  // Load dropdown options
  loadDropdownOptions(): void {
    // Load levels dropdown options
    this.adminService.Levels.subscribe({
      next: (result) => {
        this.levels = result.map(r => ({ key: r.levelId, value: r.name }));
      }
    });

    // Load categories dropdown options
    this.courseService.getCategories(0).subscribe({
      next: (result) => {
        this.categories = result.map(r => ({ key: r.categoryId, value: r.nameCategory }));
      }
    });

    // Load instructors dropdown options
    this.adminService.Instructors.subscribe({
      next: (result) => {
        this.instructors = result.map(r => ({ key: r.instructorId, value: r.name }));
      }
    });

    // Load languages dropdown options
    this.adminService.Languages.subscribe({
      next: (result) => {
        this.languages = result.map(r => ({ key: r.languageId, value: r.name }));
      }
    });
  }

  loadCountOfCartItems(): void {
    this.subs.sink = this.cartService.getCountItems.subscribe({
      next: (number) => {
        this.countItemsOfCart = number;
      }
    });
  }

  onSearch(searchInput: string) {
    this.currentPage = 1;
    this.loadCourses(this.currentPage, this.PageSize, searchInput);
  }

  toggleFilter() {
    this.filterStatus = !this.filterStatus;
    console.log(this.filterStatus);
  }

  // Add the next page to current page
  getNextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadCourses(this.currentPage, this.PageSize, '', false);
    }
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

}
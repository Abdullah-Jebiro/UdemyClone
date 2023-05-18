import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { InstructorService } from 'src/app/services/instructor.service';
import { SubSink } from 'subsink';
import { ICourseForInstructorDto } from '../../models/ICourseForInstructorDto';
import { environment } from 'src/environments/environment';
import { HttpResponse } from '@angular/common/http';
import { ActivatedRoute, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-table-course',
  templateUrl: './table-course.component.html',
  styleUrls: ['./table-course.component.css']
})
export class TableCourseComponent implements OnInit {

  private subs = new SubSink();
  imageBaseUrl: string = environment.imageEndpoint;
  Courses!: ICourseForInstructorDto[]
  coursesFilter!: ICourseForInstructorDto[];
  currentPage: number = 1;
  totalItems!: number;

  constructor(
    private toastrService: ToastrService,
    private instructorService: InstructorService,
    private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      this.currentPage = Number(params.get('currentPage'));
      if (!this.currentPage) {
        this.currentPage = 1;
      }
      this.pageChanged(this.currentPage);
    });
  }

  onSearch(searchInput: string): void {
    this.coursesFilter = this.Courses.filter(course => {
      return course.name.toLocaleLowerCase().includes(searchInput.toLocaleLowerCase());
    });
  }

  deleteCourse(courseId: number) {
    console.log(courseId);
    this.subs.sink = this.instructorService.deleteCourse(courseId).subscribe({
      next: (message) => {
        const index = this.Courses.findIndex(course => course.courseId === courseId);
        this.Courses.splice(index, 1);
        this.toastrService.success(message, 'successfully', { timeOut: 5000 });
      },
      error: (err) => {
        console.table(err);

        if (err.status == 400) {
          this.toastrService.info(err.error.Message, ' ', { timeOut: 15000 });
        }
        else
          this.toastrService.error('Please try again.', 'Error', { timeOut: 5000 });
      },
    });
  }



  pageChanged(page: number) {
    this.currentPage = page;
    this.subs.sink = this.instructorService.getCourses(page, 3).subscribe({
      next: (result: HttpResponse<ICourseForInstructorDto[]>) => {
        this.Courses = result.body!;
        this.coursesFilter = result.body!;
        const paginationData = JSON.parse(result.headers.get('x-pagination')!);
        this.totalItems = paginationData.TotalItemCount
        this.currentPage = paginationData.CurrentPage;
      },
      error: (err) => {
      },
    });
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}

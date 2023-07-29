import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  MinValidator,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { SubSink } from 'subsink';
import { ToastrService } from 'ngx-toastr';
import { AdminService } from 'src/app/services/admin.service';
import { ISelect } from 'src/app/shared/models/ISelect';
import { CourseService } from 'src/app/services/course.service';
import { InstructorService } from 'src/app/services/instructor.service';
import { min } from 'rxjs';

@Component({
  selector: 'app-create-course',
  templateUrl: './create-course.component.html',
  styleUrls: ['./create-course.component.css'],
})
export class CreateCourseComponent implements OnInit {
  isLoading: boolean = false;
  private subs = new SubSink();
  languages!: ISelect[];
  levels!: ISelect[];
  Categories!: ISelect[];

  constructor(
    private adminService: AdminService,
    private courseService: CourseService,
    private instructorService: InstructorService,
    private router: Router,
    private fb: FormBuilder,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.adminService.Levels.subscribe({
      next: (result) => {
        this.levels = result.map((r) => ({ key: r.levelId, value: r.name }));
      },
      error: (err) => {
        console.log(err);
      },
    });
    this.adminService.Languages.subscribe({
      next: (result) => {
        this.languages = result.map((r) => ({
          key: r.languageId,
          value: r.name,
        }));
      },
      error: (err) => {
        console.log(err);
        this.toastrService.error('', err, {
          timeOut: 3000,
        });
      },
    });
    this.courseService.getCategories(0).subscribe({
      next: (result) => {
        this.Categories = result.map((r) => ({
          key: r.categoryId,
          value: r.nameCategory,
        }));
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  createCourseForm: FormGroup = this.fb.group({
    name: [null, [Validators.required, Validators.minLength(3)]],
    description: [null, [Validators.required, Validators.minLength(3)]],
    price: [null, [Validators.required, Validators.pattern(/^[0-9]*$/)]],
    about: [null, [Validators.required, Validators.minLength(3)]],
    thumbnailUrl: [
      null,
      [Validators.required, Validators.pattern(/\.(jpg|jpeg|png|gif|webp)$/i)],
    ],
    levelId: [-1, [Validators.required, Validators.min(1)]],
    categoryId: [-1, [Validators.required, Validators.min(1)]],
    languageId: [-1, [Validators.required, Validators.min(1)]],
  });

  onSubmit() {
    this.isLoading = true;
    this.subs.sink = this.instructorService
      .createCourse(this.createCourseForm.value)
      .subscribe({
        next: (result) => {
          console.table(result);
          this.router.navigate(['/Instructor/Course/Table']);
          this.toastrService.success('done successfully', '', {
            timeOut: 3000,
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

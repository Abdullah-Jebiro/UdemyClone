import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CourseService } from 'src/app/services/course.service';
import { InstructorService } from 'src/app/services/instructor.service';
import { ISelect } from 'src/app/shared/models/ISelect';
import { SubSink } from 'subsink';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-update-course',
  templateUrl: './update-course.component.html',
  styleUrls: ['./update-course.component.css']
})
export class UpdateCourseComponent implements OnInit, OnDestroy {

  isLoading: boolean = false;
  private subs = new SubSink();
  languages!: ISelect[];
  levels!: ISelect[];
  Categories!: ISelect[];
  courseId!: number;
  oldThumbnailUrl!: string;

  constructor(
    private courseService: CourseService,
    private instructorService: InstructorService,
    private adminService: AdminService,
    private router: Router,
    private fb: FormBuilder,
    private toastrService: ToastrService,
    private route: ActivatedRoute
  ) { }


  updateCourseForm: FormGroup = this.fb.group(
    {
      name: new FormControl(null, [Validators.required, Validators.minLength(3)]),
      description: new FormControl(null, [Validators.required, Validators.minLength(3)]),
      price: new FormControl(null, [Validators.required, Validators.pattern(/^[0-9]*$/)]),
      about: new FormControl(null, [Validators.required, Validators.minLength(3)]),
      thumbnailUrl: new FormControl(null, [Validators.required, Validators.pattern(/\.(jpg|jpeg|png|gif|webp)$/i)]),
      levelId: new FormControl(null, [Validators.required]),
      categoryId: new FormControl(null, [Validators.required]),
      languageId: new FormControl(null, [Validators.required]),
    },
  );


  ngOnInit(): void {

    this.adminService.Levels.subscribe({
      next: (result) => {
        this.levels = result.map(r => ({ key: r.levelId, value: r.name }));
      }
    });
    this.adminService.Languages.subscribe({
      next: (result) => {
        this.languages = result.map(r => ({ key: r.languageId, value: r.name }));
      }
    });
    this.courseService.getCategories(0).subscribe({
      next: (result) => {
        this.Categories = result.map(r => ({ key: r.categoryId, value: r.nameCategory }));
      }
    });

    this.courseId = Number(this.route.snapshot.paramMap.get('courseId'));

    if (!isNaN(this.courseId)) {
      this.subs.sink = this.instructorService.getCourse(this.courseId).subscribe({
        next: (data) => {
          this.updateCourseForm.patchValue({
            name: data.name,
            description: data.description,
            price: data.price,
            about: data.about,
            languageId: data.languageId,
            categoryId: data.categoryId,
            levelId: data.levelId,
            thumbnailUrl: data.thumbnailUrl
          });
          this.oldThumbnailUrl = data.thumbnailUrl;
        },
        error: (err) => {
          console.log(err);
        },
      });
    }
  }


 

  onSubmit() {
    this.isLoading = true;
    this.subs.sink = this.instructorService.updateCourse(this.updateCourseForm.value, this.courseId).subscribe({
      next: (result) => {
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




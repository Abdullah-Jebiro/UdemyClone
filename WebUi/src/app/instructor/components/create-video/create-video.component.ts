import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { InstructorService } from 'src/app/services/instructor.service';
import { SubSink } from 'subsink';

@Component({
  selector: 'app-create-video',
  templateUrl: './create-video.component.html',
  styleUrls: ['./create-video.component.css']
})
export class CreateVideoComponent {

  isLoading: boolean = false;
  courseId = Number(this.route.snapshot.paramMap.get('CourseId'));
  private subs = new SubSink();

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private instructorService: InstructorService,
    private toastrService: ToastrService
  ) { }


  createVideoForm: FormGroup = this.fb.group(
    {
      VideoTitle: new FormControl(null, [Validators.required, Validators.minLength(3)]),
      videoUrl: new FormControl(null, [Validators.required, Validators.pattern(/\.(mp4|avi|mov)$/i)]),
      courseId: new FormControl(this.courseId, [Validators.required]),
    },
  );

  onSubmit() {
    this.isLoading = true;
    this.subs.sink = this.instructorService.createVideo(this.createVideoForm.value).subscribe({
      next: (result) => {
        this.toastrService.success('done successfully', '', {
          timeOut: 3000,
        });
        this.isLoading = false;
        this.createVideoForm.reset();
        this.createVideoForm.patchValue({
          courseId: this.courseId
        })
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




import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { InstructorService } from 'src/app/services/instructor.service';
import { SubSink } from 'subsink';

@Component({
  selector: 'app-update-video',
  templateUrl: './update-video.component.html',
  styleUrls: ['./update-video.component.css']
})
export class UpdateVideoComponent implements OnInit {

  isLoading: boolean = false;
  courseId = Number(this.route.snapshot.paramMap.get('CourseId'));
  videoId = Number(this.route.snapshot.paramMap.get('VideoId'));
  oldVideoUrl!: string;
  private subs = new SubSink();

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private instructorService: InstructorService,
    private toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    if (!isNaN(this.courseId)) {
      this.subs.sink = this.instructorService.getVideo(this.courseId, this.videoId).subscribe({
        next: (data) => {
          this.updateVideoForm.patchValue({
            VideoTitle: data.videoTitle,
            videoUrl: data.videoUrl,
          });
          this.oldVideoUrl = data.videoUrl;
        },
        error: (err) => {
          console.log(err);
        },
      });
    }
  }

  updateVideoForm: FormGroup = this.fb.group(
    {
      VideoTitle: new FormControl(null, [Validators.required, Validators.minLength(3)]),
      videoUrl: new FormControl(null, [Validators.required, Validators.pattern(/\.(mp4|avi|mov)$/i)]),
    },
  );

  onSubmit() {
    this.isLoading = true;

    this.subs.sink = this.instructorService.updateVideo(this.updateVideoForm.value, this.courseId, this.videoId).subscribe({
      next: (result) => {
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




import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { InstructorService } from 'src/app/services/instructor.service';
import { SubSink } from 'subsink';
import { ICourseForInstructorDto } from '../../models/ICourseForInstructorDto';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseService } from 'src/app/services/course.service';
import { environment } from 'src/environments/environment';
import { IVideoDto } from '../../../home/models/IVideoDto';


@Component({
  selector: 'app-table-video',
  templateUrl: './table-video.component.html',
  styleUrls: ['./table-video.component.css']
})
export class TableVideoComponent {


  videos!: IVideoDto[];
  activeVideo: number = 0;
  videoUrl: string = '';
  courseId!: number;
  private subs = new SubSink();
  videoBaseUrl: string = environment.videoEndpoint;

  constructor(
    private courseService: CourseService,
    private instructorService: InstructorService,
    private route: ActivatedRoute,
    private router: Router,
    private toastrService: ToastrService
  ) { }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  ngOnInit(): void {
    this.courseId = Number(this.route.snapshot.paramMap.get('CourseId'));

    if (!isNaN(this.courseId)) {  //If there is an error in the user number, return false
      this.subs.sink = this.courseService.getVideos(this.courseId).subscribe({
        next: (data) => {
          this.videos = data;
          this.videoUrl = this.videos[0].videoUrl
          this.activeVideo = this.videos[0].videoId;
        },
        error: (err) => {
          this.router.navigate(['/Home']);
        },
      });
    }
  }


  deleteVideo(videoId: number) {
    this.subs.sink = this.instructorService.deleteVideo(this.courseId, videoId).subscribe({
      next: (data) => {
        const index = this.videos.findIndex(video => video.videoId === videoId);
        this.videos.splice(index, 1);
        this.toastrService.success('The video has been deleted successfully', 'successfully', { timeOut: 5000 });
      },
      error: (err) => {
        this.toastrService.error('Please try again.', 'Error', { timeOut: 5000 });
      },
    });
  }

  play(videoUrl: string, Id: number): void {
    this.videoUrl = videoUrl;
    this.activeVideo = Id;
  }
}







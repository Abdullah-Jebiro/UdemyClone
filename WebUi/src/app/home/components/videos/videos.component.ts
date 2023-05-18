import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseService } from 'src/app/services/course.service';
import { environment } from 'src/environments/environment';
import { SubSink } from 'subsink';
import { IVideoDto } from '../../models/IVideoDto';

@Component({
  selector: 'app-videos',
  templateUrl: './videos.component.html',
  styleUrls: ['./videos.component.css'],
})
export class VideosComponent {

  videos!: IVideoDto[];
  activeVideo: number = 1;
  videoUrl: string = '';
  private subs = new SubSink();
  videoBaseUrl: string = environment.videoEndpoint;

  constructor(
    private courseService: CourseService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  ngOnInit(): void {
    let id = Number(this.route.snapshot.paramMap.get('id'));

    if (!isNaN(id)) {  //If there is an error in the user number, return false
      this.subs.sink = this.courseService.getVideos(id).subscribe({
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

  play(videoUrl: string, Id: number): void {
    this.videoUrl = videoUrl;
    this.activeVideo = Id;
    console.log(Id);
    console.log(this.activeVideo);
  }
}

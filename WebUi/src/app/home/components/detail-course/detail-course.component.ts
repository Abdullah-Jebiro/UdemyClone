import { Component } from '@angular/core';
import { CourseService } from 'src/app/services/course.service';
import { SubSink } from 'subsink';
import { environment } from 'src/environments/environment';
import { ActivatedRoute } from '@angular/router';
import { IVideoInfoDto } from 'src/app/instructor/models/IVideoInfoDto';
import { ICoursesWithDetailDto } from '../../models/ICoursesWithDetailDto';
import { CartService } from 'src/app/services/cart.service';

@Component({
  selector: 'app-detail-course',
  templateUrl: './detail-course.component.html',
  styleUrls: ['./detail-course.component.css'],
})
export class DetailCourseComponent {
  constructor(
    private courseService: CourseService,
    private route: ActivatedRoute,
    private cartService: CartService
  ) { }
  Course!: ICoursesWithDetailDto;
  Videos!: IVideoInfoDto[];
  private subs = new SubSink();
  imageBaseUrl: string = environment.imageEndpoint;

  ngOnInit(): void {
    let id = Number(this.route.snapshot.paramMap.get('id'));
    this.subs.sink = this.courseService.getCourse(id).subscribe({
      next: (data) => {
        this.Course = data;
      },
      error: (err) => console.log(err),
    });
    this.subs.sink = this.courseService.getInfoVideos(id).subscribe({
      next: (data) => {
        this.Videos = data;
      },
      error: (err) => console.log(err),
    });
  }

  addToCart(courseId: number) {
    this.subs.sink = this.cartService.addToCart(courseId).subscribe({
      next: (data) => {
        this.Course.status = "InCart"
      },
      error: (err) => console.log(err),
    });
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}

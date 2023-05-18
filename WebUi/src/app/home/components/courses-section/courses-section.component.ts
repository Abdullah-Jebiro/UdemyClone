import { Component, Input } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SubSink } from 'subsink';
import { CartService } from 'src/app/services/cart.service';
import { ICoursesWithStatusDto } from '../../models/ICoursesWithStatusDto';

@Component({
  selector: 'app-courses-section',
  templateUrl: './courses-section.component.html',
  styleUrls: ['./courses-section.component.css']
})
export class CoursesSectionComponent {

  @Input() courses!: ICoursesWithStatusDto[];
  private subs = new SubSink();

  imageBaseUrl: string = environment.imageEndpoint;

  constructor(private cartService: CartService) { }

  addToCart(courseId: number): void {
    this.subs.add(
      this.cartService.addToCart(courseId)
        .subscribe({
          next: () => {
            this.courses.find(c => c.courseId === courseId)!.status = 'InCart';
          },
          error: (err) => console.error(err)
        })
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}


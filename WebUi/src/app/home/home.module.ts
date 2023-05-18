import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CoursesSectionComponent } from './components/courses-section/courses-section.component';
import { SharedModule } from '../shared/shared.module';
import { AllCoursesComponent } from './components/all-courses/all-courses.component';
import { DetailCourseComponent } from './components/detail-course/detail-course.component';
import { VideosComponent } from './components/videos/videos.component';


@NgModule({
  declarations: [
    AllCoursesComponent,
    CoursesSectionComponent,
    AllCoursesComponent,
    VideosComponent,
    DetailCourseComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([
      { path: '', component: AllCoursesComponent },
      { path: 'Detail/:id', component: DetailCourseComponent },
      { path: 'Course/:id', component: VideosComponent },

      { path: '**', redirectTo: '', pathMatch: 'full' }
    ])
  ]
})
export class HomeModule { }

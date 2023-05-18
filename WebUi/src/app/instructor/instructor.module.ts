import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InstructorUpdateComponent } from './components/instructor-update/instructor-update.component';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { CreateCourseComponent } from './components/create-course/create-course.component';
import { TableCourseComponent } from './components/table-course/table-course.component';
import { CreateVideoComponent } from './components/create-video/create-video.component';
import { UpdateCourseComponent } from './components/update-course/update-course.component';
import { TableVideoComponent } from './components/table-video/table-video.component';
import { UpdateVideoComponent } from './components/update-video/update-video.component';



@NgModule({
  declarations: [
    InstructorUpdateComponent,
    CreateCourseComponent,
    TableCourseComponent,
    CreateVideoComponent,
    UpdateCourseComponent,
    TableVideoComponent,
    UpdateVideoComponent
  ],
  imports: [
    CommonModule,
    SharedModule,

    RouterModule.forChild([

      {
        path: 'Update', component: InstructorUpdateComponent,
      },
      {
        path: 'Course', component: TableCourseComponent,
      },
      {
        path: 'Course/:CourseId/Videos/Create', component: CreateVideoComponent,
      },
      {
        path: 'Course/:CourseId/Videos/Update/:VideoId', component: UpdateVideoComponent,
      },
      {
        path: 'Course/:CourseId/Videos', component: TableVideoComponent,
      },
      {
        path: 'Course/Update/:courseId', component: UpdateCourseComponent,
      },
      {
        path: 'Course/Create', component: CreateCourseComponent,
      },
      { path: '', redirectTo: 'Course', pathMatch: 'full' },
      { path: '**', redirectTo: 'Course', pathMatch: 'full' }
    ])
  ]
})
export class InstructorModule { }

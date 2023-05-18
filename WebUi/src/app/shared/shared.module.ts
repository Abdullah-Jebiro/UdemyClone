import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileUploadComponent } from './components/file-upload/file-upload.component';
import { SelectComponent } from './components/select/select.component';
import { ZeroToFreePipe } from './Pipe/zero-to-free.pipe';
import { HeaderComponent } from './components/header/header.component';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { IsAdminDirective } from './directives/is-admin.directive';

@NgModule({
  declarations: [
    SelectComponent,
    FileUploadComponent,
    ZeroToFreePipe,
    HeaderComponent,
    IsAdminDirective
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule
  ],
  exports: [
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    SelectComponent,
    FileUploadComponent,
    ZeroToFreePipe,
    HeaderComponent,
    NgxPaginationModule
  ],
})
export class SharedModule { }

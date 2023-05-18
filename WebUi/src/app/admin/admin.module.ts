import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { CreateCategoryComponent } from './components/create-category/create-category.component';
import { TableLevelComponent } from './components/table-level/table-level.component';
import { UpdateCategoryComponent } from './components/update-category/update-category.component';
import { TableCategoryComponent } from './components/table-category/table-category.component';
import { UpdateLevelComponent } from './components/update-level/update-level.component';
import { UpdateLanguageComponent } from './components/update-language/update-language.component';
import { CreateLevelComponent } from './components/create-level/create-level.component';
import { CreateLanguageComponent } from './components/create-language/create-language.component';
import { TableLanguageComponent } from './components/table-language/table-language.component';

@NgModule({
  declarations: [
    CreateCategoryComponent,
    UpdateCategoryComponent,
    TableCategoryComponent,
    TableLevelComponent,
    UpdateLevelComponent,
    CreateLevelComponent,
    UpdateLanguageComponent,
    CreateLanguageComponent,
    TableLanguageComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([
      { path: 'Language', component: TableLanguageComponent },
      { path: 'Level', component: TableLevelComponent },
      { path: 'Category', component: TableCategoryComponent },
    ])
  ]
})
export class AdminModule { }

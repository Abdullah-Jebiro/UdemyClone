import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableLanguageComponent } from './table-language.component';

describe('TableLanguageComponent', () => {
  let component: TableLanguageComponent;
  let fixture: ComponentFixture<TableLanguageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableLanguageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TableLanguageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

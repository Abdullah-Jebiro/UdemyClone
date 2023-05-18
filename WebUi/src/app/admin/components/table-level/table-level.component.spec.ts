import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableLevelComponent } from './table-level.component';

describe('TableLevelComponent', () => {
  let component: TableLevelComponent;
  let fixture: ComponentFixture<TableLevelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableLevelComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TableLevelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

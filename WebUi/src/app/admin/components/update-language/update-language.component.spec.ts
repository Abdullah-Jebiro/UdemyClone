import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateLanguageComponent } from './update-language.component';

describe('UpdateLanguageComponent', () => {
  let component: UpdateLanguageComponent;
  let fixture: ComponentFixture<UpdateLanguageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateLanguageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateLanguageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

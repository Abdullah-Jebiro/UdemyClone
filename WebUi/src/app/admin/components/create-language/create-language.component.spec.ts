import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateLanguageComponent } from './create-language.component';

describe('CreateLanguageComponent', () => {
  let component: CreateLanguageComponent;
  let fixture: ComponentFixture<CreateLanguageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateLanguageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateLanguageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

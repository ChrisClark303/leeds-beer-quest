import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FindMeBeerComponent } from './find-me-beer.component';

describe('FindMeBeerComponent', () => {
  let component: FindMeBeerComponent;
  let fixture: ComponentFixture<FindMeBeerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FindMeBeerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FindMeBeerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

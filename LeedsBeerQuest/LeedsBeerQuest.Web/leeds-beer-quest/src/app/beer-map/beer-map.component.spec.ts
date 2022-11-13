import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BeerMapComponent } from './beer-map.component';

describe('BeerMapComponent', () => {
  let component: BeerMapComponent;
  let fixture: ComponentFixture<BeerMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BeerMapComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BeerMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

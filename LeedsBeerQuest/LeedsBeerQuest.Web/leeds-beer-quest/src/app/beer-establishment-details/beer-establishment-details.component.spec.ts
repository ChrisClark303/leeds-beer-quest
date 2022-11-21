import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BeerEstablishmentDetailsComponent } from './beer-establishment-details.component';

describe('BeerEstablishmentDetailsComponent', () => {
  let component: BeerEstablishmentDetailsComponent;
  let fixture: ComponentFixture<BeerEstablishmentDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BeerEstablishmentDetailsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BeerEstablishmentDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

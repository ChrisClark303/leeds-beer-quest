import { TestBed } from '@angular/core/testing';

import { BeerQuestService } from './beer-quest.service';

describe('BeerQuestService', () => {
  let service: BeerQuestService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BeerQuestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

import { Component, OnInit, Input,Output } from '@angular/core';
import { BeerEstablishment } from '../beer-establishment';
import { BeerQuestService } from '../beer-quest.service';

@Component({
  selector: 'app-find-me-beer',
  templateUrl: './find-me-beer.component.html',
  styleUrls: ['./find-me-beer.component.css']
})
export class FindMeBeerComponent implements OnInit {

  @Output() selectedEstablishment: BeerEstablishment;

  constructor(private beerQuestService:BeerQuestService) { }

  ngOnInit(): void {
  }

  getSelectedEstablishmentDetails(establishmentName:string)
  {
        this.beerQuestService.getEstablishmentDetailsByName(establishmentName)
          .subscribe(result => {
            this.selectedEstablishment = result;
        });
  }

}

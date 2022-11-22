import { Component, Input, OnInit } from '@angular/core';
import { BeerEstablishment } from '../beer-establishment';

@Component({
  selector: 'app-beer-establishment-details',
  templateUrl: './beer-establishment-details.component.html',
  styleUrls: ['./beer-establishment-details.component.css']
})
export class BeerEstablishmentDetailsComponent implements OnInit {

  name:String; 
  @Input() selectedEstablishment: BeerEstablishment;

  constructor() { }

  ngOnInit(): void {
  }

}

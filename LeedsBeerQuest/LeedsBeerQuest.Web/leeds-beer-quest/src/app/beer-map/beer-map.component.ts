import { Component, OnInit } from '@angular/core';
import { GoogleMapsModule, MapMarker } from '@angular/google-maps'

@Component({
  selector: 'app-beer-map',
  templateUrl: './beer-map.component.html',
  styleUrls: ['./beer-map.component.css']
})
export class BeerMapComponent implements OnInit {

  markers:Array<any> = new Array<any>();

  constructor() { }

  ngOnInit(): void {
    this.addMarker();
  }

  addMarker() {
    this.markers.push({
      position: {
        lat: 53.801196991070164,
        lng: -1.555570961376415
      },
      label: {
        color: 'white',
        text: 'Joseph\'s Well',
      },
      title: 'Joseph\'s Well',
      options: { animation: google.maps.Animation.DROP },
    });
  }

}

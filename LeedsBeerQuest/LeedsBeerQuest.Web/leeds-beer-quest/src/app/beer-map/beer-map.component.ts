import { Component, OnInit, ViewChild } from '@angular/core';
import { GoogleMap, MapInfoWindow, MapMarker } from '@angular/google-maps'
import { BeerEstablishmentLocation } from '../beer-establishment-location';
import { BeerQuestService } from '../beer-quest.service';

@Component({
  selector: 'app-beer-map',
  templateUrl: './beer-map.component.html',
  styleUrls: ['./beer-map.component.css']
})
export class BeerMapComponent implements OnInit {

  @ViewChild(GoogleMap, { static: false }) map: GoogleMap;
  @ViewChild(MapInfoWindow, { static: false }) info: MapInfoWindow;
  markers:Array<any> = new Array<any>();
  center: google.maps.LatLngLiteral;
  infoContent: '';

  constructor(private beerQuestService:BeerQuestService) { }

  ngOnInit(): void {
    this.getLocations();
    this.setMapCentre();
  }

  setMapCentre()
  {
    this.center = {
      lat: 53.801196991070164,
      lng: -1.555570961376415
    }  
  }

  getLocations()
  {
    this.beerQuestService.getLocationsNearMe()
    .subscribe(result => {
      result.forEach((bl, i) => 
        {
          console.log(JSON.stringify({ bl }) );
          this.addMarker(bl, i);
        })
    });
  }

  addMarker(location:BeerEstablishmentLocation, index:Number) {
    this.markers.push({
      position: {
        lat: location.location.lat,
        lng: location.location.long
      },
      label: {
        color: 'white',
        text: `${index}: ${location.name} (${location.distance} mls)`,
      },
      title: location.name,
      info: location.name,
      options: { animation: google.maps.Animation.DROP },
    });
  }

  showMarkerInformation(marker: MapMarker, content:any) {
    console.log(content);
    this.infoContent = content;
    this.info.open(marker);
  }

}
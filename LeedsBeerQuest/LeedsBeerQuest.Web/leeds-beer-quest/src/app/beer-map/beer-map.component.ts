import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { GoogleMap, MapInfoWindow, MapMarker } from '@angular/google-maps'
import { BeerEstablishment } from '../beer-establishment';
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

  @Output() selectedEstablishment: BeerEstablishment;
  @Output() establishmentSelected = new EventEmitter<string>();

  constructor(private beerQuestService:BeerQuestService) { }

  ngOnInit(): void {
    this.setMapCentre();
    this.AddLocationsToMap();
  }

  setMapCentre()
  {
    this.center = {
      lat: 53.801196991070164,
      lng: -1.555570961376415
    }
    this.addMarkerToMap("Your starting position", this.center.lat, this.center.lng, "Your starting position");  
  }

  AddLocationsToMap()
  {
    this.beerQuestService.getEstablishmentsNearMe()
    .subscribe(result => {
      result.forEach((bl, i) => 
        {
          console.log(JSON.stringify({ bl }) );
          this.addMarkerToMap(bl.name, bl.location.lat, bl.location.long, `${i}: ${bl.name} (${bl.distance} mls)`);
        })
    });
  }

  addMarkerToMap(title:String, lat:Number, lng:Number, labelText:String) {
    this.markers.push({
      position: {
        lat: lat,
        lng: lng
      },
      label: {
        color: 'black',
        text: labelText,
      },
      title: title,
      info: title,
      options: { animation: google.maps.Animation.DROP },
    });
  }

  showMarkerInformation(marker: MapMarker, content:any) {
    console.log("Content: " + content);
    // this.beerQuestService.getEstablishmentDetailsByName(content)
    // .subscribe(result => {
    //   this.selectedEstablishment = result;
    //   console.log("establishment: " + result.name)
    // });
    this.establishmentSelected.emit(content);
    this.infoContent = content;
    this.info.open(marker);
  }
}

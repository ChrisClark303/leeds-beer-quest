import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { BeerEstablishmentLocation } from './beer-establishment-location';
import { BeerEstablishment } from './beer-establishment';

@Injectable({
  providedIn: 'root'
})
export class BeerQuestService {

  //private serviceUrl: string = 'https://leedsbeerquestapi.azurewebsites.net/';
  private serviceUrl: string = 'http://localhost:5174';

  constructor(private httpClient: HttpClient) { }

  protected get<T>(url:string): Observable<T> {
      var absUrl = `${this.serviceUrl}${url}`;
      console.log("Sending request to " + absUrl)
      return this.httpClient.get<T>(absUrl);
    }

    getEstablishmentsNearMe(): Observable<BeerEstablishmentLocation[]> {
      return this.get<BeerEstablishmentLocation[]>("/beer/nearest-establishments");
    }

    getEstablishmentDetailsByName(establishmentName:String) {
      return this.get<BeerEstablishment>(`/beer/${establishmentName}`);
    }
}

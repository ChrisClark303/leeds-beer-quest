import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { BeerEstablishmentLocation } from './beer-establishment-location';

@Injectable({
  providedIn: 'root'
})
export class BeerQuestService {

  private serviceUrl: string = 'http://localhost:5174';

  constructor(private httpClient: HttpClient) { }

  protected get<T>(url:string): Observable<T[]> {
      var absUrl = `${this.serviceUrl}${url}`;
      console.log("Sending request to " + absUrl)
      return this.httpClient.get<T[]>(absUrl);
    }

    getLocationsNearMe(): Observable<BeerEstablishmentLocation[]> {
      return this.get<BeerEstablishmentLocation>("/beer/nearest-locations");
    }
}

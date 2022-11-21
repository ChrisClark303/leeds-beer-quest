import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { GoogleMapsModule } from '@angular/google-maps'
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BeerMapComponent } from './beer-map/beer-map.component';
import { HttpClientModule } from '@angular/common/http';
import { BeerEstablishmentDetailsComponent } from './beer-establishment-details/beer-establishment-details.component';
import { FormsModule } from '@angular/forms';
import { FindMeBeerComponent } from './find-me-beer/find-me-beer.component';

@NgModule({
  declarations: [
    AppComponent,
    BeerMapComponent,
    BeerEstablishmentDetailsComponent,
    FindMeBeerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    GoogleMapsModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

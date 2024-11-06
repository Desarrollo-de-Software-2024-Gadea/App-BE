import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SeriesComponent } from './series/series.component';
import { SharedModule } from '../shared/shared.module';
import { SerieRoutingModule } from './serie-routing.module';



@NgModule({
  declarations: [SeriesComponent],
  imports: [
    CommonModule, SharedModule, SerieRoutingModule
  ]
})
export class SerieModule { }

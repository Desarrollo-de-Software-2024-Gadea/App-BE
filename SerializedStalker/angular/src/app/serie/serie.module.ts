import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SeriesComponent } from './series/series.component';
import { SeriesPersistidasComponent } from './series-persistidas/series-persistidas.component'; 
import { SharedModule } from '../shared/shared.module';
import { SerieRoutingModule } from './serie-routing.module';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

@NgModule({
  declarations: [
    SeriesComponent,
    SeriesPersistidasComponent // Declarar el nuevo componente
  ],
  imports: [
    CommonModule,
    SharedModule,
    SerieRoutingModule,
    NgxDatatableModule
  ]
})
export class SerieModule { }
import { Component } from '@angular/core';
import { SerieDto, SerieService } from '@proxy/series';

@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrl: './series.component.scss'
})
export class SeriesComponent {
  series = [] as SerieDto[];

  serieTitle: string = "";

  constructor(private serieService: SerieService) {

  }

  public searchSeries() {
    if(this.serieTitle.trim()) {
      this.serieService.search(this.serieTitle.trim(), "").subscribe(response => this.series = response || []);
    }
  }

}

import { Component, OnInit } from '@angular/core';
import { SerieDto, SerieService } from '@proxy/series';

@Component({
  selector: 'app-series-persistidas',
  templateUrl: './series-persistidas.component.html',
  styleUrls: ['./series-persistidas.component.scss']
})
export class SeriesPersistidasComponent implements OnInit {
  persistedSeries: SerieDto[] = [];

  constructor(private serieService: SerieService) {}

  ngOnInit(): void {
    this.loadPersistedSeries();
  }

  loadPersistedSeries() {
    this.serieService.obtenerSeries().subscribe(
      response => {
        this.persistedSeries = response || [];
        console.log('Series persistidas:', this.persistedSeries);
      },
      error => console.error('Error al cargar series persistidas:', error)
    );
  }
}
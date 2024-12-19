import { Component } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { SerieDto, SerieService } from '@proxy/series';

@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrl: './series.component.scss'
})
export class SeriesComponent {
  series = [] as SerieDto[];
  serieTitle: string = "";
  persisting: { [key: string]: boolean } = {};
  persisted: { [key: string]: boolean } = {};

  constructor(private serieService: SerieService) {}

  public searchSeries() {
    if (this.serieTitle.trim()) {
      this.serieService.buscarSerie(this.serieTitle.trim(), "").subscribe(
        response => {
          this.series = response || [];
          console.log('Series encontradas:', this.series);
          this.series.forEach(serie => {
            console.log('Serie DTO:', serie);
          });
        },
        error => console.error('Error al buscar series:', error)
      );
    }
  }

  public persistSerie(serie: SerieDto) {
    console.log('Persistiendo serie con ImdbIdentificator:', serie.imdbIdentificator);

    // Asegurarse de que los campos requeridos estén presentes
    if (!serie.temporadas) {
      serie.temporadas = []; // Inicializar como un array vacío si no está presente
    }
    if (!serie.calificaciones) {
      serie.calificaciones = []; // Inicializar como un array vacío si no está presente
    }
    if (!serie.imdbIdentificator) {
      console.error('Error: ImdbIdentificator es requerido');
      return;
    }

    console.log('Datos de la serie a persistir:', serie);

    this.persisting[serie.imdbIdentificator] = true;
    this.serieService.persistirSeries([serie]).subscribe(
      () => {
        console.log('Serie persistida con éxito:', serie.imdbIdentificator);
        this.persisting[serie.imdbIdentificator] = false;
        this.persisted[serie.imdbIdentificator] = true;
      },
      (error: HttpErrorResponse) => {
        console.error('Error al persistir serie:', error.message);
        console.error('Detalles del error:', error.error);
        this.persisting[serie.imdbIdentificator] = false;
      }
    );
  }
}
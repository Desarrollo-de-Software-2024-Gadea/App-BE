import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { SerieDto } from '../series/models';

@Injectable({
  providedIn: 'root',
})
export class ListaDeSeguimientoService {
  apiName = 'Default';
  

  addSerie = (serieDto: SerieDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/lista-de-seguimiento/serie',
      body: serieDto,
    },
    { apiName: this.apiName,...config });
  

  eliminarSerie = (ImdbID: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/lista-de-seguimiento/eliminar-serie',
      params: { imdbID: ImdbID },
    },
    { apiName: this.apiName,...config });
  

  mostrarSeries = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto[]>({
      method: 'POST',
      url: '/api/app/lista-de-seguimiento/mostrar-series',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}

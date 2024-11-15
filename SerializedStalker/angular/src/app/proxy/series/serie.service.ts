import type { CalificacionDto, CreateUpdateSerieDto, SerieDto, TemporadaDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SerieService {
  apiName = 'Default';
  

  buscarSerie = (titulo: string, genero?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto[]>({
      method: 'POST',
      url: '/api/app/serie/buscar-serie',
      params: { titulo, genero },
    },
    { apiName: this.apiName,...config });
  

  buscarTemporada = (imdbId: string, numeroTemporada: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TemporadaDto>({
      method: 'POST',
      url: `/api/app/serie/buscar-temporada/${imdbId}`,
      params: { numeroTemporada },
    },
    { apiName: this.apiName,...config });
  

  calificarSerie = (input: CalificacionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/serie/calificar-serie',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateSerieDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto>({
      method: 'POST',
      url: '/api/app/serie',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/serie/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto>({
      method: 'GET',
      url: `/api/app/serie/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<SerieDto>>({
      method: 'GET',
      url: '/api/app/serie',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  persistirSeries = (seriesDto: SerieDto[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/serie/persistir-series',
      body: seriesDto,
    },
    { apiName: this.apiName,...config });
  

  update = (id: number, input: CreateUpdateSerieDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SerieDto>({
      method: 'PUT',
      url: `/api/app/serie/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}

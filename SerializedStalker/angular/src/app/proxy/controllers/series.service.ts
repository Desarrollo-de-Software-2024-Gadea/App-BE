import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { IActionResult } from '../microsoft/asp-net-core/mvc/models';

@Injectable({
  providedIn: 'root',
})
export class SeriesService {
  apiName = 'Default';
  

  buscarSeries = (titulo: string, genero: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IActionResult>({
      method: 'GET',
      url: '/api/Series/buscar',
      params: { titulo, genero },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}

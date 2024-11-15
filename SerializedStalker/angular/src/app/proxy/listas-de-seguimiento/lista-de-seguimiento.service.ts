import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ListaDeSeguimientoService {
  apiName = 'Default';
  

  addSerie = (titulo: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/lista-de-seguimiento/serie',
      params: { titulo },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}

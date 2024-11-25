import type { MonitoreoApiDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MonitoreoApiService {
  apiName = 'Default';
  

  mostrarMonitoreos = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, MonitoreoApiDto[]>({
      method: 'POST',
      url: '/api/app/monitoreo-api/mostrar-monitoreos',
    },
    { apiName: this.apiName,...config });
  

  persistirMonitoreo = (monitoreoApiDto: MonitoreoApiDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/monitoreo-api/persistir-monitoreo',
      body: monitoreoApiDto,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}

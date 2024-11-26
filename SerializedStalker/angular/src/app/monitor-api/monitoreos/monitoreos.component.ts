import { Component, OnInit } from '@angular/core';
import { MonitoreoApiDto, MonitoreoApiService, MonitoreoApiEstadisticasDto } from '@proxy/series';

@Component({
  selector: 'app-monitoreos',
  templateUrl: './monitoreos.component.html',
  styleUrls: ['./monitoreos.component.scss']
})
export class MonitoreosComponent implements OnInit {
  monitoreos: MonitoreoApiDto[] = [];
  estadisticas: MonitoreoApiEstadisticasDto | null = null;

  constructor(private monitoreoService: MonitoreoApiService) {}

  ngOnInit(): void {
    this.mostrarMonitoreosEnBD();
    this.mostrarEstadisticasMonitoreos();
  }

  public mostrarMonitoreosEnBD(): void {    
    this.monitoreoService.mostrarMonitoreos().subscribe(response => {
      this.monitoreos = response || [];
    });
  }

  public mostrarEstadisticasMonitoreos(): void {    
    this.monitoreoService.obtenerEstadisticas().subscribe(response => {
      this.estadisticas = response || null;
    });
  }
}
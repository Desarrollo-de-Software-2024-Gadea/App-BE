import { Component } from '@angular/core';
import { MonitoreoApiDto, MonitoreoApiService } from '@proxy/series';

@Component({
  selector: 'app-monitoreos',
  templateUrl: './monitoreos.component.html',
  styleUrl: './monitoreos.component.scss'
})
export class MonitoreosComponent {
  monitoreos = [] as MonitoreoApiDto[];

  serieTitle: string = "";

  constructor(private monitoreoService: MonitoreoApiService) {

  }

  public mostrarMonitoreosEnBD() {    
      this.monitoreoService.mostrarMonitoreos().subscribe(response => this.monitoreos = response || []);
  }
}

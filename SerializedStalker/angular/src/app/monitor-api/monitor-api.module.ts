import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { MonitoreosComponent } from './monitoreos/monitoreos.component';
import { MonitorRoutingModule } from './monitor-api-routing.module';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
@NgModule({
  declarations: [MonitoreosComponent],
  imports: [
    CommonModule, 
    SharedModule, 
    MonitorRoutingModule, 
    NgxDatatableModule
  ]
})
export class MonitorApiModule { }
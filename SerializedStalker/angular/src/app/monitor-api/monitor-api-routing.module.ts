import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MonitoreosComponent } from './monitoreos/monitoreos.component';
const routes: Routes = [
    {
        path: '',
        component: MonitoreosComponent,
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class MonitorRoutingModule { }

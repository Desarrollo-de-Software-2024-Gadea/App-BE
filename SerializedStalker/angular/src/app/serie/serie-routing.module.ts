import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SeriesComponent } from './series/series.component';
import { SeriesPersistidasComponent } from './series-persistidas/series-persistidas.component'; 

const routes: Routes = [
    {
        path: '',
        component: SeriesComponent,
    },
    {
        path: 'series-persistidas',
        component: SeriesPersistidasComponent, 
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class SerieRoutingModule { }
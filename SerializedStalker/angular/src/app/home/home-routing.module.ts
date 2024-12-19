import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home.component';
import { SeriesComponent } from '../serie/series/series.component';
import { SeriesPersistidasComponent } from '../serie/series-persistidas/series-persistidas.component';
import { MonitoreosComponent } from '../monitor-api/monitoreos/monitoreos.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
  },
  {
    path: 'series',
    component: SeriesComponent,
  },
  {
    path: 'series/series-persistidas',
    component: SeriesPersistidasComponent,
  },
  {
    path: 'monitoreos',
    component: MonitoreosComponent,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})

export class HomeRoutingModule {}

import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, Routes } from '@angular/router';

import { SearchComponent } from '../components/search/search.component';
import { DetailsComponent } from '../components/details/details.component';
import { provideHttpClient, withFetch } from '@angular/common/http';

const routes: Routes = [
  { path: '', component: SearchComponent, pathMatch: 'full' }, 
  { path: 'movie/:id', component: DetailsComponent },
  { path: '**', redirectTo: '' } 
];

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withFetch())
  ]
};

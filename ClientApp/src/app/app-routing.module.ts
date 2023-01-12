import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  // default component
  { path: '', component: HomeComponent },
  { path: 'account', loadChildren: () => import('./account/account.module').then(module => module.AccountModule) },
  { path: 'play', loadChildren: () => import('./play/play.module').then(module => module.PlayModule) },
  { path: 'profile', loadChildren: () => import('./profile/profile.module').then(module => module.ProfileModule) },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

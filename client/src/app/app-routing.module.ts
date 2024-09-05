import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { authGuard } from './guards/auth.guard';
import { UsersComponent } from './pages/users/users.component';

import { roleGuard } from './guards/role.guard';

import { RolesComponent } from './pages/roles/roles.component';
import { jobOffersGuard } from './guards/job-offers.guard';
import { JobOfferComponent } from './pages/job-offer/job-offer.component';
import { ApplyJobOffersComponent } from './pages/apply-job-offers/apply-job-offers.component';
import { applyJobOffersGuard } from './guards/apply-job-offers.guard';

const routes: Routes = [
  { path: 'applyjoboffers', component: ApplyJobOffersComponent, canActivate:[applyJobOffersGuard], data: { roles:['Admin','Reclutador','Candidato'] } },
  { path: 'joboffers', component: JobOfferComponent, canActivate:[jobOffersGuard], data: { roles:['Admin','Reclutador'] } },
  { path: 'roles', component: RolesComponent, canActivate:[roleGuard], data: { roles:['Admin'] } },
  { path: 'users', component: UsersComponent, canActivate:[roleGuard], data: { roles:['Admin'] } },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: '', component: HomeComponent } // Ruta por defecto
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

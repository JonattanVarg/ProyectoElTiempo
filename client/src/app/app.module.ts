import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { Router, RouterLink } from '@angular/router';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';

import { NavbarComponent } from './components/navbar/navbar.component'

import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatMenuModule } from '@angular/material/menu';
import { AsyncPipe, CommonModule } from '@angular/common';
import { RegisterComponent } from './pages/register/register.component';
import { MatSelectModule } from "@angular/material/select";

import { TokenInterceptor } from './interceptor/token.interceptor';
import { UsersComponent } from './pages/users/users.component';
import { RolesComponent } from './pages/roles/roles.component';
import { RoleFormComponent } from './components/role-form/role-form.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { RoleListComponent } from './components/role-list/role-list.component';
import { JobOfferComponent } from './pages/job-offer/job-offer.component';
import { MatListModule } from '@angular/material/list';
import { ApplyJobOffersComponent } from './pages/apply-job-offers/apply-job-offers.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavbarComponent,
    HomeComponent,
    RegisterComponent,
    UsersComponent,
    RolesComponent,
    RoleFormComponent,
    RoleListComponent,
    JobOfferComponent,
    ApplyJobOffersComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatToolbarModule,
    MatIconModule,
    RouterLink,
    MatInputModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatSnackBarModule,
    MatMenuModule,
    CommonModule,
    MatSnackBarModule,
    MatSelectModule,
    AsyncPipe,
    HttpClientModule,
    MatFormFieldModule,
    FormsModule,
    MatListModule 
  ],
  providers: [{
                provide: HTTP_INTERCEPTORS,
                useClass: TokenInterceptor,
                multi: true // Permite que múltiples interceptores se puedan usar
              }],
  bootstrap: [AppComponent]
})
export class AppModule { }

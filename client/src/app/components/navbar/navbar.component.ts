import { Component, inject, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent{
  authService = inject(AuthService);
  router = inject(Router);
  matSnackBar=inject(MatSnackBar);

  logout=()=>{
    this.authService.logout();
    this.matSnackBar.open('Has cerrado sesión', "Close",{
      duration:5000,
      horizontalPosition:'center'
    })
    this.router.navigate(['/login']);


  }
}

import { Component, inject, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from 'src/app/interfaces/user';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit{
  authService = inject(AuthService)

  users$!: Observable<User[]>
  
  ngOnInit(): void {
    this.users$ = this.authService.getAllUsers();
  }

  // Función trackBy que usa el id del usuario para identificar elementos únicos
  trackById(index: number, user: User): string {
    return user.id;
  }
  

}

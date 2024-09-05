import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { Role } from 'src/app/interfaces/role';
import { RoleCreateRequest } from 'src/app/interfaces/role-create-request';
import { User } from 'src/app/interfaces/user';
import { AuthService } from 'src/app/services/auth.service';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit{
  roleService = inject(RoleService)
  authService = inject(AuthService)

  roles$!: Observable<Role[]>
  users$!: Observable<User[]>

  selectedUser:string = ''
  selectedRole:string = ''

  errorMessage = ''
  
  role:RoleCreateRequest = {} as RoleCreateRequest
  
  matSnackBar = inject(MatSnackBar)


  ngOnInit(): void {
    this.roles$ = this.roleService.getRoles()
    this.users$ = this.authService.getAllUsers()
  }

  trackByUserId(index: number, user: User): string {
    return user.id;
  }

  trackByRoleId(index: number, role: Role): string {
    return role.id;
  }

  createRole(role:RoleCreateRequest){
    this.roleService.createRole(role).subscribe({
      next:(response:{message:string}) => {
        this.roles$ = this.roleService.getRoles()
        this.matSnackBar.open("Role created successfully", "Ok", {
          duration:4000
        })
      },
      error: (error:HttpErrorResponse) => {
        if(error.status === 400){
          this.errorMessage = error.error
        }
      }
    })
  }

  deleteRole(id:string) {
    this.roleService.deleteRole(id).subscribe({
      next:(response)=>{
        this.roles$ = this.roleService.getRoles()
        this.matSnackBar.open("Role deleted successfully", "Ok", {
          duration:4000
        })
      },
      error:(error:HttpErrorResponse) => {
        this.matSnackBar.open(error.message, 'Close', {
          duration:4000
        })
      }
    })
  }

  assignRole(){
    this.roleService.assignRole(this.selectedUser, this.selectedRole).subscribe({
      next:(response)=>{
        this.roles$ = this.roleService.getRoles()
        this.matSnackBar.open("Role assign successfully", "Ok", {
          duration:4000
        })
      },
      error:(error:HttpErrorResponse) => {
        this.matSnackBar.open(error.message, 'Close', {
          duration:4000
        })
      }
    })
  }
}

import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ErrorResponse } from 'src/app/interfaces/error-response';
import { FieldError } from 'src/app/interfaces/field-error';
import { RegisterRequest } from 'src/app/interfaces/register-request';
import { Role } from 'src/app/interfaces/role';


import { AuthService } from 'src/app/services/auth.service';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  fb = inject(FormBuilder)
  registerFom!: FormGroup
  router = inject(Router)
  passwordHide:boolean = true;
  confirmPasswordHide:boolean = true;
  matSnackBar = inject(MatSnackBar);
  errors: FieldError[] = [];

  roleService = inject(RoleService);
  authService = inject(AuthService);

  ngOnInit(): void {
      this.registerFom = this.fb.group({
        email:['', [Validators.required, Validators.email]],
        password: ['', [
          Validators.required,
          Validators.minLength(8),   
          Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)
        ]],
        confirmPassword:['', [
          Validators.required,
          Validators.minLength(8),   
          this.passwordValidator()
        ]],
        fullName:['', Validators.required]
      },
      {
        validator:this.passwordMatchValidator,
      });

  }

  register(){
    this.authService.register(this.registerFom.value).subscribe({
      next:(response)=>{
        this.matSnackBar.open(response.message, 'Close', {
          duration: 5000,
          horizontalPosition: 'center'
        });
        this.router.navigate(['/login']);
      },
      error: (err: HttpErrorResponse) => {
        if (err.error && err.error.message) {
          // Show server-side message in MatSnackBar
          this.matSnackBar.open(err.error.message, 'Close', {
            duration: 5000,
            horizontalPosition: 'center'
          });
        } else {
          // Handle unexpected errors
          this.matSnackBar.open('An unexpected error occurred. Please try again.', 'Close', {
            duration: 5000,
            horizontalPosition: 'center'
          });
        }
      }
    });
  }

  

  passwordValidator(): Validators {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const value = control.value || '';
      const hasUpperCase = /[A-Z]/.test(value);
      const hasLowerCase = /[a-z]/.test(value);
      const hasNumeric = /[0-9]/.test(value);
      const hasSpecial = /[@$!%*?&]/.test(value);
      const valid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecial;

      return !valid ? { 'strongPassword': { value: control.value } } : null;
    };
  }

  private passwordMatchValidator(
    control:AbstractControl
  ):{ [key:string]:boolean } | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if(password !== confirmPassword){
      return { passwordMismatch:true }
    }
    return null;
  }
}

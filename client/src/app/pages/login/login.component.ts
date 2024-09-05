import { HttpBackend } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  authService = inject(AuthService);
  matSnackBar = inject(MatSnackBar);
  router = inject(Router);
  hide = true;
  form!: FormGroup;
  fb = inject(FormBuilder);

  ngOnInit(): void {
    this.form = this.fb.group({
      email:['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required,
        Validators.minLength(8),   
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)
      ]]
    });
  }

  login() {
    // Llama al método `login` del servicio de autenticación, pasando los valores del formulario.
    this.authService.login(this.form.value).subscribe({
      // La propiedad `next` maneja la respuesta exitosa del servidor.
      next: (response) => {
        // Muestra un mensaje en la barra de notificaciones (matSnackBar) con el mensaje del servidor.
        this.matSnackBar.open(response.message, 'Close', {
          duration: 5000, // Duración de la notificación en milisegundos (5 segundos).
          horizontalPosition: 'center' // Posición horizontal de la notificación.
        });
        // Redirige al usuario a la página principal (o cualquier otra página).
        this.router.navigate(['/']);
      },
      // La propiedad `error` maneja cualquier error que ocurra durante la solicitud.
      error: (error) => {
        // Muestra un mensaje en la barra de notificaciones con el mensaje de error del servidor.
        this.matSnackBar.open(error.error.message, 'Close', {
          duration: 5000, // Duración de la notificación en milisegundos (5 segundos).
          horizontalPosition: 'center' // Posición horizontal de la notificación.
        });
      }
    });
  }
  
  // Validador personalizado para una contraseña fuerte
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
}

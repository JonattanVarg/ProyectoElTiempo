import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

export const roleGuard: CanActivateFn = (route, state) => {
  const roles = route.data['roles'] as string[]
  const authService = inject(AuthService)
  const matSnackBar = inject(MatSnackBar)
  const userRoles = authService.getRolesFromUser()
  const router = inject(Router)

  if(!authService.isLoggedIn()){
    router.navigate(['/login'])

    matSnackBar.open('Debes estar logueado para ver esta página', 'Ok', {
      duration: 4000
    })
    return false
  }

  if(roles.some((role)=>userRoles?.includes(role))) return true;

  router.navigate(['/'])
  matSnackBar.open('No tienes permiso para ver esta página', 'Ok', {
    duration:5000
  })

  return false
};

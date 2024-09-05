import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { LoginRequest } from '../interfaces/login-request';
import { map, Observable } from 'rxjs';
import { AuthResponse } from '../interfaces/auth-response';
import { HttpClient } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';
import { UserDetail } from '../interfaces/user-detail';
import { RegisterRequest } from '../interfaces/register-request';
import { UserFromJWT } from '../interfaces/user-from-jwt';
import { User } from '../interfaces/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  apiUrl:string = environment.apiUrl;
  private tokenKey = 'token';

  constructor(private http:HttpClient) { }

  login(data:LoginRequest):Observable<AuthResponse>{
    return this.http.post<AuthResponse>(`${this.apiUrl}/account/login`, data).pipe(
      map((response) => {
        if(response.isSuccess){
          localStorage.setItem(this.tokenKey, response.token);
        }
        return response;
      })
    )
  }

  register(data:RegisterRequest):Observable<AuthResponse>{
    return this.http.post<AuthResponse>(`${this.apiUrl}/account/register`, data)
  }

  isLoggedIn=():boolean => {
    const token = this.getToken();
    if(!token) return false;

    return !this.isTokenExpired();
  }

  private isTokenExpired(){
    const token = this.getToken();
    if(!token) return true;
    const decoded = jwtDecode(token);
    const isTokenExpired = Date.now() >= decoded['exp']!*1000;  
    if(isTokenExpired) this.logout();
    return isTokenExpired;
  }

  getToken = ():string => localStorage.getItem(this.tokenKey) || "";

  logout=():void=>{
    localStorage.removeItem(this.tokenKey)
  };

  getUserFromJWT =(): UserFromJWT | null =>{
    const token = this.getToken();
    if(!token) return null;
    const decodedToken:any = jwtDecode(token);
    const user = {
      id:decodedToken.nameid,
      fullName:decodedToken.name,
      email:decodedToken.email,
      role:decodedToken.role
    }
    return user;
  };

  getAllUsers = (): Observable<User[]> => this.http.get<User[]>(`${this.apiUrl}/account`);

  getRolesFromUser = (): string[] | null => {
    const token = this.getToken()
    if(!token) return null

    const decodedToken:any = jwtDecode(token)
    return decodedToken.role || null
    
  }

}

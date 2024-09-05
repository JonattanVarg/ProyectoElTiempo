import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { JobApplicationCreate } from '../interfaces/job-application-create';
import { Observable } from 'rxjs';
import { GenericResponse } from '../interfaces/generic-response';
import { JobApplication } from '../interfaces/job-application';

@Injectable({
  providedIn: 'root'
})
export class ApplyJobOffersService {

  apiUrl = environment.apiUrl;

  constructor(private http:HttpClient) { }


//Envía una solicitud POST para aplicar a un empleo.
applyToJob(jobApplication: JobApplicationCreate): Observable<GenericResponse<JobApplication>> {
  return this.http.post<GenericResponse<JobApplication>>(`${this.apiUrl}/jobapplications`, jobApplication);
}

// Obtener una aplicación de trabajo por ID
getApplicationById(applicationId: number): Observable<GenericResponse<JobApplication>> {
  return this.http.get<GenericResponse<JobApplication>>(`${this.apiUrl}/jobapplications/${applicationId}`);
}

// Recupera todas las aplicaciones de empleo.
getAllJobApplications(): Observable<GenericResponse<JobApplication[]>> {
  return this.http.get<GenericResponse<JobApplication[]>>(`${this.apiUrl}/jobapplications`);
}

// Envía una solicitud DELETE para eliminar una aplicación de empleo por su ID.
deleteApplication(applicationId: number): Observable<GenericResponse<null>> {
  return this.http.delete<GenericResponse<null>>(`${this.apiUrl}/jobapplications/${applicationId}`);
}
}

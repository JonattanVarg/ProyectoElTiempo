import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { GenericResponse } from '../interfaces/generic-response';
import { JobOffer } from '../interfaces/job-offer';
import { JobApplication } from '../interfaces/job-application';
import { JobOfferCreate } from '../interfaces/job-offer-create';
import { JobOfferUpdate } from '../interfaces/job-offer-update';


@Injectable({
  providedIn: 'root'
})
export class JobOfferService {

  apiUrl = environment.apiUrl;

  constructor(private http:HttpClient) { }

  getAllJobOffers(): Observable<GenericResponse<JobOffer[]>> {
    return this.http.get<GenericResponse<JobOffer[]>>(`${this.apiUrl}/joboffers`);
  }

  // Obtener una oferta de trabajo por ID
  getJobOfferById(id: number): Observable<GenericResponse<JobOffer>> {
    return this.http.get<GenericResponse<JobOffer>>(`${this.apiUrl}/joboffers/${id}`);
  }

  // Crear una nueva oferta de trabajo
  createJobOffer(jobOffer: JobOfferCreate): Observable<GenericResponse<JobOffer>> {
    return this.http.post<GenericResponse<JobOffer>>(`${this.apiUrl}/joboffers`, jobOffer);
  }

  // Actualizar una oferta de trabajo existente
  updateJobOffer(id: number, jobOffer: JobOfferUpdate): Observable<GenericResponse<JobOffer>> {
    return this.http.put<GenericResponse<JobOffer>>(`${this.apiUrl}/joboffers/${id}`, jobOffer);
  }

  // Eliminar una oferta de trabajo
  deleteJobOffer(id: number): Observable<GenericResponse<null>> {
    return this.http.delete<GenericResponse<null>>(`${this.apiUrl}/joboffers/${id}`);
  }

  // Obtener aplicaciones para una oferta de trabajo específica
  getJobApplicationsByJobOfferId(id: number): Observable<GenericResponse<JobApplication[]>> {
    return this.http.get<GenericResponse<JobApplication[]>>(`${this.apiUrl}/joboffers/${id}/applications`);
  }
}

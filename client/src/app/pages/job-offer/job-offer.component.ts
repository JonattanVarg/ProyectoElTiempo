import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { GenericResponse } from 'src/app/interfaces/generic-response';
import { JobApplication } from 'src/app/interfaces/job-application';
import { JobOffer } from 'src/app/interfaces/job-offer';
import { JobOfferCreate } from 'src/app/interfaces/job-offer-create';
import { JobOfferUpdate } from 'src/app/interfaces/job-offer-update';
import { JobOfferService } from 'src/app/services/job-offer.service';

@Component({
  selector: 'app-job-offer',
  templateUrl: './job-offer.component.html',
  styleUrls: ['./job-offer.component.css']
})
export class JobOfferComponent implements OnInit {
  jobOffers: JobOffer[] = [];
  jobOfferForm: FormGroup;
  selectedJobOffer: JobOffer | null = null;
  jobApplications: JobApplication[] = [];
  isEditing: boolean = false;
  errorMessage: string | null = null;

  matSnackBar = inject(MatSnackBar);
  router = inject(Router);
  route = inject(ActivatedRoute);
  fb = inject(FormBuilder);
  jobOfferService = inject(JobOfferService);

  constructor() {
    this.jobOfferForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      description: ['', [Validators.required, Validators.minLength(5)]],
      location: ['', [Validators.required, Validators.minLength(5)]],
      salary: [1, [Validators.required, Validators.min(1)]],
      contractType: ['', [Validators.required, Validators.minLength(5)]],
    });
  }

  ngOnInit(): void {
    this.getAllJobOffers();

    const jobId = this.route.snapshot.paramMap.get('id');
    if (jobId) {
      this.getJobOfferById(Number(jobId));
    }
  }

  getAllJobOffers(): void {
    this.jobOfferService.getAllJobOffers().subscribe({
      next: (response: GenericResponse<JobOffer[]>) => {
        if (response.isSuccess) {
          this.jobOffers = response.data!;
        } else {
          this.showError(response.message);
        }
      },
      error: (err) => {
        this.showError('Error obtaining job offers.');
      }
    });
  }

  getJobOfferById(id: number): void {
    this.jobOfferService.getJobOfferById(id).subscribe({
      next: (response: GenericResponse<JobOffer>) => {
        if (response.isSuccess) {
          this.selectedJobOffer = response.data;
          this.populateForm(response.data!);
          this.isEditing = true;
          this.getJobApplicationsByJobOffer(id); // Obtener solicitudes de empleo cuando se selecciona una oferta de trabajo
        } else {
          this.showError(response.message);
        }
      },
      error: (err) => {
        this.showError('Error obtaining job offer.');
      }
    });
  }

  loadJobOfferForEdit(id: number): void {
    this.jobOfferService.getJobOfferById(id).subscribe({
      next: (response: GenericResponse<JobOffer>) => {
        if (response.isSuccess) {
          this.selectedJobOffer = response.data;
          this.populateForm(response.data!);
          this.isEditing = true;
        } else {
          this.showError(response.message);
        }
      },
      error: (err) => {
        this.showError('Error obtaining job offer.');
      }
    });
  }  

  showJobApplication(jobOfferId: number): void {
    this.jobOfferService.getJobOfferById(jobOfferId).subscribe({
      next: (response: GenericResponse<JobOffer>) => {
        if (response.isSuccess) {
          this.selectedJobOffer = response.data;
          this.getJobApplicationsByJobOffer(jobOfferId);
        } else {
          this.showError(response.message);
        }
      },
      error: (err) => {
        this.showError('Error obtaining job offer.');
      }
    });
  }


  getJobApplicationsByJobOffer(id: number): void {
    this.jobOfferService.getJobApplicationsByJobOfferId(id).subscribe({
      next: (response: GenericResponse<JobApplication[]>) => {
        if (response.isSuccess) {
          this.jobApplications = response.data!;
        } else {
          this.showError(response.message);
        }
      },
      error: () => {
        this.showError('Error obtaining job applications.');
      }
    });
  }




  populateForm(jobOffer: JobOffer): void {
    this.jobOfferForm.patchValue({
      title: jobOffer.title,
      description: jobOffer.description,
      location: jobOffer.location,
      salary: jobOffer.salary,
      contractType: jobOffer.contractType,
    });
  }

  saveJobOffer(): void {
    if (this.jobOfferForm.invalid) {
      return;
    }

    const jobOfferData = this.jobOfferForm.value;

    if (this.isEditing && this.selectedJobOffer) {
      this.updateJobOffer(this.selectedJobOffer.id, jobOfferData);
    } else {
      this.createJobOffer(jobOfferData);
    }
  }

  createJobOffer(jobOffer: JobOfferCreate): void {
    this.jobOfferService.createJobOffer(jobOffer).subscribe({
      next: (response: GenericResponse<JobOffer>) => {
        if (response.isSuccess) {
          const currentUrl = this.router.url;
          // Navega a la misma ruta y vuelve a cargar
          this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
            this.router.navigate([currentUrl]);
          });
        } else {
          this.showError(response.message);
        }
      },
      error: () => {
        this.showError('Error creating job offer.');
      }
    });
  }

  updateJobOffer(id: number, jobOffer: JobOfferUpdate): void {
    this.jobOfferService.updateJobOffer(id, jobOffer).subscribe({
      next: (response: GenericResponse<JobOffer>) => { 
        if (response.isSuccess) { 
          const currentUrl = this.router.url;
          // Navega a la misma ruta y vuelve a cargar
          this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
            this.router.navigate([currentUrl]);
          });
        } else {
          this.showError(response.message);
        }
      },
      error: (err) => {
        this.showError('Error updating job offer.');
      }
    });
  }

  deleteJobOffer(id: number): void {
    if (confirm('¿Estás seguro de eliminar la oferta laboral?')) {
      this.jobOfferService.deleteJobOffer(id).subscribe({
        next: (response: GenericResponse<null>) => {
          if (response.isSuccess) {
            const currentUrl = this.router.url;
            // Navega a la misma ruta y vuelve a cargar
          this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
            this.router.navigate([currentUrl]);
          });
          } else {
            this.showError(response.message);
          }
        },
        error: (err) => {
          this.showError('Error deleting job offer.');
        }
      });
    }
  }

  resetForm(): void {
    this.jobOfferForm.reset();
    this.selectedJobOffer = null;
    this.isEditing = false;
  }

  private showError(message: string): void {
    this.matSnackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
    });
  }
}



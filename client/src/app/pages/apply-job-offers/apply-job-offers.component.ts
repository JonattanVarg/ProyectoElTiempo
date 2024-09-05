import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { JobApplicationCreate } from 'src/app/interfaces/job-application-create';
import { JobOffer } from 'src/app/interfaces/job-offer';
import { ApplyJobOffersService } from 'src/app/services/apply-job-offers.service';
import { AuthService } from 'src/app/services/auth.service';
import { JobOfferService } from 'src/app/services/job-offer.service';

@Component({
  selector: 'app-apply-job-offers',
  templateUrl: './apply-job-offers.component.html',
  styleUrls: ['./apply-job-offers.component.css']
})
export class ApplyJobOffersComponent implements OnInit {

  jobOffers: JobOffer[] = [];

  matSnackBar = inject(MatSnackBar);
  router = inject(Router);
  route = inject(ActivatedRoute);
  fb = inject(FormBuilder);
  authService = inject(AuthService);
  jobOfferService = inject(JobOfferService);
  applyJobOffersService = inject(ApplyJobOffersService);

  ngOnInit(): void {
    this.loadJobOffers();
  }

  loadJobOffers(): void {
    this.jobOfferService.getAllJobOffers().subscribe(response => {
      if (response.isSuccess) {
        this.jobOffers = response.data || [];
      } else {
        this.matSnackBar.open('Error al cargar las ofertas de trabajo', 'Cerrar', {
          duration: 5000,
          horizontalPosition: 'center'
        });
      }
    });
  }

  applyJob(jobOfferId: number): void {
    const user = this.authService.getUserFromJWT();

    if (!user) {
      this.matSnackBar.open('Debe iniciar sesión para aplicar a una oferta de trabajo.', 'Cerrar', {
        duration: 5000,
        horizontalPosition: 'center'
      });
      return;
    }

    const jobApplication: JobApplicationCreate = {
      candidateName: user.fullName,
      candidateEmail: user.email,
      jobOfferId: jobOfferId
    }; 
    this.applyJobOffersService.applyToJob(jobApplication).subscribe(response => {
      if (response.isSuccess) {
        this.matSnackBar.open(response.message, 'Cerrar', {
          duration: 5000,
          horizontalPosition: 'center'
        });
      } else {
        this.matSnackBar.open('Error al aplicar a la oferta de trabajo', 'Cerrar', {
          duration: 5000,
          horizontalPosition: 'center'
        });
      }
    });
  }
}
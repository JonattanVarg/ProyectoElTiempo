using API.Dtos.JobApplication;
using API.Dtos.JobOffer;
using API.Models;
using AutoMapper;
using System.Collections.Generic;

namespace API.Mappings
{
    public class JobMappingsProfile : Profile
    {
        public JobMappingsProfile()
        {
            CreateMap<JobOffer, JobOfferDto>();
            CreateMap<JobOfferDto, JobOffer>();
            CreateMap<JobOfferCreateDto, JobOffer>();
            CreateMap<JobOfferUpdateDto, JobOffer>();
            CreateMap<JobApplication, JobApplicationDto>(); 
            CreateMap<JobApplicationDto, JobApplication>();
            CreateMap<JobApplication, JobApplicationCreateDto>();
            CreateMap<JobApplicationCreateDto, JobApplication>();
            CreateMap<JobApplicationCreateDto, JobApplication>();
        }
    }
}

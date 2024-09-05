using API.Dtos.JobApplication;
using API.Dtos.JobOffer;
using API.Dtos.Responses;

namespace API.Services.Interfaces
{
    public interface IJobOfferService
    {
        Task<GenericResponseDto<IEnumerable<JobOfferDto>>> GetAllJobOffersAsync();
        Task<GenericResponseDto<JobOfferDto>> GetJobOfferByIdAsync(int id);
        Task<GenericResponseDto<JobOfferDto>> AddJobOfferAsync(JobOfferCreateDto jobOfferCreateDto);  
        Task<GenericResponseDto<JobOfferDto>> UpdateJobOfferAsync(int id, JobOfferUpdateDto jobOfferUpdateDto);
        Task<GenericResponseDto<JobOfferDto>> DeleteJobOfferAsync(int id);
        Task<GenericResponseDto<IEnumerable<JobApplicationDto>>> GetJobApplicationsByJobOfferIdAsync(int id);
    }
}

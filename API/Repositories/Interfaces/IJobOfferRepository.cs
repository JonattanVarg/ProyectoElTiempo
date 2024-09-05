using API.Dtos.JobOffer;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IJobOfferRepository
    {
        Task<IEnumerable<JobOffer>> GetAllJobOffersAsync();
        Task<JobOffer> GetJobOfferByIdAsync(int id); 
        Task<JobOffer> GetJobOfferWithApplicationsByIdAsync(int id); 
        Task AddJobOfferAsync(JobOffer jobOffer);
        Task UpdateJobOfferAsync(JobOffer jobOffer);
        Task DeleteJobOfferAsync(int id);
    }
}
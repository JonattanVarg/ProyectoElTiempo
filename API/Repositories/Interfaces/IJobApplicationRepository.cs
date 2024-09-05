using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<IEnumerable<JobApplication>> GetAllJobApplicationsAsync();
        Task<JobApplication> GetJobApplicationByIdAsync(int id);
        Task AddJobApplicationAsync(JobApplication application);
        Task DeleteJobApplicationAsync(int id);
    }
}

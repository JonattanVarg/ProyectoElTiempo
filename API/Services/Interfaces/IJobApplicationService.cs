using API.Dtos.JobApplication;
using API.Dtos.JobOffer;
using API.Dtos.Responses;

namespace API.Services.Interfaces
{
    public interface IJobApplicationService
    {
        Task<GenericResponseDto<IEnumerable<JobApplicationDto>>> GetAllJobApplicationsAsync();
        Task<GenericResponseDto<JobApplicationDto>> GetJobApplicationByIdAsync(int id);
        Task<GenericResponseDto<JobApplicationDto>> AddJobApplicationAsync(JobApplicationCreateDto jobApplicationCreateDto);
        Task<GenericResponseDto<JobApplicationDto>> DeleteJobApplicationAsync(int id);
    }
}

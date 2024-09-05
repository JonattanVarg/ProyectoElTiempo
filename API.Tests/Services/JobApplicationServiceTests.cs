using API.Dtos.JobApplication;
using API.Dtos.Responses;
using API.Models;
using API.Repositories.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API.Tests.Services
{
    public class JobApplicationServiceTests
    {
        private readonly Mock<IJobApplicationRepository> _mockRepository;
        private readonly Mock<IJobOfferRepository> _mockJobOfferRepository;
        private readonly Mock<ILogger<JobApplicationService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly JobApplicationService _service;

        public JobApplicationServiceTests()
        {
            _mockRepository = new Mock<IJobApplicationRepository>();
            _mockJobOfferRepository = new Mock<IJobOfferRepository>();
            _mockLogger = new Mock<ILogger<JobApplicationService>>();
            _mockMapper = new Mock<IMapper>();
            _service = new JobApplicationService(
                _mockRepository.Object,
                _mockJobOfferRepository.Object,
                _mockLogger.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetAllJobApplicationsAsync_ReturnsSuccessResponse_WhenJobApplicationsExist()
        {
            // Arrange
            var jobApplications = new List<JobApplication>
            {
                new JobApplication { CandidateName = "Carlos Alvarez", CandidateEmail = "carlosalvarez@gmail.com", JobOfferId = 1 },
                new JobApplication { CandidateName = "Juan Smith", CandidateEmail = "juansmith@gmail.com", JobOfferId = 2 }
            };

            _mockRepository.Setup(repo => repo.GetAllJobApplicationsAsync()).ReturnsAsync(jobApplications);
            _mockMapper.Setup(m => m.Map<IEnumerable<JobApplicationDto>>(jobApplications)).Returns(jobApplications.Select(ja => new JobApplicationDto
            {
                Id = ja.Id,
                CandidateName = ja.CandidateName,
                CandidateEmail = ja.CandidateEmail,
                JobOfferId = ja.JobOfferId,
                DateApplied = ja.DateApplied
            }));

            // Act
            var result = await _service.GetAllJobApplicationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data!.Count());
        }

        [Fact]
        public async Task GetAllJobApplicationsAsync_ReturnsSuccessResponse_WhenNoJobApplicationsExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllJobApplicationsAsync()).ReturnsAsync(new List<JobApplication>());

            // Act
            var result = await _service.GetAllJobApplicationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data!);
        }

        [Fact]
        public async Task GetJobApplicationByIdAsync_ReturnsSuccessResponse_WhenJobApplicationExists()
        {
            // Arrange
            var jobApplication = new JobApplication { Id = 1, CandidateName = "Carlos Alvarez", CandidateEmail = "carlosalvarez@gmail.com", JobOfferId = 1 };

            _mockRepository.Setup(repo => repo.GetJobApplicationByIdAsync(It.IsAny<int>())).ReturnsAsync(jobApplication);
            _mockMapper.Setup(m => m.Map<JobApplicationDto>(jobApplication)).Returns(new JobApplicationDto
            {
                Id = jobApplication.Id,
                CandidateName = jobApplication.CandidateName,
                CandidateEmail = jobApplication.CandidateEmail,
                JobOfferId = jobApplication.JobOfferId,
                DateApplied = jobApplication.DateApplied
            });

            // Act
            var result = await _service.GetJobApplicationByIdAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Carlos Alvarez", result.Data!.CandidateName);
        }

        [Fact]
        public async Task GetJobApplicationByIdAsync_ReturnsFailureResponse_WhenJobApplicationDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetJobApplicationByIdAsync(It.IsAny<int>())).ReturnsAsync((JobApplication)null);

            // Act
            var result = await _service.GetJobApplicationByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AddJobApplicationAsync_ReturnsSuccessResponse_WhenJobApplicationIsAddedSuccessfully()
        {
            // Arrange
            var jobOffer = new JobOffer { Id = 1, Title = "Oferta", Description = "Oferta", Location = "Bogotá", Salary = 3000, ContractType = "Tiempo completo" };
            var jobApplicationCreateDto = new JobApplicationCreateDto
            {
                CandidateName = "David Espinoza",
                CandidateEmail = "davidespinoza@example.com",
                JobOfferId = jobOffer.Id
            };

            var jobApplication = new JobApplication
            {
                Id = 1,
                CandidateName = jobApplicationCreateDto.CandidateName,
                CandidateEmail = jobApplicationCreateDto.CandidateEmail,
                JobOfferId = jobApplicationCreateDto.JobOfferId,
                JobOffer = jobOffer
            };

            _mockJobOfferRepository.Setup(repo => repo.GetJobOfferByIdAsync(It.IsAny<int>())).ReturnsAsync(jobOffer);
            _mockMapper.Setup(m => m.Map<JobApplication>(jobApplicationCreateDto)).Returns(jobApplication);
            _mockRepository.Setup(repo => repo.AddJobApplicationAsync(It.IsAny<JobApplication>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<JobApplicationDto>(It.IsAny<JobApplication>())).Returns(new JobApplicationDto
            {
                Id = jobApplication.Id,
                CandidateName = jobApplication.CandidateName,
                CandidateEmail = jobApplication.CandidateEmail,
                JobOfferId = jobApplication.JobOfferId,
                DateApplied = jobApplication.DateApplied
            });

            // Act
            var result = await _service.AddJobApplicationAsync(jobApplicationCreateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("David Espinoza", result.Data!.CandidateName);
        }

        [Fact]
        public async Task AddJobApplicationAsync_ReturnsFailureResponse_WhenJobOfferDoesNotExist()
        {
            // Arrange
            var jobApplicationCreateDto = new JobApplicationCreateDto
            {
                CandidateName = "Juan Diaz",
                CandidateEmail = "juandiaz@gmail.com",
                JobOfferId = 1
            };

            _mockJobOfferRepository.Setup(repo => repo.GetJobOfferByIdAsync(It.IsAny<int>())).ReturnsAsync((JobOffer)null);

            // Act
            var result = await _service.AddJobApplicationAsync(jobApplicationCreateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task DeleteJobApplicationAsync_ReturnsSuccessResponse_WhenJobApplicationIsDeletedSuccessfully()
        {
            // Arrange
            var jobApplication = new JobApplication { Id = 1, CandidateName = "Mateo Diaz", CandidateEmail = "mateodiaz@gmail.com" };

            _mockRepository.Setup(repo => repo.GetJobApplicationByIdAsync(It.IsAny<int>())).ReturnsAsync(jobApplication);
            _mockRepository.Setup(repo => repo.DeleteJobApplicationAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteJobApplicationAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteJobApplicationAsync_ReturnsFailureResponse_WhenJobApplicationDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetJobApplicationByIdAsync(It.IsAny<int>())).ReturnsAsync((JobApplication)null);

            // Act
            var result = await _service.DeleteJobApplicationAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }
    }
}

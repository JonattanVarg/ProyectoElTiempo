using API.Dtos.JobOffer;
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
    public class JobOfferServiceTests
    {
        private readonly Mock<IJobOfferRepository> _mockRepository;
        private readonly Mock<ILogger<JobOfferService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly JobOfferService _service;

        public JobOfferServiceTests()
        {
            _mockRepository = new Mock<IJobOfferRepository>();
            _mockLogger = new Mock<ILogger<JobOfferService>>();
            _mockMapper = new Mock<IMapper>();
            _service = new JobOfferService(_mockRepository.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllJobOffersAsync_ReturnsSuccessResponse_WhenJobOffersExist()
        {
            // Arrange
            var jobOffers = new List<JobOffer>
            {
                new JobOffer { Title = "El Tiempo", Description = "Trabajar en el Tiempo", Location = "Bogotá", Salary = 666, ContractType = "Tiempo parcial" },
                new JobOffer { Title = "Google", Description = "Trabajar en Google", Location = "Estados Unidos", Salary = 555, ContractType = "Tiempo completo" }
            };
            _mockRepository.Setup(repo => repo.GetAllJobOffersAsync()).ReturnsAsync(jobOffers);
            _mockMapper.Setup(m => m.Map<IEnumerable<JobOfferDto>>(jobOffers)).Returns(jobOffers.Select(jo => new JobOfferDto
            {
                Id = jo.Id,
                Title = jo.Title,
                Description = jo.Description,
                Location = jo.Location,
                Salary = jo.Salary,
                ContractType = jo.ContractType,
                DatePosted = jo.DatePosted
            }));

            // Act
            var result = await _service.GetAllJobOffersAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data!.Count());
        }

        [Fact]
        public async Task GetAllJobOffersAsync_ReturnsFailureResponse_WhenNoJobOffersExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllJobOffersAsync()).ReturnsAsync(new List<JobOffer>());

            // Act
            var result = await _service.GetAllJobOffersAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data!);
        }

        [Fact]
        public async Task GetJobOfferByIdAsync_ReturnsSuccessResponse_WhenJobOfferExists()
        {
            // Arrange
            var jobOffer = new JobOffer { Id = 1, Title = "El Tiempo", Description = "Trabajar en el Tiempo", Location = "Bogotá", Salary = 666, ContractType = "Tiempo parcial" };
            _mockRepository.Setup(repo => repo.GetJobOfferByIdAsync(It.IsAny<int>())).ReturnsAsync(jobOffer);
            _mockMapper.Setup(m => m.Map<JobOfferDto>(jobOffer)).Returns(new JobOfferDto
            {
                Id = jobOffer.Id,
                Title = jobOffer.Title,
                Description = jobOffer.Description,
                Location = jobOffer.Location,
                Salary = jobOffer.Salary,
                ContractType = jobOffer.ContractType,
                DatePosted = jobOffer.DatePosted
            });

            // Act
            var result = await _service.GetJobOfferByIdAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("El Tiempo", result.Data!.Title);
        }

        [Fact]
        public async Task GetJobOfferByIdAsync_ReturnsFailureResponse_WhenJobOfferDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetJobOfferByIdAsync(It.IsAny<int>())).ReturnsAsync((JobOffer)null);

            // Act
            var result = await _service.GetJobOfferByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AddJobOfferAsync_ReturnsSuccessResponse_WhenJobOfferIsAddedSuccessfully()
        {
            // Arrange
            var jobOfferCreateDto = new JobOfferCreateDto { Title = "El Tiempo", Description = "Trabajar en el Tiempo", Location = "Bogotá", Salary = 666, ContractType = "Tiempo parcial" };
            var jobOffer = new JobOffer
            {
                Id = 1,
                Title = jobOfferCreateDto.Title,
                Description = jobOfferCreateDto.Description,
                Location = jobOfferCreateDto.Location,
                Salary = jobOfferCreateDto.Salary,
                ContractType = jobOfferCreateDto.ContractType
            };
            _mockMapper.Setup(m => m.Map<JobOffer>(jobOfferCreateDto)).Returns(jobOffer);
            _mockRepository.Setup(repo => repo.AddJobOfferAsync(jobOffer)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<JobOfferDto>(jobOffer)).Returns(new JobOfferDto
            {
                Id = jobOffer.Id,
                Title = jobOffer.Title,
                Description = jobOffer.Description,
                Location = jobOffer.Location,
                Salary = jobOffer.Salary,
                ContractType = jobOffer.ContractType,
                DatePosted = jobOffer.DatePosted
            });

            // Act
            var result = await _service.AddJobOfferAsync(jobOfferCreateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("El Tiempo", result.Data!.Title);
        }

        [Fact]
        public async Task UpdateJobOfferAsync_ReturnsSuccessResponse_WhenJobOfferIsUpdatedSuccessfully()
        {
            // Arrange
            var jobOfferUpdateDto = new JobOfferUpdateDto
            {
                Title = "Updated Job",
                Description = "Updated Description",
                Location = "Updated Location",
                Salary = 1500,
                ContractType = "Full-Time"
            };
            var jobOffer = new JobOffer
            {
                Id = 1,
                Title = "Original Job",
                Description = "Original Description",
                Location = "Original Location",
                Salary = 1000,
                ContractType = "Full-Time"
            };
            _mockRepository.Setup(repo => repo.GetJobOfferByIdAsync(It.IsAny<int>())).ReturnsAsync(jobOffer);
            _mockRepository.Setup(repo => repo.UpdateJobOfferAsync(It.IsAny<JobOffer>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<JobOfferDto>(jobOffer)).Returns(new JobOfferDto
            {
                Id = jobOffer.Id,
                Title = jobOfferUpdateDto.Title,
                Description = jobOfferUpdateDto.Description,
                Location = jobOfferUpdateDto.Location,
                Salary = jobOfferUpdateDto.Salary,
                ContractType = jobOfferUpdateDto.ContractType,
                DatePosted = jobOffer.DatePosted
            });

            // Act
            var result = await _service.UpdateJobOfferAsync(1, jobOfferUpdateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated Job", result.Data.Title);
        }

        [Fact]
        public async Task UpdateJobOfferAsync_ReturnsFailureResponse_WhenJobOfferDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetJobOfferByIdAsync(It.IsAny<int>())).ReturnsAsync((JobOffer)null);

            // Act
            var result = await _service.UpdateJobOfferAsync(1, new JobOfferUpdateDto());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task DeleteJobOfferAsync_ReturnsSuccessResponse_WhenJobOfferIsDeletedSuccessfully()
        {
            // Arrange
            var jobOffer = new JobOffer { Id = 1, Title = "El Tiempo", Description = "Trabajar en el Tiempo", Location = "Bogotá", Salary = 666, ContractType = "Tiempo parcial" };
            _mockRepository.Setup(repo => repo.GetJobOfferByIdAsync(It.IsAny<int>())).ReturnsAsync(jobOffer);
            _mockRepository.Setup(repo => repo.DeleteJobOfferAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteJobOfferAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteJobOfferAsync_ReturnsFailureResponse_WhenJobOfferDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetJobOfferByIdAsync(It.IsAny<int>())).ReturnsAsync((JobOffer)null);

            // Act
            var result = await _service.DeleteJobOfferAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }
    }
}

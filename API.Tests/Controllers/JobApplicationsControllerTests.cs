using API.Controllers;
using API.Dtos.JobApplication;
using API.Dtos.Responses;
using API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Tests.Controllers
{
    public class JobApplicationsControllerTests
    {
        private readonly JobApplicationsController _controller;
        private readonly Mock<IJobApplicationService> _jobApplicationServiceMock;
        private readonly Mock<ILogger<JobApplicationsController>> _loggerMock;

        // Aqui utilizo utilizo mocks para simular el servicio
        public JobApplicationsControllerTests()
        {
            _jobApplicationServiceMock = new Mock<IJobApplicationService>();
            _loggerMock = new Mock<ILogger<JobApplicationsController>>();
            _controller = new JobApplicationsController(_jobApplicationServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllJobApplications_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var jobApplications = new List<JobApplicationDto>
        {
            new JobApplicationDto { Id = 1, CandidateName = "Juan Diaz", CandidateEmail = "juandiaz@gmail.com", JobOfferId = 1 },
            new JobApplicationDto { Id = 2, CandidateName = "Camilo Marquez", CandidateEmail = "camilomarquez@gmail.com", JobOfferId = 2 }
        };
            var response = new GenericResponseDto<IEnumerable<JobApplicationDto>>
            {
                IsSuccess = true,
                Data = jobApplications
            };

            _jobApplicationServiceMock.Setup(s => s.GetAllJobApplicationsAsync())
                                      .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllJobApplications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<IEnumerable<JobApplicationDto>>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(2, returnValue.Data!.Count());
        }

        [Fact]
        public async Task GetJobApplicationById_ReturnsOkResult_WhenFound()
        {
            // Arrange
            var jobApplication = new JobApplicationDto { Id = 1, CandidateName = "Juan Diaz", CandidateEmail = "juandiaz@gmail.com", JobOfferId = 1 };
            var response = new GenericResponseDto<JobApplicationDto>
            {
                IsSuccess = true,
                Data = jobApplication
            };

            _jobApplicationServiceMock.Setup(s => s.GetJobApplicationByIdAsync(1))
                                      .ReturnsAsync(response);

            // Act
            var result = await _controller.GetJobApplicationById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobApplicationDto>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(1, returnValue.Data!.Id);
        }

        [Fact]
        public async Task GetJobApplicationById_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            var response = new GenericResponseDto<JobApplicationDto>
            {
                IsSuccess = false,
                Data = null
            };

            _jobApplicationServiceMock.Setup(s => s.GetJobApplicationByIdAsync(1))
                                      .ReturnsAsync(response);

            // Act
            var result = await _controller.GetJobApplicationById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobApplicationDto>>(notFoundResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Null(returnValue.Data);
        }

        [Fact]
        public async Task CreateJobApplication_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var jobApplicationCreateDto = new JobApplicationCreateDto { CandidateName = "Carlos Alvarez", CandidateEmail = "carlosalvarez@gmail.com", JobOfferId = 1 };
            var createdJobApplication = new JobApplicationDto { Id = 1, CandidateName = "Juan Diaz", CandidateEmail = "juandiaz@gmail.com", JobOfferId = 1 };
            var response = new GenericResponseDto<JobApplicationDto>
            {
                IsSuccess = true,
                Data = createdJobApplication
            };

            _jobApplicationServiceMock.Setup(s => s.AddJobApplicationAsync(jobApplicationCreateDto))
                                      .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateJobApplication(jobApplicationCreateDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobApplicationDto>>(createdAtActionResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(1, returnValue.Data!.Id);
        }

        [Fact]
        public async Task DeleteJobApplication_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var jobApplication = new JobApplicationDto { Id = 1, CandidateName = "Ricardo Suarez", CandidateEmail = "ricardosuarez@gmail.com", JobOfferId = 1 };
            var response = new GenericResponseDto<JobApplicationDto>
            {
                IsSuccess = true,
                Data = jobApplication
            };

            _jobApplicationServiceMock.Setup(s => s.DeleteJobApplicationAsync(It.IsAny<int>()))
                                      .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteJobApplication(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobApplicationDto>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(jobApplication.Id, returnValue.Data!.Id);
        }

        [Fact]
        public async Task DeleteJobApplication_ReturnsNotFound_WhenNotFound()
        {
            // Arrange
            var response = new GenericResponseDto<JobApplicationDto>
            {
                IsSuccess = false,
                Data = null
            };

            _jobApplicationServiceMock.Setup(s => s.DeleteJobApplicationAsync(It.IsAny<int>()))
                                      .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteJobApplication(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobApplicationDto>>(notFoundResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Null(returnValue.Data);
        }
    }
}
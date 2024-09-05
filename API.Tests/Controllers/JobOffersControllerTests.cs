using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.Services.Interfaces;
using API.Dtos.JobOffer;
using API.Dtos.Responses;
using Microsoft.Extensions.Logging;

namespace API.Tests.Controllers
{
    public class JobOffersControllerTests
    {
        private readonly Mock<IJobOfferService> _jobOfferServiceMock;
        private readonly JobOffersController _controller;
        private readonly Mock<ILogger<JobOffersController>> _loggerMock;

        public JobOffersControllerTests()
        {
            _jobOfferServiceMock = new Mock<IJobOfferService>();
            _loggerMock = new Mock<ILogger<JobOffersController>>();
            _controller = new JobOffersController(_jobOfferServiceMock.Object, _loggerMock.Object);
        }

        // 1. Pruebas del Método GetAllJobOffers
        [Fact]
        public async Task GetAllJobOffers_ReturnsOkResult_WithListOfJobOffers()
        {
            // Arrange
            var jobOffers = new List<JobOfferDto>
        {
            new JobOfferDto { Id = 1, Title = "Ingeniero Civil", Description = "Ingeniero Civil", Location = "Bogotá", Salary = 60000, ContractType = "Tiempo completo", DatePosted = DateTime.UtcNow },
            new JobOfferDto { Id = 2, Title = "Ingeniero Industrial", Description = "Ingeniero Industrial", Location = "Bogotá", Salary = 40000, ContractType = "Tiempo parcial", DatePosted = DateTime.UtcNow }
        };

            var response = new GenericResponseDto<IEnumerable<JobOfferDto>>
            {
                IsSuccess = true,
                Data = jobOffers
            };

            _jobOfferServiceMock.Setup(s => s.GetAllJobOffersAsync()).ReturnsAsync(response);

            // Act
            var result = await _controller.GetAllJobOffers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<IEnumerable<JobOfferDto>>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(2, returnValue.Data!.Count());
        }

        [Fact]
        public async Task GetAllJobOffers_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _jobOfferServiceMock.Setup(s => s.GetAllJobOffersAsync()).ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var result = await _controller.GetAllJobOffers();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var returnValue = Assert.IsType<GenericResponseDto<IEnumerable<JobOfferDto>>>(objectResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal("Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.", returnValue.Message);
        }

        // 2. Pruebas del Método GetJobOfferById
        [Fact]
        public async Task GetJobOfferById_ReturnsOkResult_WithJobOffer()
        {
            // Arrange
            var jobOffer = new JobOfferDto { Id = 1, Title = "Ingeniero Industrial", Description = "Ingeniero Industrial", Location = "Bogotá", Salary = 40000, ContractType = "Tiempo parcial", DatePosted = DateTime.UtcNow };

            var response = new GenericResponseDto<JobOfferDto>
            {
                IsSuccess = true,
                Data = jobOffer
            };

            _jobOfferServiceMock.Setup(s => s.GetJobOfferByIdAsync(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetJobOfferById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(1, returnValue.Data!.Id);
        }

        [Fact]
        public async Task GetJobOfferById_ReturnsNotFoundResult_WhenJobOfferNotFound()
        {
            // Arrange
            var response = new GenericResponseDto<JobOfferDto>
            {
                IsSuccess = false,
                Data = null
            };

            _jobOfferServiceMock.Setup(s => s.GetJobOfferByIdAsync(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetJobOfferById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(notFoundResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Null(returnValue.Data);
        }

        [Fact]
        public async Task GetJobOfferById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _jobOfferServiceMock.Setup(s => s.GetJobOfferByIdAsync(1)).ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var result = await _controller.GetJobOfferById(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var returnValue = Assert.IsType<GenericResponseDto<IEnumerable<JobOfferDto>>>(objectResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal("Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.", returnValue.Message);
        }

        // 3. Pruebas del Método CreateJobOffer
        [Fact]
        public async Task CreateJobOffer_ReturnsCreatedAtActionResult_WithCreatedJobOffer()
        {
            // Arrange
            var jobOfferCreateDto = new JobOfferCreateDto { Title = "Ingeniero Industrial", Description = "Ingeniero Industrial", Location = "Bogotá", Salary = 40000, ContractType = "Tiempo parcial" };

            var jobOfferDto = new JobOfferDto
            {
                Id = 1,
                Title = "Ingeniero Industrial",
                Description = "Ingeniero Industrial",
                Location = "Bogotá",
                Salary = 40000,
                ContractType = "Tiempo parcial",
                DatePosted = DateTime.UtcNow
            };

            var response = new GenericResponseDto<JobOfferDto>
            {
                IsSuccess = true,
                Data = jobOfferDto
            };

            _jobOfferServiceMock.Setup(s => s.AddJobOfferAsync(jobOfferCreateDto)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateJobOffer(jobOfferCreateDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(createdAtActionResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(1, returnValue.Data!.Id);
            Assert.Equal(nameof(_controller.GetJobOfferById), createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task CreateJobOffer_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var jobOfferCreateDto = new JobOfferCreateDto
            {
                Title = "Ingeniero Industrial",
                Description = "Ingeniero Industrial",
                Location = "Bogotá",
                Salary = 40000,
                ContractType = "Tiempo parcial"
            };

            _jobOfferServiceMock.Setup(s => s.AddJobOfferAsync(jobOfferCreateDto)).ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var result = await _controller.CreateJobOffer(jobOfferCreateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(objectResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal("Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.", returnValue.Message);
        }

        [Fact]
        public async Task UpdateJobOffer_ReturnsOkResult_WithUpdatedJobOffer()
        {
            // Arrange
            var jobOfferUpdateDto = new JobOfferUpdateDto
            {
                Title = "Ingeniero Industrial Updated",
                Description = "Ingeniero Industrial",
                Location = "Bogotá",
                Salary = 40000,
                ContractType = "Tiempo parcial"
            };

            var jobOfferDto = new JobOfferDto
            {
                Id = 1,
                Title = "Ingeniero Industrial Updated",
                Description = "Ingeniero Industrial",
                Location = "Bogotá",
                Salary = 40000,
                ContractType = "Tiempo parcial",
                DatePosted = DateTime.UtcNow
            };

            var response = new GenericResponseDto<JobOfferDto>
            {
                IsSuccess = true,
                Data = jobOfferDto
            };

            _jobOfferServiceMock.Setup(s => s.UpdateJobOfferAsync(1, jobOfferUpdateDto)).ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateJobOffer(1, jobOfferUpdateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(1, returnValue.Data!.Id);
        }

        [Fact]
        public async Task UpdateJobOffer_ReturnsNotFoundResult_WhenJobOfferNotFound()
        {
            // Arrange
            var jobOfferUpdateDto = new JobOfferUpdateDto
            {
                Title = "Ingeniero Industrial Updated",
                Description = "Ingeniero Industrial",
                Location = "Bogotá",
                Salary = 40000,
                ContractType = "Tiempo parcial",
            };

            var response = new GenericResponseDto<JobOfferDto>
            {
                IsSuccess = false,
                Data = null
            };

            _jobOfferServiceMock.Setup(s => s.UpdateJobOfferAsync(1, jobOfferUpdateDto)).ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateJobOffer(1, jobOfferUpdateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(notFoundResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Null(returnValue.Data);
        }

        [Fact]
        public async Task UpdateJobOffer_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var jobOfferUpdateDto = new JobOfferUpdateDto
            {
                Title = "Ingeniero Industrial Updated",
                Description = "Ingeniero Industrial",
                Location = "Bogotá",
                Salary = 40000,
                ContractType = "Tiempo parcial"
            };

            _jobOfferServiceMock.Setup(s => s.UpdateJobOfferAsync(1, jobOfferUpdateDto)).ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var result = await _controller.UpdateJobOffer(1, jobOfferUpdateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(objectResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal("Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.", returnValue.Message);
        }

        [Fact]
        public async Task DeleteJobOffer_ReturnsOkResult_WithDeletedJobOffer()
        {
            // Arrange
            var jobOfferDto = new JobOfferDto
            {
                Id = 1,
                Title = "Ingeniero Industrial",
                Description = "Ingeniero Industrial",
                Location = "Bogotá",
                Salary = 40000,
                ContractType = "Tiempo parcial",
                DatePosted = DateTime.UtcNow
            };

            var response = new GenericResponseDto<JobOfferDto>
            {
                IsSuccess = true,
                Data = jobOfferDto
            };

            _jobOfferServiceMock.Setup(s => s.DeleteJobOfferAsync(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteJobOffer(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(1, returnValue.Data!.Id);
        }

        [Fact]
        public async Task DeleteJobOffer_ReturnsNotFoundResult_WhenJobOfferNotFound()
        {
            // Arrange
            var response = new GenericResponseDto<JobOfferDto>
            {
                IsSuccess = false,
                Data = null
            };

            _jobOfferServiceMock.Setup(s => s.DeleteJobOfferAsync(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteJobOffer(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(notFoundResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Null(returnValue.Data);
        }

        [Fact]
        public async Task DeleteJobOffer_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _jobOfferServiceMock.Setup(s => s.DeleteJobOfferAsync(1)).ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var result = await _controller.DeleteJobOffer(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var returnValue = Assert.IsType<GenericResponseDto<JobOfferDto>>(objectResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal("Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde.", returnValue.Message);
        }

    }
}

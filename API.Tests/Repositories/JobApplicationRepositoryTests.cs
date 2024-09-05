using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API.Tests.Repositories
{
    public class JobApplicationRepositoryTests
    {
        private readonly JobApplicationRepository _repository;
        private readonly Mock<ILogger<JobApplicationRepository>> _loggerMock;
        private readonly AppDbContext _context;

        public JobApplicationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _loggerMock = new Mock<ILogger<JobApplicationRepository>>();
            _repository = new JobApplicationRepository(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllJobApplicationsAsync_ReturnsAllJobApplications()
        {
            // Arrange
            _context.JobApplications.AddRange(new List<JobApplication>
            {
                new JobApplication { CandidateName = "Juan Diaz", CandidateEmail = "juandiaz@gmail.com", JobOfferId = 1 },
                new JobApplication { CandidateName = "David Martinez", CandidateEmail = "davidmartinez@gmail.com", JobOfferId = 2 }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllJobApplicationsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetJobApplicationByIdAsync_ReturnsJobApplication_WhenApplicationExists()
        {
            // Arrange
            var jobApplication = new JobApplication { CandidateName = "Juan Diaz", CandidateEmail = "juandiaz@gmail.com", JobOfferId = 1 };

            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetJobApplicationByIdAsync(jobApplication.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobApplication.Id, result.Id);
        }

        [Fact]
        public async Task AddJobApplicationAsync_AddsJobApplicationToDatabase()
        {
            // Arrange
            var jobApplication = new JobApplication { CandidateName = "Juan Diaz", CandidateEmail = "juandiaz@gmail.com", JobOfferId = 1 };

            // Act
            await _repository.AddJobApplicationAsync(jobApplication);
            var result = await _context.JobApplications.FindAsync(jobApplication.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobApplication.CandidateName, result.CandidateName);
        }

        [Fact]
        public async Task DeleteJobApplicationAsync_RemovesJobApplicationFromDatabase()
        {
            // Arrange
            var jobApplication = new JobApplication { CandidateName = "Juan Diaz", CandidateEmail = "juandiaz@gmail.com", JobOfferId = 1 };

            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteJobApplicationAsync(jobApplication.Id);
            var result = await _context.JobApplications.FindAsync(jobApplication.Id);

            // Assert
            Assert.Null(result);
        }
    }
}


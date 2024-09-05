using System;
using System.Collections.Generic;
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
    public class JobOfferRepositoryTests
    {
        private readonly JobOfferRepository _repository;
        private readonly Mock<ILogger<JobOfferRepository>> _loggerMock;
        private readonly AppDbContext _context;

        public JobOfferRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _loggerMock = new Mock<ILogger<JobOfferRepository>>();
            _repository = new JobOfferRepository(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllJobOffersAsync_ReturnsAllJobOffers()
        {
            // Arrange
            _context.JobOffers.AddRange(new List<JobOffer>
            {
                new JobOffer { Title = "El Tiempo", Description = "Trabajar en el Tiempo", Location = "Bogotà", Salary = 666, ContractType = "Tiempo parcial" },
                new JobOffer { Title = "Google", Description = "Trabajar en Google", Location = "Estados Unidos", Salary = 555, ContractType = "Tiempo completo" }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllJobOffersAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetJobOfferByIdAsync_ReturnsJobOffer_WhenJobOfferExists()
        {
            // Arrange
            var jobOffer = new JobOffer { Title = "El Tiempo", Description = "Trabajar en el Tiempo", Location = "Bogotà", Salary = 666, ContractType = "Tiempo parcial" };
            _context.JobOffers.Add(jobOffer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetJobOfferByIdAsync(jobOffer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobOffer.Id, result.Id);
        }

        [Fact]
        public async Task AddJobOfferAsync_AddsJobOfferToDatabase()
        {
            // Arrange
            var jobOffer = new JobOffer { Title = "El Tiempo", Description = "Trabajar en el Tiempo", Location = "Bogotà", Salary = 666, ContractType = "Tiempo parcial" };

            // Act
            await _repository.AddJobOfferAsync(jobOffer);
            var result = await _context.JobOffers.FindAsync(jobOffer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobOffer.Title, result.Title);
        }

        [Fact]
        public async Task UpdateJobOfferAsync_UpdatesExistingJobOffer()
        {
            // Arrange
            var jobOffer = new JobOffer { Title = "El Tiempo", Description = "Trabajar en el Tiempo", Location = "Bogotá", Salary = 666, ContractType = "Tiempo parcial" };
            _context.JobOffers.Add(jobOffer);
            await _context.SaveChangesAsync();

            jobOffer.Title = "Casa Editorial";

            // Act
            await _repository.UpdateJobOfferAsync(jobOffer);
            var result = await _context.JobOffers.FindAsync(jobOffer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Casa Editorial", result.Title);
        }

        [Fact]
        public async Task DeleteJobOfferAsync_RemovesJobOfferFromDatabase()
        {
            // Arrange
            var jobOffer = new JobOffer { Title = "Google", Description = "Trabajar en Google", Location = "Estados Unidos", Salary = 555, ContractType = "Tiempo completo" };
            _context.JobOffers.Add(jobOffer);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteJobOfferAsync(jobOffer.Id);
            var result = await _context.JobOffers.FindAsync(jobOffer.Id);

            // Assert
            Assert.Null(result);
        }

    }
}

﻿using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration
{
    [Collection("Sequential")]
    public class BundleTests : BaseToursIntegrationTest
    {
        public BundleTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new BundleDto
            {
                Id = 1,
                UserId = -11,
                Name = "Novi",
                Price = 0,
                Status = BundleDto.BundleStatus.Draft,
                Tours = new List<TourDto>()



            };

            // Act
            var result = (ObjectResult)controller.Create(newEntity).Result;

            // Assert - Response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Assert - Database
            var storedEntity = dbContext.Bundles.FirstOrDefault(i => i.Id == newEntity.Id);
            storedEntity.ShouldNotBeNull();

        }

        [Fact]
        public void PublishBundle()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            List<TourDto> tours = new List<TourDto>();
            var tour1 = new TourDto
            {
                Id = 1,
                Name = "ime",
                Description = "naziv",
                Status = API.Dtos.AccountStatus.PUBLISHED, // Assumption: 1 corresponds to Published status in your code
                Difficulty = 1, // Assumption: 1 corresponds to the Difficulty value in your code
                Price = 100.0,
                Tags = new List<string> { "Prva vrednost", "Druga vrednost" }, // Adjust as needed
                Equipment = new List<int>(),
                CheckPoints = new List<long>(),
                AuthorId = -11,
                Objects = new List<long>(), // Add missing field
                FootTime = 1, // Add missing field
                BicycleTime = 1, // Add missing field
                CarTime = 1, // Add missing field
                TotalLength = 1, // Add missing field
                PublishTime = new DateTime(2023, 1, 1, 13, 0, 0, DateTimeKind.Utc),

            };

            var tour2 = new TourDto
            {
                Id = 2,
                Name = "ime",
                Description = "naziv",
                Status = API.Dtos.AccountStatus.PUBLISHED, // Assumption: 1 corresponds to Published status in your code
                Difficulty = 1, // Assumption: 1 corresponds to the Difficulty value in your code
                Price = 100.0,
                Tags = new List<string> { "Prva vrednost", "Druga vrednost" }, // Adjust as needed
                Equipment = new List<int>(),
                CheckPoints = new List<long>(),
                AuthorId = -11,
                Objects = new List<long>(), // Add missing field
                FootTime = 1, // Add missing field
                BicycleTime = 1, // Add missing field
                CarTime = 1, // Add missing field
                TotalLength = 1, // Add missing field
                PublishTime = new DateTime(2023, 1, 1, 13, 0, 0, DateTimeKind.Utc),

            };

            tours.Add(tour1);
            tours.Add(tour2);

            var bundle = new BundleDto
            {
                Id = 1,
                UserId = -11,
                Name = "Novi",
                Price = 100,
                Tours = tours,
                Status = BundleDto.BundleStatus.Draft
            };

            var result = (ObjectResult)controller.PublishBundle(bundle.Id).Result;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Assert - Database
            var storedEntity = dbContext.Bundles.FirstOrDefault(t => t.Name == bundle.Name);
            storedEntity.ShouldNotBeNull();
            storedEntity.Status.ShouldBe(Core.Domain.Bundle.BundleStatus.Published);


        }
        private static BundleController CreateController(IServiceScope scope)
        {
            return new BundleController(scope.ServiceProvider.GetRequiredService<IBundleService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    
}
}

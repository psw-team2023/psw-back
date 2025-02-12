﻿using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases
{
    public class BundleService : CrudService<BundleDto, Bundle>, IBundleService
    {
        private readonly ICrudRepository<Tour> _tourRepository;
        private readonly IBundleRepository _bundleRepository;
        public BundleService(ICrudRepository<Bundle> crudRepository, IMapper mapper, ICrudRepository<Tour> tourRepository, IBundleRepository bundleRepository) : base(crudRepository, mapper)
        {
            _tourRepository = tourRepository;
            _bundleRepository = bundleRepository;
        }

        public override Result<BundleDto> Create(BundleDto bundleDto)
        {
            bundleDto.Price = 0;
            bundleDto.Status = BundleStatus.Draft;
            bundleDto.Tours = new List<int>();

            return base.Create(bundleDto);
        }
        public override Result Delete(int bundleId)
        {

            var existingBundle = _bundleRepository.GetById(bundleId);
            if (existingBundle != null)
            {
                if (existingBundle.Status == Bundle.BundleStatus.Published)
                {
                    return Result.Fail("Bundle status is published, it cannot be deleted");
                }
                _bundleRepository.Delete(bundleId);
                return Result.Ok();
            }
            return Result.Fail("Existing bundle is null");

        }
        public Result<BundleDto> ArchiveBundle(int bundleId)
        {
            var existingBundle = _bundleRepository.GetById(bundleId);
            if (existingBundle != null)
            {
                existingBundle.Status = Bundle.BundleStatus.Archived;
                _bundleRepository.Update(existingBundle);
            }
            return MapToDto(existingBundle);
        }

        public Result<BundleDto> FinishCreatingBundle(int bundleId, double price)
        {
            var existingBundle = _bundleRepository.GetById(bundleId);

            if (existingBundle != null)
            {
                existingBundle.Price = price;
                _bundleRepository.Update(existingBundle);
            }
            return MapToDto(existingBundle);
        }
        public Result<BundleDto> PublishBundle(int bundleId)
        {
            var existingBundle = _bundleRepository.GetById(bundleId);

            if (existingBundle != null)
            {

                int publishedTourCount = 0;
                foreach (int tourId in existingBundle.Tours)
                {
                    var tour = _tourRepository.Get(tourId);
                    if (tour.Status == Domain.AccountStatus.PUBLISHED)
                    {
                        publishedTourCount++;
                    }
                }

                if (publishedTourCount >= 2)
                {
                    existingBundle.Status = Bundle.BundleStatus.Published;
                }
                else
                {
                    existingBundle.Status = Bundle.BundleStatus.Draft;
                }

                _bundleRepository.Update(existingBundle);

                return MapToDto(existingBundle);
            }
            return Result.Fail("Existing bundle cannot be published"); ;

        }

        public List<BundleDto> GetBundlesByAuthorId(int authorId)
        {
            var bundles = _bundleRepository.GetBundlesByAuthorId(authorId);

            // Perform the necessary mapping to DTOs here.
            var bundlesDto = bundles.Select(bundle => new BundleDto
            {
                Name = bundle.Name,
                Price = bundle.Price,
                UserId = (int)bundle.UserId,
                Id = (int)bundle.Id,
                Status = (API.Dtos.BundleStatus)bundle.Status,
                Image = bundle.Image
            }).ToList();

            return bundlesDto;
        }

        public Result<BundleDto> AddTour(BundleDto bundleDto, int tourId)
        {
            try
            {
                Tour tour = _tourRepository.Get(tourId);
                if (bundleDto != null)
                {
                    Bundle bundle = _bundleRepository.GetById(bundleDto.Id);
                    bundle.AddTour((int)tour.Id);

                    bundle.Price += tour.Price;
                    _bundleRepository.Update(bundle);
                    return Result.Ok(bundleDto);
                }
                else
                {
                    return Result.Fail(FailureCode.NotFound).WithError("Tour not found.");
                }

            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
        }

        public Result<BundleDto> RemoveTour(int bundleId, int tourId)
        {
            try
            {
                Bundle bundle = _bundleRepository.GetById(bundleId);
                Tour tour = _tourRepository.Get(tourId);

                bundle.RemoveTour(tourId);
                bundle.Price -= tour.Price;

                _bundleRepository.Update(bundle);
                return Result.Ok();
            }
            catch (ArgumentException e)
            {
                return Result.Fail<BundleDto>(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }

    }
}

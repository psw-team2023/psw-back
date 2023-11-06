﻿using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize]
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-problems")]
    public class TourProblemTouristController : BaseApiController
    {
        private readonly ITourProblemService _tourProblemService;
        private readonly ITourProblemResponseService _problemResponseService;

        public TourProblemTouristController(ITourProblemService tourProblemService, ITourProblemResponseService problemResponseService)
        {
            _tourProblemService = tourProblemService;
            _problemResponseService = problemResponseService;
        }

        [HttpGet]
        public ActionResult<PagedResult<TourProblemDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var userIdClaim = HttpContext.User.Claims.First(x => x.Type == "id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int touristId))
            {

                var result = _tourProblemService.GetByTouristId(touristId);
                return CreateResponse(result);
            }
            else
            {
                return BadRequest("User ID not found or invalid.");
            }
        }

        [HttpPost]
        public ActionResult<TourProblemDto> Create([FromBody] TourProblemDto tourProblem)
        {
            var result = _tourProblemService.Create(tourProblem);
            return CreateResponse(result);
        }

        [HttpPut("{id:int}")]
        public ActionResult<EquipmentDto> Update([FromBody] TourProblemDto tourProblem)
        {
            var result = _tourProblemService.Update(tourProblem);
            return CreateResponse(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _tourProblemService.Delete(id);
            return CreateResponse(result);
        }

        [HttpPost("respond")]
        public ActionResult RespondToProblem([FromBody] TourProblemResponseDto tourProblemResponse)
        {
            var result = _problemResponseService.Create(tourProblemResponse);
            return CreateResponse(result);
        }

        [HttpGet("{problemId:int}/responses")]
        public ActionResult<IEnumerable<TourProblemResponseDto>> GetProblemResponses(int problemId)
        {
            var result = _problemResponseService.GetProblemResponses(problemId);
            return CreateResponse(result);
        }
    }
}
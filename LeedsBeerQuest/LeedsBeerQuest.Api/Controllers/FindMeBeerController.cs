using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeedsBeerQuest.Api.Controllers
{
    [ApiController]
    [Route("Beer")]
    public class FindMeBeerController : ControllerBase
    {
        private readonly IFindMeBeerService _findBeerService;
        private readonly ILogger<FindMeBeerController> _logger;

        public FindMeBeerController(IFindMeBeerService findBeerService, ILogger<FindMeBeerController> logger)
        {
            _findBeerService = findBeerService;
            _logger = logger;
        }

        [HttpGet("nearest-establishments")]
        public async Task<BeerEstablishmentLocation[]> GetNearestEstablishments(double? lat = null, double? lng = null)
        {
            Location? location = null;
            if (lat != null && lng != null)
            {
                location = new Location() { Lat = (double)lat, Long = (double)lng };
            }
            return await _findBeerService.GetNearestBeerLocations(location);
        }
    }
}
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
        [Produces(typeof(BeerEstablishmentLocation[]))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNearestEstablishments(double? lat = null, double? lng = null)
        {
            Location? location = null;
            if (lat != null && lng != null)
            {
                location = new Location() { Lat = (double)lat, Long = (double)lng };
            }
            var locations = await _findBeerService.GetNearestBeerLocations(location);
            return Ok(locations);
        }

        [HttpGet("{establishmentName}")]
        [Produces(typeof(BeerEstablishment))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetBeerEstablishmentByName(string establishmentName)
        {
            var beerEstablishment = await _findBeerService.GetBeerEstablishmentByName(establishmentName);
            if (beerEstablishment == null)
            {
                return NoContent();
            }
            return Ok(beerEstablishment);
        }
    }
}
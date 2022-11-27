using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeedsBeerQuest.Api.Controllers
{
    [ApiController]
    [Route("Beer")]
    [Tags("Find Me Beer!")]
    public class FindMeBeerController : ControllerBase
    {
        private readonly IFindMeBeerService _findBeerService;
        private readonly ILogger<FindMeBeerController> _logger;

        public FindMeBeerController(IFindMeBeerService findBeerService, ILogger<FindMeBeerController> logger)
        {
            _findBeerService = findBeerService;
            _logger = logger;
        }

        /// <summary>
        /// Returns the nearest establishments to a given location
        /// </summary>
        /// <remarks>Returns the location details of establishments nearest to the specified coordinates, in ascending order of distance.
        /// If no coordinates are supplied, a default search location is used instead.</remarks>
        /// <param name="lat">The latitude coordinate to use as the search location</param>
        /// <param name="lng">The longitude coordinate to use as the search location</param>
        /// <returns></returns>
        [HttpGet("nearest-establishments")]
        [Produces(typeof(BeerEstablishmentLocation[]))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNearestEstablishments(double? lat = null, double? lng = null)
        {
            try
            {
                Location? location = CreateLocationModel(lat, lng);
                var locations = await _findBeerService.GetNearestBeerLocations(location);
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting nearest establishments");
                return Problem();
            }
        }

        private static Location? CreateLocationModel(double? lat, double? lng)
        {
            Location? location = null;
            if (lat != null && lng != null)
            {
                location = new Location() { Lat = (double)lat, Long = (double)lng };
            }

            return location;
        }

        /// <summary>
        /// Returns the specified beer establishment
        /// </summary>
        /// <remarks>Returns full details of the beer establishment specified by name in the route.</remarks>
        /// <param name="establishmentName">The name of the establishment to search for</param>
        /// <returns></returns>
        [HttpGet("{establishmentName}")]
        [Produces(typeof(BeerEstablishment))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetBeerEstablishmentByName(string establishmentName)
        {
            try
            {
                var beerEstablishment = await _findBeerService.GetBeerEstablishmentByName(establishmentName);
                if (beerEstablishment == null)
                {
                    return NoContent();
                }
                return Ok(beerEstablishment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred fetching establishment {establishment}", establishmentName);
                return Problem();
            }
        }
    }
}
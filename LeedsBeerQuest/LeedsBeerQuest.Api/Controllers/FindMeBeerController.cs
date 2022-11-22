using LeedsBeerQuest.App.Models.Read;
using Microsoft.AspNetCore.Mvc;

namespace LeedsBeerQuest.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FindMeBeerController : ControllerBase
    {
        private readonly ILogger<FindMeBeerController> _logger;

        public FindMeBeerController(ILogger<FindMeBeerController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/nearest-locations")]
        public IEnumerable<BeerEstablishment> Get(double? lat, double? lng)
        {
            return null;
        }
    }
}
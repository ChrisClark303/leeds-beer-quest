using Castle.Core.Logging;
using LeedsBeerQuest.Api.Controllers;
using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.Api.Controllers
{
    public class FindMeBeerControllerTests
    {
        [Test]
        public async Task GetNearestLocations_CallsFindMeBeerService_Find_WithLocation()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);

            double lat = 51.000;
            double lng = -1.000;
            await controller.GetNearestEstablishments(lat, lng);

            findBeerService.Verify(s => s.GetNearestBeerLocations(It.Is<Location>(l => l.Lat == lat && l.Long == lng)));
        }

        [Test]
        public async Task GetNearestLocations_LocationIsNull_CallsFindMeBeerService_Find_WithNullLocation()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);

            double lat = 51.000;
            double lng = -1.000;
            await controller.GetNearestEstablishments();

            findBeerService.Verify(s => s.GetNearestBeerLocations(null));
        }
    }
}

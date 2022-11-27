using Castle.Core.Logging;
using LeedsBeerQuest.Api.Controllers;
using LeedsBeerQuest.App.Models.Read;
using LeedsBeerQuest.App.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests.Api.Controllers
{
    public class FindMeBeerControllerTests
    {
        [Test]
        public async Task GetNearestLocations_CallsFindMeBeerService_GetNearestEstablishments_WithLocation()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);

            double lat = 51.000;
            double lng = -1.000;
            await controller.GetNearestEstablishments(lat, lng);

            findBeerService.Verify(s => s.GetNearestBeerLocations(It.Is<Location>(l => l.Lat == lat && l.Long == lng)));
        }

        [Test]
        public async Task GetNearestLocations_LocationIsNull_CallsFindMeBeerService_GetNearestEstablishments_WithNullLocation()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);

            await controller.GetNearestEstablishments();

            findBeerService.Verify(s => s.GetNearestBeerLocations(null));
        }

        [Test]
        public async Task GetNearestLocations_Returns_OK_WithResponse_FromBeerService()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            var establishments = new[] { new BeerEstablishmentLocation() };
            findBeerService.Setup(s => s.GetNearestBeerLocations(It.IsAny<Location>()))
                .ReturnsAsync(establishments);

            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);
            var response = await controller.GetNearestEstablishments();

            Assert.That(response, Is.TypeOf<OkObjectResult>());
            var okResponse = (OkObjectResult)response;
            Assert.That(okResponse.Value, Is.EqualTo(establishments));
        }

        [Test]
        public async Task GetNearestLocations_BeerService_ThrowsException_Returns500Response()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            findBeerService.Setup(s => s.GetNearestBeerLocations(It.IsAny<Location>()))
                .Throws<Exception>();

            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);
            var response = await controller.GetNearestEstablishments();

            Assert.That(response, Is.TypeOf<ObjectResult>());
            var objectResult = response as ObjectResult;
            Assert.That(objectResult!.Value, Is.TypeOf<ProblemDetails>());
            var problemDetails = objectResult.Value as ProblemDetails;
            Assert.That(problemDetails!.Status, Is.EqualTo(500));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_CallsFindMeBeerService_GetBeerEstablishmentByName()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);

            await controller.GetBeerEstablishmentByName("The Faversham");

            findBeerService.Verify(s => s.GetBeerEstablishmentByName("The Faversham"));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_Returns_OK_WithResponse_FromBeerService()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            var establishment = new BeerEstablishment();
            findBeerService.Setup(s => s.GetBeerEstablishmentByName(It.IsAny<string>()))
                .ReturnsAsync(establishment);

            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);
            var response = await controller.GetBeerEstablishmentByName("The Faversham");

            Assert.That(response, Is.TypeOf<OkObjectResult>());
            var okResponse = (OkObjectResult)response;
            Assert.That(okResponse.Value, Is.EqualTo(establishment));
        }

        [Test]
        public async Task GetBeerEstablishmentByName_BeerService_ReturnsNull_ReturnsNoContent()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            BeerEstablishment? establishment = null;
            findBeerService.Setup(s => s.GetBeerEstablishmentByName(It.IsAny<string>()))
                .ReturnsAsync(establishment);

            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);
            var response = await controller.GetBeerEstablishmentByName("The Faversham");

            Assert.That(response, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task GetBeerEstablishmentByName_BeerService_ThrowsException_Returns500Response()
        {
            var findBeerService = new Mock<IFindMeBeerService>();
            findBeerService.Setup(s => s.GetBeerEstablishmentByName(It.IsAny<string>()))
                .Throws<Exception>();

            var controller = new FindMeBeerController(findBeerService.Object, new Mock<ILogger<FindMeBeerController>>().Object);
            var response = await controller.GetBeerEstablishmentByName("The Faversham");

            Assert.That(response, Is.TypeOf<ObjectResult>());
            var objectResult = response as ObjectResult;
            Assert.That(objectResult!.Value, Is.TypeOf<ProblemDetails>());
            var problemDetails = objectResult.Value as ProblemDetails;
            Assert.That(problemDetails!.Status, Is.EqualTo(500));
        }
    }
}

using GoTime.ApplicationServices.Impl;
using GoTime.ApplicationServices.Interfaces;
using GoTime.Domain.Enums;
using GoTime.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace GoTimeDomain.Tests
{
    [TestFixture]
    public class ApplicationServiceTests
    {
        [Test]
        public void IGoBoardApplicationService_GetNextBoardPositions_ShouldReturnAEnumerableListOfPositions()
        {
            IGoBoardApplicationService appService = new GoBoardApplicationServiceV1();

            var result = appService.GetNextBoardPositions(new List<Move>(), BoardSize.NineByNine);

            Assert.IsInstanceOf<IEnumerable<Position>>(result);
        }
    }
}

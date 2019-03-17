using Evento;
using GoTime.Adapters;
using GoTime.ApplicationServices.Interfaces;
using GoTime.Domain;
using GoTime.Domain.Aggregrates;
using GoTime.Domain.Commands;
using GoTime.Domain.Enums;
using GoTime.Domain.Events;
using GoTime.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class CommandHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InputCommandHandler_HandleWhenNoAggregrateExists_ShouldAlwaysReturnANewGameAggregate()
        {
            const int expectedNumberOfUncommittedEvents = 1;

            var mockRepo = new Mock<IDomainRepository>();
            mockRepo
                .Setup(x => x.GetById<GameAggregate>(It.IsAny<string>()))
                .Throws(new AggregateNotFoundException("Not found"));

            var cmd = new NewGame();

            var sut = new InputCommandHandler(mockRepo.Object, Mock.Of<IGoBoardApplicationService>());

            var result = sut.Handle(cmd);

            Assert.IsInstanceOf<GameAggregate>(result);
            Assert.AreEqual(expectedNumberOfUncommittedEvents, result.UncommitedEvents().ToList().Count);
        }

        [Test]
        public void GameAggregate_HandleNewGameCommand_ShouldRaiseAGameCreatedV1Event()
        {
            var mockRepo = new Mock<IDomainRepository>();
            mockRepo
                .Setup(x => x.GetById<GameAggregate>(It.IsAny<string>()))
                .Throws(new AggregateNotFoundException("Not found"));

            var cmd = new NewGame();

            var sut = new InputCommandHandler(mockRepo.Object, Mock.Of<IGoBoardApplicationService>());

            var result = sut.Handle(cmd).UncommitedEvents().ToList().First();

            Assert.IsInstanceOf<GameCreatedV1>(result);
        }

        [Test]
        public void GameAggregate_HandleAcceptNewGameCommand_ShouldRaiseAnAcceptNewGameV1Event()
        {
            var aggregate = new GameAggregate();

            var mockRepo = new Mock<IDomainRepository>();
            mockRepo    
                .Setup(x => x.GetById<GameAggregate>(It.IsAny<string>()))
                .Returns(aggregate);

            var cmd = new AcceptNewGame();

            var sut = new InputCommandHandler(mockRepo.Object, Mock.Of<IGoBoardApplicationService>());

            var result = sut.Handle(cmd).UncommitedEvents().ToList().First();

            Assert.IsInstanceOf<AcceptNewGameV1>(result);
        }

        [Test]
        public void GameAggregate_HandleRejectNewGameCommand_ShouldRaiseARejectNewGameV1Event()
        {
            var aggregate = new GameAggregate();

            var mockRepo = new Mock<IDomainRepository>();
            mockRepo
                .Setup(x => x.GetById<GameAggregate>(It.IsAny<string>()))
                .Returns(aggregate);

            var cmd = new RejectNewGame();

            var sut = new InputCommandHandler(mockRepo.Object, Mock.Of<IGoBoardApplicationService>());

            var result = sut.Handle(cmd).UncommitedEvents().ToList().First();

            Assert.IsInstanceOf<RejectNewGameV1>(result);
        }

        [Test]
        public void GameAggregate_HandlePlaceMoveCommand_ShouldRaiseAStonePlacedV1Event()
        {
            var aggregate = new GameAggregate();

            var mockRepo = new Mock<IDomainRepository>();
            mockRepo
                .Setup(x => x.GetById<GameAggregate>(It.IsAny<string>()))
                .Returns(aggregate);

            var cmd = new AddStone();

            var sut = new InputCommandHandler(mockRepo.Object, Mock.Of<IGoBoardApplicationService>());

            var result = sut.Handle(cmd).UncommitedEvents().ToList().First();

            Assert.IsInstanceOf<StonePlacedV1>(result);
        }

        [Test]
        public void GameAggregate_HandlePlaceMoveCommand_ShouldInvokeProduceNextBoardPositionsOnTheInjectedApplicationService()
        {
            var appService = new Mock<IGoBoardApplicationService>();

            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerOneColour = ColourSelection.Black;
            var playerTwoColour = ColourSelection.White;
            var boardSize = BoardSize.ThirteenByThirteen;

            var evt1 = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);
            var evt2 = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);
            var evt3 = new StonePlacedV1(correlationId.ToString(), userId, gameId, "c", 3);
            var evt4 = new StonePlacedV1(correlationId.ToString(), oppontentId, gameId, "d", 4);

            var aggregate = new GameAggregate();
            aggregate.ApplyEvent(evt1);
            aggregate.ApplyEvent(evt2);
            aggregate.ApplyEvent(evt3);
            aggregate.ApplyEvent(evt4);

            var mockRepo = new Mock<IDomainRepository>();
            mockRepo
                .Setup(x => x.GetById<GameAggregate>(It.IsAny<string>()))
                .Returns(aggregate);

            var cmd = new AddStone();

            var sut = new InputCommandHandler(mockRepo.Object, appService.Object);

            var result = sut.Handle(cmd).UncommitedEvents().ToList().First();

            appService.Verify(x => x.GetNextBoardPositions(It.IsAny<IEnumerable<Move>>(), It.IsAny<BoardSize>()), Times.Once);
        }
    }

}
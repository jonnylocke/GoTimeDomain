using GoTime.Domain.Aggregrates;
using GoTime.Domain.Enums;
using GoTime.Domain.Events;
using GoTime.Domain.Exceptions;
using GoTime.Models;
using NUnit.Framework;
using System;

namespace GoTimeDomain.Tests
{
    public class AggregateTests
    {
        // TODO refactor asserts
        // and add setup
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructed_GameAggregate_ShouldApplyCreatedGameV1Event()
        {
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerOneColour = ColourSelection.Black;
            var boardSize = BoardSize.ThirteenByThirteen;

            var evt = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);

            var aggregate = new GameAggregate();
            aggregate.ApplyEvent(evt);

            Assert.AreEqual(correlationId, Guid.Parse(aggregate.AggregateId), "CorrelationId does not match");
            Assert.AreEqual(userId, aggregate.PlayerOne);
            Assert.IsNull(aggregate.PlayerTwo);
            Assert.AreEqual(playerOneColour, aggregate.PlayerOneColour);
            Assert.IsNull(aggregate.PlayerTwoColour);
            Assert.AreEqual(boardSize, aggregate.BoardSize);
            Assert.AreEqual(oppontentId, aggregate.OppontentId, "OppontentId does not match");
            Assert.AreEqual(gameId, aggregate.GameId, "GameId does not match");

            Assert.AreEqual(GameStatus.Created, aggregate.GameStatus);
        }

        [Test]
        public void Constructed_GameAggregate_ShouldApplyPrevioulsEventsAnddAcceptNewGameV1()
        {
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerOneColour = ColourSelection.Black;
            var playerTwoColour = ColourSelection.White;
            var boardSize = BoardSize.ThirteenByThirteen;

            var evt1 = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);
            var evt2 = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);

            var aggregate = new GameAggregate();
            aggregate.ApplyEvent(evt1);
            aggregate.ApplyEvent(evt2);

            Assert.AreEqual(correlationId, Guid.Parse(aggregate.AggregateId), "CorrelationId does not match");

            Assert.AreEqual(userId, aggregate.PlayerOne);
            Assert.IsNotNull(aggregate.PlayerTwo);
            Assert.AreEqual(oppontentId, aggregate.PlayerTwo);

            Assert.AreEqual(playerOneColour, aggregate.PlayerOneColour);
            Assert.IsNotNull(aggregate.PlayerTwoColour);
            Assert.AreEqual(playerTwoColour, aggregate.PlayerTwoColour);

            Assert.AreEqual(boardSize, aggregate.BoardSize);
            Assert.AreEqual(oppontentId, aggregate.OppontentId, "OppontentId does not match");
            Assert.AreEqual(gameId, aggregate.GameId, "GameId does not match");

            Assert.AreEqual(GameStatus.Live, aggregate.GameStatus);
        }

        [Test]
        public void Constructed_GameAggregate_ShouldApplyPrevioulsEventsAnddRejectNewGameV1()
        {
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerOneColour = ColourSelection.Black;
            var playerTwoColour = ColourSelection.White;
            var boardSize = BoardSize.ThirteenByThirteen;

            var evt1 = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);
            var evt2 = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);
            var evt3 = new RejectNewGameV1(correlationId.ToString(), oppontentId, gameId);

            var aggregate = new GameAggregate();
            aggregate.ApplyEvent(evt1);
            aggregate.ApplyEvent(evt2);
            aggregate.ApplyEvent(evt3);

            Assert.AreEqual(correlationId, Guid.Parse(aggregate.AggregateId), "CorrelationId does not match");

            Assert.AreEqual(userId, aggregate.PlayerOne);
            Assert.IsNotNull(aggregate.PlayerTwo);
            Assert.AreEqual(oppontentId, aggregate.PlayerTwo);

            Assert.AreEqual(playerOneColour, aggregate.PlayerOneColour);
            Assert.IsNotNull(aggregate.PlayerTwoColour);
            Assert.AreEqual(playerTwoColour, aggregate.PlayerTwoColour);

            Assert.AreEqual(boardSize, aggregate.BoardSize);
            Assert.AreEqual(oppontentId, aggregate.OppontentId, "OppontentId does not match");
            Assert.AreEqual(gameId, aggregate.GameId, "GameId does not match");

            Assert.AreEqual(GameStatus.Rejected, aggregate.GameStatus);
        }

        [Test]
        public void Constructed_GameAggregate_ShouldApplyPrevioulsEventsAnddStonePlacedV1vent()
        {
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerOneColour = ColourSelection.Black;
            var playerTwoColour = ColourSelection.White;
            var boardSize = BoardSize.ThirteenByThirteen;

            var expectedMoveNumber = 1;

            var expectedPosition = Move.New();
            expectedPosition.X = 4;
            expectedPosition.Y = 3;

            var lastMove = Move.New();
            lastMove.PlayerId = userId;
            lastMove.Number = expectedMoveNumber;
            //lastMove.Position = expectedPosition;

            var evt1 = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);
            var evt2 = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);
            var evt3 = new StonePlacedV1(correlationId.ToString(), userId, gameId, "c", 3);

            var aggregate = new GameAggregate();
            aggregate.ApplyEvent(evt1);
            aggregate.ApplyEvent(evt2);
            aggregate.ApplyEvent(evt3);

            Assert.AreEqual(correlationId, Guid.Parse(aggregate.AggregateId), "CorrelationId does not match");

            Assert.AreEqual(userId, aggregate.PlayerOne);
            Assert.IsNotNull(aggregate.PlayerTwo);
            Assert.AreEqual(oppontentId, aggregate.PlayerTwo);

            Assert.AreEqual(playerOneColour, aggregate.PlayerOneColour);
            Assert.IsNotNull(aggregate.PlayerTwoColour);
            Assert.AreEqual(playerTwoColour, aggregate.PlayerTwoColour);

            Assert.AreEqual(boardSize, aggregate.BoardSize);
            Assert.AreEqual(oppontentId, aggregate.OppontentId, "OppontentId does not match");
            Assert.AreEqual(gameId, aggregate.GameId, "GameId does not match");

            Assert.AreEqual(GameStatus.Live, aggregate.GameStatus);

            Assert.AreEqual(userId, aggregate.LastMove.PlayerId);
            Assert.AreEqual(expectedMoveNumber, aggregate.LastMove.Number);

            Assert.AreEqual(expectedPosition.X, aggregate.LastMove.X);
            Assert.AreEqual(expectedPosition.Y, aggregate.LastMove.Y);

        }

        [Test]
        public void Constructed_GameAggregate_ShouldIncrementMoveNumberEveryTimedStonePlacedV1ventIsApplied()
        {
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerOneColour = ColourSelection.Black;
            var playerTwoColour = ColourSelection.White;
            var boardSize = BoardSize.ThirteenByThirteen;

            var expectedMoveNumber = 2;

            //var expectedPosition = Move.New();
            //expectedPosition.X = 5;
            //expectedPosition.Y = 4;

            var evt1 = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);
            var evt2 = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);
            var evt3 = new StonePlacedV1(correlationId.ToString(), userId, gameId, "c", 3);
            var evt4 = new StonePlacedV1(correlationId.ToString(), oppontentId, gameId, "e", 3);

            var aggregate = new GameAggregate();
            aggregate.ApplyEvent(evt1);
            aggregate.ApplyEvent(evt2);
            aggregate.ApplyEvent(evt3);
            aggregate.ApplyEvent(evt4);

            Assert.AreEqual(correlationId, Guid.Parse(aggregate.AggregateId), "CorrelationId does not match");

            Assert.AreEqual(userId, aggregate.PlayerOne);
            Assert.IsNotNull(aggregate.PlayerTwo);
            Assert.AreEqual(oppontentId, aggregate.PlayerTwo);

            Assert.AreEqual(playerOneColour, aggregate.PlayerOneColour);
            Assert.IsNotNull(aggregate.PlayerTwoColour);
            Assert.AreEqual(playerTwoColour, aggregate.PlayerTwoColour);

            Assert.AreEqual(boardSize, aggregate.BoardSize);
            Assert.AreEqual(oppontentId, aggregate.OppontentId, "OppontentId does not match");
            Assert.AreEqual(gameId, aggregate.GameId, "GameId does not match");

            Assert.AreEqual(GameStatus.Live, aggregate.GameStatus);

            Assert.AreEqual(oppontentId, aggregate.LastMove.PlayerId);
            Assert.AreEqual(expectedMoveNumber, aggregate.LastMove.Number);
        }

        [Test]
        public void GameAggregate_ApplyStonePlaceV1_ShouldThrowAnIllegalMoveExceptionWhenThePlayerIdMatchesThePreviousPlayerId()
        {
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerOneColour = ColourSelection.Black;
            var playerTwoColour = ColourSelection.White;
            var boardSize = BoardSize.ThirteenByThirteen;

            //var expectedPosition = Move.New();
            //expectedPosition.X = "d";
            //expectedPosition.Y = 4;

            var evt1 = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);
            var evt2 = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);
            var evt3 = new StonePlacedV1(correlationId.ToString(), userId, gameId, "c", 3);
            var evt4 = new StonePlacedV1(correlationId.ToString(), userId, gameId, "d", 3);

            var aggregate = new GameAggregate();
            aggregate.ApplyEvent(evt1);
            aggregate.ApplyEvent(evt2);
            aggregate.ApplyEvent(evt3);

            Assert.Throws<IllegalMoveException>(() => aggregate.ApplyEvent(evt4));
        }
    }
}
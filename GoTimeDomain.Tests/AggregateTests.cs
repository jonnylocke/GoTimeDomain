using Evento;
using GoTime.Adapters;
using GoTime.Domain;
using GoTime.Domain.Aggregrates;
using GoTime.Domain.Commands;
using GoTime.Domain.Enums;
using GoTime.Domain.Events;
using GoTime.Domain.Exceptions;
using GoTime.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    public class AggregateTests
    {
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

            var expectedPosition = Position.New();
            expectedPosition.X = "c";
            expectedPosition.Y = 3;

            var lastMove = Move.New();
            lastMove.PlayerId = userId;
            lastMove.Number = expectedMoveNumber;
            lastMove.Position = expectedPosition;

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

            Assert.AreEqual(expectedPosition.X, aggregate.LastMove.Position.X);
            Assert.AreEqual(expectedPosition.Y, aggregate.LastMove.Position.Y);

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

            var expectedPosition = Position.New();
            expectedPosition.X = "d";
            expectedPosition.Y = 4;

            var evt1 = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);
            var evt2 = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);
            var evt3 = new StonePlacedV1(correlationId.ToString(), userId, gameId, "c", 3);
            var evt4 = new StonePlacedV1(correlationId.ToString(), oppontentId, gameId, expectedPosition.X, expectedPosition.Y);

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

            Assert.AreEqual(expectedPosition.X, aggregate.LastMove.Position.X);
            Assert.AreEqual(expectedPosition.Y, aggregate.LastMove.Position.Y);
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

            var expectedPosition = Position.New();
            expectedPosition.X = "d";
            expectedPosition.Y = 4;

            var evt1 = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);
            var evt2 = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);
            var evt3 = new StonePlacedV1(correlationId.ToString(), userId, gameId, "c", 3);
            var evt4 = new StonePlacedV1(correlationId.ToString(), userId, gameId, expectedPosition.X, expectedPosition.Y);

            var aggregate = new GameAggregate();
            aggregate.ApplyEvent(evt1);
            aggregate.ApplyEvent(evt2);
            aggregate.ApplyEvent(evt3);

            Assert.Throws<IllegalMoveException>(() => aggregate.ApplyEvent(evt4));
        }
    }
}
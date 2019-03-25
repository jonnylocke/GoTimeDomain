using GoTime.Domain.Enums;
using GoTime.Domain.Events;
using GoTime.Models;
using NUnit.Framework;
using System;

namespace GoTimeDomain.Tests
{
    public class EventTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructed_CreatedGameV1Event_ShouldHaveFilledExpectedProperties()
        {
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerOneColour = ColourSelection.Black;
            var boardSize = BoardSize.ThirteenByThirteen;

            var evt = new GameCreatedV1(correlationId.ToString(), userId, oppontentId, gameId, playerOneColour, boardSize);

            Assert.AreEqual(correlationId, Guid.Parse(evt.Metadata["$correlationId"]), correlationId.ToString(), "Metadata correlationId does not match");
            Assert.AreEqual(correlationId, Guid.Parse(evt.CorrelationId), "CorrelationId does not match");
            Assert.AreEqual(userId, evt.PlayerOne, "UserId does not match");
            Assert.AreEqual(oppontentId, evt.OppontentId, "OppontentId does not match");
            Assert.AreEqual(gameId, evt.GameId, "GameId does not match");
            Assert.AreEqual(playerOneColour, evt.Colour);
            Assert.AreEqual(boardSize, evt.BoardSize);
        }

        [Test]
        public void Constructed_AcceptNewGameV1_ShouldHaveFilledExpectedProperties()
        {
            var correlationId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerTwoColour = ColourSelection.White;

            var evt = new AcceptNewGameV1(correlationId.ToString(), oppontentId, gameId, playerTwoColour);

            Assert.AreEqual(correlationId, Guid.Parse(evt.Metadata["$correlationId"]), correlationId.ToString(), "Metadata correlationId does not match");
            Assert.AreEqual(correlationId, Guid.Parse(evt.CorrelationId), "CorrelationId does not match");
            Assert.AreEqual(oppontentId, evt.OppontentId, "OppontentId does not match");
            Assert.AreEqual(gameId, evt.GameId, "GameId does not match");
            Assert.AreEqual(playerTwoColour, evt.Colour);
        }

        [Test]
        public void Constructed_RejectNewGameV1_ShouldHaveFilledExpectedProperties()
        {
            var correlationId = Guid.NewGuid();
            var oppontentId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            var evt = new RejectNewGameV1(correlationId.ToString(), oppontentId, gameId);

            Assert.AreEqual(correlationId, Guid.Parse(evt.Metadata["$correlationId"]), correlationId.ToString(), "Metadata correlationId does not match");
            Assert.AreEqual(correlationId, Guid.Parse(evt.CorrelationId), "CorrelationId does not match");
            Assert.AreEqual(oppontentId, evt.OppontentId, "OppontentId does not match");
            Assert.AreEqual(gameId, evt.GameId, "GameId does not match");
        }

        [Test]
        public void Constructed_MoveCompletedV1_ShouldHaveFilledExpectedProperties()
        {
            var correlationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var x = "c";
            var y = 3;

            var evt = new StonePlacedV1(correlationId.ToString(), userId, gameId, x, y);

            Assert.AreEqual(correlationId, Guid.Parse(evt.Metadata["$correlationId"]), correlationId.ToString(), "Metadata correlationId does not match");
            Assert.AreEqual(correlationId, Guid.Parse(evt.CorrelationId), "CorrelationId does not match");
            Assert.AreEqual(userId, evt.PlayerId, "PlayerId does not match");
            Assert.AreEqual(gameId, evt.GameId, "GameId does not match");
            Assert.AreEqual(x, evt.XPosition);
            Assert.AreEqual(y, evt.YPosition);
        }
    }
}
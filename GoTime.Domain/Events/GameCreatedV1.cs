using Evento;
using GoTime.Domain.Enums;
using GoTime.Models;
using System;
using System.Collections.Generic;

namespace GoTime.Domain.Events
{
    internal class GameCreatedV1 : Event
    {
        public GameCreatedV1(string correlationId, Guid userId, Guid opponentId, Guid gameId, ColourSelection colourSelection, BoardSize boardSize)
        {
            CorrelationId = correlationId;
            PlayerOne = userId;
            OppontentId = opponentId;
            GameId = gameId;
            Colour = colourSelection;
            BoardSize = boardSize;

            Metadata = new Dictionary<string, string>
            {
                { "$correlationId", correlationId }
            };
        }

        public IDictionary<string, string> Metadata { get; set; }
        public string CorrelationId { get; private set; }
        public Guid PlayerOne { get; private set; }
        public Guid OppontentId { get; private set; }
        public Guid GameId { get; private set; }
        public ColourSelection Colour { get; private set; }
        public BoardSize BoardSize { get; private set; }
    }
}

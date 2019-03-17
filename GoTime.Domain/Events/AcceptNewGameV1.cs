using Evento;
using GoTime.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoTime.Domain.Events
{
    public class AcceptNewGameV1 : Event
    {
        public AcceptNewGameV1(string correlationId, Guid opponentId, Guid gameId, ColourSelection colour)
        {
            CorrelationId = correlationId;
            OppontentId = opponentId;
            GameId = gameId;
            Colour = colour;

            Metadata = new Dictionary<string, string>
            {
                { "$correlationId", correlationId }
            };
        }

        public IDictionary<string, string> Metadata { get; set; }

        public string CorrelationId { get; private set; }
        public Guid OppontentId { get; private set; }
        public Guid GameId { get; private set; }
        public ColourSelection Colour { get; private set; }
    }
}

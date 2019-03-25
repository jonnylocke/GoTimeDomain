using Evento;
using GoTime.Models;
using System;
using System.Collections.Generic;

namespace GoTime.Domain.Events
{
    public class StonePlacedV1 : Event
    {
        public StonePlacedV1(string correlationId, Guid playerId, Guid gameId, string x, int y)
        {
            CorrelationId = correlationId;
            PlayerId = playerId;
            GameId = gameId;
            XPosition = x;
            YPosition = y;

            Metadata = new Dictionary<string, string>
            {
                { "$correlationId", correlationId }
            };
        }

        public IDictionary<string, string> Metadata { get; set; }

        public string CorrelationId { get; private set; }
        public Guid PlayerId { get; private set; }
        public Guid GameId { get; private set; }
        public ColourSelection Colour { get; private set; }
        public string XPosition { get; private set; }
        public int YPosition { get; private set; }
    }
}

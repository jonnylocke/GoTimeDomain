using Evento;
using GoTime.Domain.Enums;
using GoTime.Models;
using System;
using System.Collections.Generic;

namespace GoTime.Domain.Commands
{
    public class NewGame : Command
    {
        public IDictionary<string, string> Metadata { get; set; }
        public Guid GameId { get; set; }
        public string CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public Guid OpponentId { get; set; }
        
        public BoardSize BoardSize { get; set; }
        public ColourSelection PlayerColour  { get; set; }

        public void SetCorrelationId(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public void SetNewGameId()
        {
            GameId = Guid.NewGuid();
        }
    }
}

using Evento;
using System;
using System.Collections.Generic;

namespace GoTime.Domain.Commands
{
    public class RejectNewGame : Command
    {
        public IDictionary<string, string> Metadata { get; set;}

        public string CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
    }
}

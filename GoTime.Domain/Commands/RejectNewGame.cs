using Evento;
using System;
using System.Collections.Generic;

namespace GoTime.Domain.Commands
{
    public class RejectNewGame : Command
    {
        public IDictionary<string, string> Metadata { get; set;}

        public string CorrelationId { get; set; }
        public Guid UserId { get; private set; }
        public Guid GameId { get; private set; }
    }
}

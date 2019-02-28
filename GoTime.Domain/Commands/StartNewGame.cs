using Evento;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoTime.Domain.Commands
{
    public class StartNewGame : Command
    {
        public IDictionary<string, string> Metadata => throw new NotImplementedException();
    }
}

using System;
using Evento;
using GoTime.Domain.Commands;

namespace GoTime.Adapters
{
    public class InputCommandHandler : 
        IHandle<StartNewGame>,
        IHandle<AcceptNewGame>,
        IHandle<RejectNewGame>,
        IHandle<MovePiece>
    {
        private IDomainRepository _repo;

        public InputCommandHandler(IDomainRepository repo)
        {
            _repo = repo;
        }

        public IAggregate Handle(StartNewGame command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(AcceptNewGame command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(RejectNewGame command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(MovePiece command)
        {
            throw new NotImplementedException();
        }
    }
}

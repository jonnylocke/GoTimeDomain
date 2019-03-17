using Evento;
using GoTime.ApplicationServices.Interfaces;
using GoTime.Domain.Aggregrates;
using GoTime.Domain.Commands;

namespace GoTime.Adapters
{
    public class InputCommandHandler : 
        IHandle<NewGame>,
        IHandle<AcceptNewGame>,
        IHandle<RejectNewGame>,
        IHandle<AddStone>
    {
        private IDomainRepository _repo;
        private IGoBoardApplicationService _appService;

        public InputCommandHandler(IDomainRepository repo, IGoBoardApplicationService appService)
        {
            _repo = repo;
            _appService = appService;
        }

        public IAggregate Handle(NewGame command)
        {
            try
            {
                var aggregrate = _repo.GetById<GameAggregate>(command.CorrelationId);

                return aggregrate;
            }
            catch (AggregateNotFoundException)
            {
                return GameAggregate.Create(command);
            }
        }

        public IAggregate Handle(AcceptNewGame command)
        {
            try
            {
                var aggregrate = _repo.GetById<GameAggregate>(command.CorrelationId);
                aggregrate.AcceptGameChallenge(command);

                return aggregrate;
            }
            catch (AggregateNotFoundException)
            {
                // log
                return null;
            }
        }

        public IAggregate Handle(RejectNewGame command)
        {
            try
            {
                var aggregrate = _repo.GetById<GameAggregate>(command.CorrelationId);
                aggregrate.RejectGameChallenge(command);

                return aggregrate;
            }
            catch (AggregateNotFoundException)
            {
                // log
                return null;
            }
        }

        public IAggregate Handle(AddStone command)
        {
            try
            {
                var aggregrate = _repo.GetById<GameAggregate>(command.CorrelationId);
                aggregrate.PlaceStone(command);
                aggregrate.UpdateBoardPositionsAvailable(_appService);

                return aggregrate;
            }
            catch (AggregateNotFoundException)
            {
                // log
                return null;
            }
        }
    }
}

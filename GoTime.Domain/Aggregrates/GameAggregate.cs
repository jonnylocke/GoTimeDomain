using Evento;
using GoTime.ApplicationServices.Interfaces;
using GoTime.Domain.Commands;
using GoTime.Domain.Enums;
using GoTime.Domain.Events;
using GoTime.Domain.Exceptions;
using GoTime.Models;
using System;
using System.Collections.Generic;

namespace GoTime.Domain.Aggregrates
{
    public class GameAggregate : AggregateBase
    {
        public override string AggregateId => CorrelationId;

        private string CorrelationId { get; set; }
        private IList<Move> _allMoves { get; set; }

        public Guid OppontentId { get; private set; }
        public Guid GameId { get; private set; }
        public Guid PlayerOne { get; private set; }
        public Guid? PlayerTwo { get; private set; }
        public ColourSelection PlayerOneColour { get; private set; }
        public ColourSelection? PlayerTwoColour { get; private set; }
        public BoardSize BoardSize { get; private set; }
        public GameStatus GameStatus { get; private set; }
        public Move LastMove{ get; private set; }


        public GameAggregate()
        {
            _allMoves = new List<Move>();

            RegisterTransition<GameCreatedV1>(Apply);
            RegisterTransition<AcceptNewGameV1>(Apply);
            RegisterTransition<RejectNewGameV1>(Apply);
            RegisterTransition<StonePlacedV1>(Apply);
        }

        #region Apply Events

        private void Apply(GameCreatedV1 obj)
        {
            CorrelationId = obj.CorrelationId;
            PlayerOne = obj.PlayerOne;
            OppontentId = obj.OppontentId;
            GameId = obj.GameId;
            PlayerOneColour = obj.Colour;   
            BoardSize = obj.BoardSize;
            GameStatus = GameStatus.Created;
        }

        private void Apply(AcceptNewGameV1 obj)
        {
            PlayerTwo = obj.OppontentId;
            PlayerTwoColour = obj.Colour;
            GameStatus = GameStatus.Live;
        }

        private void Apply(RejectNewGameV1 obj)
        {
            GameStatus = GameStatus.Rejected;
        }

        private void Apply(StonePlacedV1 obj)
        {
            if (LastMove == null)
            {
                LastMove = Move.New();
            }

            if (LastMove.PlayerId != Guid.Empty)
            {
                if (LastMove.PlayerId == obj.PlayerId)
                {
                    throw new IllegalMoveException($"A consecutive move was attempted by Player Id '{LastMove.PlayerId}'");
                }
            }

            LastMove.Update(obj.PlayerId, obj.XPosition, obj.YPosition, obj.Colour);

            _allMoves.Add(LastMove);
        }

        public void UpdateBoardPositionsAvailable(IGoBoardApplicationService appService)
        {
            appService.GetNextBoardPositions(_allMoves, BoardSize);
        }


        #endregion


        private GameAggregate(string correlationId, Guid userId, Guid opponentId, Guid gameId, ColourSelection colour, BoardSize boardSize) : this ()
        {
            RaiseEvent(new GameCreatedV1(correlationId, userId, opponentId, gameId, colour, boardSize));
        }
        
        public static IAggregate Create(NewGame cmd)
        {
            return new GameAggregate(cmd.CorrelationId, cmd.UserId, cmd.OpponentId, cmd.GameId, cmd.PlayerColour, cmd.BoardSize);
        }

        public void AcceptGameChallenge(AcceptNewGame cmd)
        {
            var colour = ColourSelection.Black;
            if (PlayerOneColour == ColourSelection.Black)
            {
                colour = ColourSelection.White;
            }
            
            RaiseEvent(new AcceptNewGameV1(cmd.CorrelationId, cmd.OpponentId, cmd.GameId, colour));
        }

        public void RejectGameChallenge(RejectNewGame cmd)
        {
            RaiseEvent(new RejectNewGameV1(cmd.CorrelationId, cmd.UserId, cmd.GameId));
        }

        public void PlaceStone(AddStone cmd)
        {
            RaiseEvent(new StonePlacedV1(cmd.CorrelationId, cmd.UserId, cmd.GameId, cmd.PositionX, cmd.PositionY));

            var currentMove = Move.New();
            currentMove.Update(cmd.UserId, cmd.PositionX, cmd.PositionY, ColourSelection.Black); // <-- TODO need to get the colour and test

            _allMoves.Add(currentMove);
        }
    }
}

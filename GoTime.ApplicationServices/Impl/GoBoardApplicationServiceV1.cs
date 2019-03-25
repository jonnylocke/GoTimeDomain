using GoTime.ApplicationServices.Interfaces;
using GoTime.Models;
using System;
using System.Collections.Generic;

namespace GoTime.ApplicationServices.Impl
{
    public class GoBoardApplicationServiceV1 : IGoBoardApplicationService
    {

        public BoardPosition[,] GetNextBoardPositions(IEnumerable<Move> moves, BoardSize boardSize)
        {
            var retVal = SetupEmptyBoard(GetBoardDimension(boardSize));

            foreach (var move in moves)
            {
                SetPositionState(retVal[move.X, move.Y], move.StoneColour);
            }

            return retVal;
        }

        private void SetPositionState(BoardPosition boardPosition, ColourSelection stoneColour)
        {
            var positionState = PositionState.OccupiedByBlack;

            if (stoneColour == ColourSelection.White)
            {
                positionState = PositionState.OccupiedByWhite;
            }

            boardPosition.SetPositionStatue(positionState);
        }


        private int GetBoardDimension(BoardSize boardSize)
        {
            if (boardSize == BoardSize.ThirteenByThirteen)
            {
                return 13;
            }

            if (boardSize == BoardSize.NineteenByNineteen)
            {
                return 19;
            }

            return 9;
        }

        private BoardPosition[,] SetupEmptyBoard(int boardDimension)
        {
            var retVal = new BoardPosition[boardDimension, boardDimension];

            for (int i = 0; i < boardDimension; i++)
            {
                for (int j = 0; j < boardDimension; j++)
                {
                    retVal.SetValue(BoardPosition.New(), i, j);
                }
            }

            return retVal;
        }

    }
}

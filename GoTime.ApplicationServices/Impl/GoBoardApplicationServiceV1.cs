using GoTime.ApplicationServices.Interfaces;
using GoTime.Models;
using System.Collections.Generic;

namespace GoTime.ApplicationServices.Impl
{
    public class GoBoardApplicationServiceV1 : IGoBoardApplicationService
    {
        //public IEnumerable<Position> GetNextBoardPositions(IEnumerable<Move> allMoves)
        //{
            
        //}

        public IEnumerable<Position> GetNextBoardPositions(IEnumerable<Move> list, BoardSize nineByNine)
        {
            return new List<Position>();
        }
    }
}

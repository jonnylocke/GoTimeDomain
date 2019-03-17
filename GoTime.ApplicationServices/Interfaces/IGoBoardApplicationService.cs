using GoTime.Models;
using System.Collections.Generic;

namespace GoTime.ApplicationServices.Interfaces
{
    public interface IGoBoardApplicationService
    {
        //IEnumerable<Position> GetNextBoardPositions(List<Move> list, IEnumerable<Move> allMoves);
        IEnumerable<Position> GetNextBoardPositions(IEnumerable<Move> list, BoardSize nineByNine);
    }
}

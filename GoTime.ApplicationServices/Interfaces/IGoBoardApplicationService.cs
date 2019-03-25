using GoTime.Models;
using System.Collections.Generic;

namespace GoTime.ApplicationServices.Interfaces
{
    public interface IGoBoardApplicationService
    {
        BoardPosition[,] GetNextBoardPositions(IEnumerable<Move> moves, BoardSize boardSize);
    }
}

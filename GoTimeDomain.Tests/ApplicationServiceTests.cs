using GoTime.ApplicationServices.Impl;
using GoTime.ApplicationServices.Interfaces;
using GoTime.Domain.Enums;
using GoTime.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoTimeDomain.Tests
{
    [TestFixture]
    public class ApplicationServiceTests
    {
        [Test]
        public void IGoBoardApplicationService_GetNextBoardPositions_ShouldReturnATwoDimensionaleArrayOfBoardPositionObjects()
        {
            IGoBoardApplicationService appService = new GoBoardApplicationServiceV1();

            var result = appService.GetNextBoardPositions(new List<Move>(), BoardSize.NineByNine);

            Assert.IsInstanceOf<BoardPosition [,]> (result);
        }

        [Test]
        public void GoBoardApplicationServiceV1_GetNextBoardPositionsForNineByNine_ShouldReturnAListOf81Positions()
        {
            const int expectedNumberOfPositions = 81;
            var appService = new GoBoardApplicationServiceV1();

            var result = appService.GetNextBoardPositions(new List<Move>(), BoardSize.NineByNine);

            AssertPositions(expectedNumberOfPositions, result);
        }

        [Test]
        public void GoBoardApplicationServiceV1_GetNextBoardPositionsForThirteenByThirteen_ShouldReturnAListOf169Positions()
        {
            const int expectedNumberOfPositions = 169;
            var appService = new GoBoardApplicationServiceV1();

            var result = appService.GetNextBoardPositions(new List<Move>(), BoardSize.ThirteenByThirteen);

            AssertPositions(expectedNumberOfPositions, result);
        }

        [Test]
        public void GoBoardApplicationServiceV1_GetNextBoardPositionsForNineteenByNineteen_ShouldReturnAListOf361Positions()
        {
            const int expectedNumberOfPositions = 361;
            var appService = new GoBoardApplicationServiceV1();

            var result = appService.GetNextBoardPositions(new List<Move>(), BoardSize.NineteenByNineteen);

            AssertPositions(expectedNumberOfPositions, result);
        }

        [TestCase(BoardSize.NineByNine, 80)]
        [TestCase(BoardSize.ThirteenByThirteen, 168)]
        [TestCase(BoardSize.NineteenByNineteen, 360)]
        public void GetNextBoardPositionsForBoardSize_WithOneStonePlaced_ShouldReturnTheCorrectNumberOfEmptySpaces(BoardSize boardSize, int expectedNumberOfEmptyPositions)
        {
            var appService = new GoBoardApplicationServiceV1();

            var moves = new List<Move>();
            var move1 = Move.New();
            move1.Update(Guid.NewGuid(), "a", 1, ColourSelection.Black);
            moves.Add(move1);

            var result = appService.GetNextBoardPositions(moves, boardSize);

            int count = 0;
            foreach (var item in result)
            {
                if (item.PositionState == PositionState.Empty)
                {
                    count++;
                }
            }

            Assert.AreEqual(expectedNumberOfEmptyPositions, count, $"for board {boardSize}");
        }

        [TestCase(BoardSize.NineByNine, "b", 8, ColourSelection.White, PositionState.OccupiedByWhite)]
        [TestCase(BoardSize.ThirteenByThirteen, "j", 9, ColourSelection.Black, PositionState.OccupiedByBlack)]
        [TestCase(BoardSize.NineteenByNineteen, "s", 18, ColourSelection.White, PositionState.OccupiedByWhite)]
        public void GetNextBoardPositionsForBoardSize_WithOneStonePlaced_ShouldPlaceTheCorrectColourInTheCorrectPosition(BoardSize boardSize, string x, int y, ColourSelection colourSelection, PositionState expectedState)
        {
            var expectedXPosition = x.LetterCoordinateToNumber();
            var expectedYPosition = y;

            var appService = new GoBoardApplicationServiceV1();

            var moves = new List<Move>();
            var move1 = Move.New();
            move1.Update(Guid.NewGuid(), x, y, colourSelection);
            moves.Add(move1);

            var boardPositions = appService.GetNextBoardPositions(moves, boardSize);
            var result = boardPositions[expectedXPosition, expectedYPosition];

            Assert.AreEqual(expectedState, result.PositionState, $"for board {boardSize}");
        }

        private void AssertPositions(int expectedNumberOfPositions, BoardPosition[,] positions)
        {
            Assert.AreEqual(expectedNumberOfPositions, GetTotalPositions(positions));

            int count = 0;
            foreach (var item in positions)
            {
                Assert.IsNotNull(item);
                count++;
            }

            Assert.AreEqual(expectedNumberOfPositions, count);
        }

        private int GetTotalPositions(BoardPosition[,] positions)
        {
            return positions.GetLength(0) * positions.GetLength(1);
        }

    }
}

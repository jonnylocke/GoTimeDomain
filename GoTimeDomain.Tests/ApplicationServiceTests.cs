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

        [TestCase(BoardSize.NineByNine, "b", 8, ColourSelection.White, PositionState.OccupiedByWhite, ColourSelection.Black, PositionState.OccupiedByBlack, "c", 7, 79)]
        [TestCase(BoardSize.ThirteenByThirteen, "j", 9, ColourSelection.Black, PositionState.OccupiedByBlack, ColourSelection.White, PositionState.OccupiedByWhite, "b", 7, 166)]
        [TestCase(BoardSize.NineteenByNineteen, "s", 18, ColourSelection.White, PositionState.OccupiedByWhite, ColourSelection.Black, PositionState.OccupiedByBlack, "s", 16, 358)]

        public void GetNextBoardPositionsForBoardSize_WithTwoStonesPlaced_ShouldPlaceEachColourInTheCorrectPositionAndReturnTheCorrectNumberOfEmptySpaces(
            BoardSize boardSize, string x1, int y1, ColourSelection colourSelection1, PositionState expectedState1, ColourSelection colourSelection2, PositionState expectedState2, string x2, int y2, int numberOfEmptySpacesLeft)
        {
            var expectedXPosition1 = x1.LetterCoordinateToNumber();
            var expectedYPosition1 = y1;

            var expectedXPosition2 = x2.LetterCoordinateToNumber();
            var expectedYPosition2 = y2;

            var appService = new GoBoardApplicationServiceV1();
            
            var move1 = Move.New();
            move1.Update(Guid.NewGuid(), x1, y1, colourSelection1);
            

            var move2 = Move.New();
            move2.Update(Guid.NewGuid(), x2, y2, colourSelection2);

            var moves = new List<Move>();
            moves.Add(move1);
            moves.Add(move2);

            // act
            var boardPositions = appService.GetNextBoardPositions(moves, boardSize);

            var result1 = boardPositions[expectedXPosition1, expectedYPosition1];
            Assert.AreEqual(expectedState1, result1.PositionState, $"for board {boardSize}");

            var result2 = boardPositions[expectedXPosition2, expectedYPosition2];
            Assert.AreEqual(expectedState2, result2.PositionState, $"for board {boardSize}");
        }

        [Test]
        public void GetNextBoardPositions_WhenStonesOfOneColourWithLibertiesFreeOccupyAnAreaLeavingOneSpaceFree_ShouldMarkThatSpaceWithPositionStateOfSuicideForTheOpposingColour()
        {
            var expectedXPosition = "b".LetterCoordinateToNumber();
            var expectedYPosition = 2;

            var appService = new GoBoardApplicationServiceV1();
            
            var move1 = Move.New();
            move1.Update(Guid.NewGuid(), "b", 1, ColourSelection.Black);
            var move2 = Move.New();
            move2.Update(Guid.NewGuid(), "a", 2, ColourSelection.Black);
            var move3 = Move.New();
            move3.Update(Guid.NewGuid(), "b", 3, ColourSelection.Black);
            var move4 = Move.New();
            move4.Update(Guid.NewGuid(), "c", 2, ColourSelection.Black);

            var moves = new List<Move>
            {
                move1, move2, move3, move4
            };

            var boardPositions = appService.GetNextBoardPositions(moves, BoardSize.NineByNine);
            var result = boardPositions[expectedXPosition, expectedYPosition];

            Assert.AreEqual(PositionState.Suicide, result.PositionState);
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

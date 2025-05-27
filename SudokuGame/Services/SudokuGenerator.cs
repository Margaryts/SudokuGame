using System;
using System.Collections.Generic;
using System.Linq;
using SudokuGame.Models;

namespace SudokuGame.Services
{
    public class SudokuGenerator : ISudokuGenerator
    {
        private Random _random;

        public SudokuGenerator()
        {
            _random = new Random();
        }

        public int[,] GeneratePuzzle(DifficultyLevel difficulty)
        {
            int[,] solution = GenerateCompleteSolution();
            return RemoveCells(solution, (int)difficulty);
        }

        public int[,] GenerateCompleteSolution()
        {
            int[,] grid = new int[9, 9];
            SolveSudoku(grid);
            return grid;
        }

        private bool SolveSudoku(int[,] grid)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (grid[row, col] == 0)
                    {
                        List<int> numbers = Enumerable.Range(1, 9).OrderBy(x => _random.Next()).ToList();

                        foreach (int num in numbers)
                        {
                            if (IsValidPlacement(grid, row, col, num))
                            {
                                grid[row, col] = num;

                                if (SolveSudoku(grid))
                                    return true;

                                grid[row, col] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsValidPlacement(int[,] grid, int row, int col, int num)
        {
            for (int x = 0; x < 9; x++)
            {
                if (grid[row, x] == num)
                    return false;
            }

            for (int x = 0; x < 9; x++)
            {
                if (grid[x, col] == num)
                    return false;
            }

            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (grid[i + startRow, j + startCol] == num)
                        return false;
                }
            }

            return true;
        }

        private int[,] RemoveCells(int[,] solution, int cellsToKeep)
        {
            int[,] puzzle = (int[,])solution.Clone();
            List<(int, int)> positions = new List<(int, int)>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    positions.Add((i, j));
                }
            }

            positions = positions.OrderBy(x => _random.Next()).ToList();
            int cellsToRemove = 81 - cellsToKeep;

            for (int i = 0; i < cellsToRemove && i < positions.Count; i++)
            {
                var (row, col) = positions[i];
                puzzle[row, col] = 0;
            }

            return puzzle;
        }
    }
}
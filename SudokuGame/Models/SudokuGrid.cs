using System;

namespace SudokuGame.Models
{
    public class SudokuGrid
    {
        private int[,] _grid;
        private int[,] _solution;
        private bool[,] _isInitial;

        public int[,] Grid => _grid;
        public bool[,] IsInitial => _isInitial;

        public SudokuGrid()
        {
            _grid = new int[9, 9];
            _solution = new int[9, 9];
            _isInitial = new bool[9, 9];
        }

        public void SetCell(int row, int col, int value)
        {
            if (IsValidPosition(row, col) && !_isInitial[row, col])
            {
                _grid[row, col] = value;
            }
        }

        public int GetCell(int row, int col)
        {
            return IsValidPosition(row, col) ? _grid[row, col] : 0;
        }

        public bool IsInitialCell(int row, int col)
        {
            return IsValidPosition(row, col) && _isInitial[row, col];
        }

        public void SetInitialGrid(int[,] initialGrid)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _grid[i, j] = initialGrid[i, j];
                    _solution[i, j] = initialGrid[i, j];
                    _isInitial[i, j] = initialGrid[i, j] != 0;
                }
            }
        }

        public bool IsValidMove(int row, int col, int value)
        {
            if (!IsValidPosition(row, col) || value < 1 || value > 9)
                return false;

            if (_isInitial[row, col])
                return false;

            // Temporarily place the value
            int originalValue = _grid[row, col];
            _grid[row, col] = value;

            bool isValid = IsValidGrid();

            // Restore original value
            _grid[row, col] = originalValue;

            return isValid;
        }

        public bool IsValidGrid()
        {
            // Check rows
            for (int row = 0; row < 9; row++)
            {
                if (!IsValidUnit(GetRow(row)))
                    return false;
            }

            // Check columns
            for (int col = 0; col < 9; col++)
            {
                if (!IsValidUnit(GetColumn(col)))
                    return false;
            }

            // Check 3x3 boxes
            for (int boxRow = 0; boxRow < 3; boxRow++)
            {
                for (int boxCol = 0; boxCol < 3; boxCol++)
                {
                    if (!IsValidUnit(GetBox(boxRow, boxCol)))
                        return false;
                }
            }

            return true;
        }

        public bool IsComplete()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_grid[i, j] == 0)
                        return false;
                }
            }
            return IsValidGrid();
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < 9 && col >= 0 && col < 9;
        }

        private int[] GetRow(int row)
        {
            int[] result = new int[9];
            for (int col = 0; col < 9; col++)
            {
                result[col] = _grid[row, col];
            }
            return result;
        }

        private int[] GetColumn(int col)
        {
            int[] result = new int[9];
            for (int row = 0; row < 9; row++)
            {
                result[row] = _grid[row, col];
            }
            return result;
        }

        private int[] GetBox(int boxRow, int boxCol)
        {
            int[] result = new int[9];
            int index = 0;

            for (int row = boxRow * 3; row < (boxRow + 1) * 3; row++)
            {
                for (int col = boxCol * 3; col < (boxCol + 1) * 3; col++)
                {
                    result[index++] = _grid[row, col];
                }
            }
            return result;
        }

        private bool IsValidUnit(int[] unit)
        {
            bool[] seen = new bool[10]; // Index 0 unused, 1-9 for numbers

            foreach (int value in unit)
            {
                if (value != 0)
                {
                    if (seen[value])
                        return false;
                    seen[value] = true;
                }
            }
            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (!_isInitial[i, j])
                    {
                        _grid[i, j] = 0;
                    }
                }
            }
        }
    }
}

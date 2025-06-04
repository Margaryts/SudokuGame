using System;

namespace SudokuGame.Models
{
    public class SudokuGrid
    {
        private const int GridSize = 9;
        private const int BoxSize = 3;

        private readonly int[,] _grid = new int[GridSize, GridSize];
        private readonly int[,] _solution = new int[GridSize, GridSize];
        private readonly bool[,] _isInitial = new bool[GridSize, GridSize];

        public int[,] Grid => _grid;
        public bool[,] IsInitial => _isInitial;

        public void SetCell(int row, int col, int value)
        {
            if (IsEditable(row, col))
                _grid[row, col] = value;
        }

        public int GetCell(int row, int col) => IsValidPosition(row, col) ? _grid[row, col] : 0;

        public bool IsInitialCell(int row, int col) => IsValidPosition(row, col) && _isInitial[row, col];

        public void SetInitialGrid(int[,] initialGrid)
        {
            ForEachCell((i, j) =>
            {
                int value = initialGrid[i, j];
                _grid[i, j] = value;
                _solution[i, j] = value;
                _isInitial[i, j] = value != 0;
            });
        }

        public bool IsValidMove(int row, int col, int value)
        {
            if (!IsEditable(row, col) || value < 1 || value > 9)
                return false;

            int original = _grid[row, col];
            _grid[row, col] = value;

            bool valid = IsValidGrid();
            _grid[row, col] = original;
            return valid;
        }

        public bool IsValidGrid()
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (!IsValidUnit(GetRow(i)) || !IsValidUnit(GetColumn(i)))
                    return false;
            }

            for (int row = 0; row < GridSize; row += BoxSize)
            {
                for (int col = 0; col < GridSize; col += BoxSize)
                {
                    if (!IsValidUnit(GetBox(row, col)))
                        return false;
                }
            }

            return true;
        }

        public bool IsComplete()
        {
            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize; j++)
                    if (_grid[i, j] == 0)
                        return false;

            return IsValidGrid();
        }

        public void Clear()
        {
            ForEachCell((i, j) =>
            {
                if (!_isInitial[i, j])
                    _grid[i, j] = 0;
            });
        }

        // 🔧 Допоміжні методи

        private bool IsValidPosition(int row, int col) =>
            row >= 0 && row < GridSize && col >= 0 && col < GridSize;

        private bool IsEditable(int row, int col) =>
            IsValidPosition(row, col) && !_isInitial[row, col];

        private void ForEachCell(Action<int, int> action)
        {
            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize; j++)
                    action(i, j);
        }

        private int[] GetRow(int row)
        {
            var result = new int[GridSize];
            for (int col = 0; col < GridSize; col++)
                result[col] = _grid[row, col];
            return result;
        }

        private int[] GetColumn(int col)
        {
            var result = new int[GridSize];
            for (int row = 0; row < GridSize; row++)
                result[row] = _grid[row, col];
            return result;
        }

        private int[] GetBox(int startRow, int startCol)
        {
            var result = new int[GridSize];
            int index = 0;
            for (int i = 0; i < BoxSize; i++)
                for (int j = 0; j < BoxSize; j++)
                    result[index++] = _grid[startRow + i, startCol + j];
            return result;
        }

        private bool IsValidUnit(int[] unit)
        {
            var seen = new bool[GridSize + 1]; // 0..9
            foreach (int val in unit)
            {
                if (val != 0 && seen[val])
                    return false;
                seen[val] = true;
            }
            return true;
        }
    }
}

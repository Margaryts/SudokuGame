using System;
using System.IO;
using System.Text.Json;
using SudokuGame.Models;

namespace SudokuGame.Services
{
    public class GameManager : IGameManager
    {
        private readonly ISudokuGenerator _generator;
        public GameState CurrentGame { get; private set; }

        public GameManager(ISudokuGenerator generator)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public void StartNewGame(DifficultyLevel difficulty)
        {
            CurrentGame = new GameState(difficulty);
            int[,] puzzle = _generator.GeneratePuzzle(difficulty);
            CurrentGame.Grid.SetInitialGrid(puzzle);
        }

        public bool MakeMove(int row, int col, int value)
        {
            if (CurrentGame?.Grid == null)
                return false;

            if (CurrentGame.Grid.IsValidMove(row, col, value))
            {
                CurrentGame.Grid.SetCell(row, col, value);
                return true;
            }
            else
            {
                CurrentGame.Mistakes++;
                return false;
            }
        }

        public void ClearCell(int row, int col)
        {
            CurrentGame?.Grid?.SetCell(row, col, 0);
        }

        public void ResetGame()
        {
            CurrentGame?.Reset();
        }

        public bool CheckWin()
        {
            if (CurrentGame?.Grid == null)
                return false;

            bool isComplete = CurrentGame.Grid.IsComplete();
            if (isComplete)
            {
                CurrentGame.IsCompleted = true;
            }
            return isComplete;
        }

        public void SaveGame(string filePath)
        {
            if (CurrentGame == null)
                return;

            try
            {
                int[][] gridArray = new int[9][];
                bool[][] isInitialArray = new bool[9][];

                for (int i = 0; i < 9; i++)
                {
                    gridArray[i] = new int[9];
                    isInitialArray[i] = new bool[9];
                    for (int j = 0; j < 9; j++)
                    {
                        gridArray[i][j] = CurrentGame.Grid.Grid[i, j];
                        isInitialArray[i][j] = CurrentGame.Grid.IsInitial[i, j];
                    }
                }

                var saveData = new
                {
                    Grid = gridArray,
                    IsInitial = isInitialArray,
                    Difficulty = (int)CurrentGame.Difficulty,
                    ElapsedTime = CurrentGame.ElapsedTime.TotalSeconds,
                    Mistakes = CurrentGame.Mistakes,
                    StartTime = CurrentGame.StartTime.ToBinary()
                };

                string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save game: {ex.Message}", ex);
            }
        }

        public bool LoadGame(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                string json = File.ReadAllText(filePath);
                var saveData = JsonSerializer.Deserialize<JsonElement>(json);

                var difficulty = (DifficultyLevel)saveData.GetProperty("Difficulty").GetInt32();
                CurrentGame = new GameState(difficulty);

                var gridProperty = saveData.GetProperty("Grid");
                var isInitialProperty = saveData.GetProperty("IsInitial");
                int[,] grid = new int[9, 9];
                bool[,] isInitial = new bool[9, 9];

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        grid[i, j] = gridProperty[i][j].GetInt32();
                        isInitial[i, j] = isInitialProperty[i][j].GetBoolean();
                    }
                }

                CurrentGame.Grid.SetInitialGrid(grid);

                // Manually set the IsInitial array
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (!isInitial[i, j] && grid[i, j] != 0)
                        {
                            // This is a user-entered value, not initial
                            CurrentGame.Grid.SetCell(i, j, grid[i, j]);
                        }
                    }
                }

                // Load other properties
                CurrentGame.ElapsedTime = TimeSpan.FromSeconds(saveData.GetProperty("ElapsedTime").GetDouble());
                CurrentGame.Mistakes = saveData.GetProperty("Mistakes").GetInt32();
                CurrentGame.StartTime = DateTime.FromBinary(saveData.GetProperty("StartTime").GetInt64());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
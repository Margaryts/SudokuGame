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
                SaveDataDTO saveData = CreateSaveDataDTO();

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
                SaveDataDTO saveData = DeserializeSaveData(filePath);
                InitializeGameFromSaveData(saveData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private SaveDataDTO CreateSaveDataDTO()
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

            return new SaveDataDTO
            {
                Grid = gridArray,
                IsInitial = isInitialArray,
                Difficulty = (int)CurrentGame.Difficulty,
                ElapsedTime = CurrentGame.ElapsedTime.TotalSeconds,
                Mistakes = CurrentGame.Mistakes,
                StartTime = CurrentGame.StartTime.ToBinary()
            };
        }

        private SaveDataDTO DeserializeSaveData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Save file not found at {filePath}");

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SaveDataDTO>(json);
        }

        private void InitializeGameFromSaveData(SaveDataDTO saveData)
        {
            var difficulty = (DifficultyLevel)saveData.Difficulty;
            CurrentGame = new GameState(difficulty);

            int[,] grid = new int[9, 9];
            bool[,] isInitial = new bool[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    grid[i, j] = saveData.Grid[i][j];
                    isInitial[i, j] = saveData.IsInitial[i][j];
                }
            }

            CurrentGame.Grid.SetInitialGrid(grid);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (!isInitial[i, j] && grid[i, j] != 0)
                    {
                        CurrentGame.Grid.SetCell(i, j, grid[i, j]);
                    }
                }
            }

            CurrentGame.ElapsedTime = TimeSpan.FromSeconds(saveData.ElapsedTime);
            CurrentGame.Mistakes = saveData.Mistakes;
            CurrentGame.StartTime = DateTime.FromBinary(saveData.StartTime);
        }
    }
}

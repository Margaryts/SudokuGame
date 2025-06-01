using System;
using System.IO;
using System.Text.Json;
using SudokuGame.Models;

namespace SudokuGame.Services
{
    public class GameManager : IGameManager
    {
        private readonly ISudokuGenerator _generator;
        private readonly IGamePersistenceService _persistenceService;

        public GameState CurrentGame { get; private set; }

        public GameManager(ISudokuGenerator generator, IGamePersistenceService persistenceService)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
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

            CurrentGame.Mistakes++;
            return false;
        }

        public void ClearCell(int row, int col) =>
            CurrentGame?.Grid?.SetCell(row, col, 0);

        public void ResetGame() =>
            CurrentGame?.Reset();

        public bool CheckWin()
        {
            if (CurrentGame?.Grid == null)
                return false;

            if (CurrentGame.Grid.IsComplete())
            {
                CurrentGame.IsCompleted = true;
                return true;
            }

            return false;
        }

        public void SaveGame(string filePath) =>
            _persistenceService.SaveGame(CurrentGame, filePath);

        public bool LoadGame(string filePath)
        {
            try
            {
                CurrentGame = _persistenceService.LoadGame(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

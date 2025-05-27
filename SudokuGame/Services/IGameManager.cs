using SudokuGame.Models;

namespace SudokuGame.Services
{
    public interface IGameManager
    {
        GameState CurrentGame { get; }
        void StartNewGame(DifficultyLevel difficulty);
        bool MakeMove(int row, int col, int value);
        void ClearCell(int row, int col);
        void ResetGame();
        bool CheckWin();
        void SaveGame(string filePath);
        bool LoadGame(string filePath);
    }
}
using SudokuGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGame.Services
{
    public interface IGamePersistenceService
    {
        void SaveGame(GameState game, string filePath);
        GameState LoadGame(string filePath);
    }
}

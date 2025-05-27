using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGame.Services
{
    public interface ISudokuGenerator
    {
        int[,] GeneratePuzzle(Models.DifficultyLevel difficulty);
        int[,] GenerateCompleteSolution();
    }
}
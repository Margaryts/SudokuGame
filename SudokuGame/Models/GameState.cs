using System;

namespace SudokuGame.Models
{
    public enum DifficultyLevel
    {
        Easy = 45,
        Medium = 35,
        Hard = 25,
        Expert = 17
    }

    public class GameState
    {
        public SudokuGrid Grid { get; private set; }
        public DifficultyLevel Difficulty { get; private set; }
        public TimeSpan ElapsedTime { get; set; }
        public int Mistakes { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartTime { get; set; }

        public GameState(DifficultyLevel difficulty)
        {
            Grid = new SudokuGrid();
            Difficulty = difficulty;
            ElapsedTime = TimeSpan.Zero;
            Mistakes = 0;
            IsCompleted = false;
            StartTime = DateTime.Now;
        }

        public void Reset()
        {
            Grid.Clear();
            ElapsedTime = TimeSpan.Zero;
            Mistakes = 0;
            IsCompleted = false;
            StartTime = DateTime.Now;
        }
    }
}

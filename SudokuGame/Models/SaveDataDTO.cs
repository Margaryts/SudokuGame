using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGame.Models
{
    public class SaveDataDTO
    {
        public int[][] Grid { get; set; }
        public bool[][] IsInitial { get; set; }
        public int Difficulty { get; set; }
        public double ElapsedTime { get; set; }
        public int Mistakes { get; set; }
        public long StartTime { get; set; }
    }
}

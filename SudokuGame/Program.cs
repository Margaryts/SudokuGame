using SudokuGame.View;
using System;
using System.Windows.Forms;

namespace SudokuGame
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(
                new MainForm {
                    Size = new Size(600, 550)
                });
        }
    }
}
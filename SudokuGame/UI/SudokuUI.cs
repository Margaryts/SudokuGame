using System;
using System.Drawing;
using System.Windows.Forms;
using SudokuGame.Models;
using SudokuGame.UI;

namespace SudokuGame.UI
{
    public class SudokuUI
    {
        public Panel MainPanel { get; private set; }
        public SudokuCell[,] Cells { get; private set; }
        public Label TimeLabel { get; private set; }
        public Label MistakesLabel { get; private set; }
        public event EventHandler<CellValueChangedEventArgs> CellValueChanged;
        public event EventHandler<EventArgs> NewGameClicked;
        public event EventHandler<DifficultyLevel> DifficultyClicked;
        public event EventHandler<EventArgs> ResetClicked;
        public event EventHandler<EventArgs> SaveClicked;
        public event EventHandler<EventArgs> LoadClicked;
        public event EventHandler<EventArgs> CheckClicked;

        public SudokuUI()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            MainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            AddTitleLabel(MainPanel);
            AddInfoPanel(MainPanel);
            CreateGameGrid(MainPanel);
            CreateControlButtons(MainPanel);
        }

        private void AddTitleLabel(Panel mainPanel)
        {
            Label titleLabel = new Label
            {
                Text = "SUDOKU",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(250, 10)
            };
            mainPanel.Controls.Add(titleLabel);
        }

        private void AddInfoPanel(Panel mainPanel)
        {
            Panel infoPanel = new Panel
            {
                Location = new Point(20, 50),
                Size = new Size(540, 40),
                BorderStyle = BorderStyle.FixedSingle
            };
            mainPanel.Controls.Add(infoPanel);

            TimeLabel = new Label
            {
                Text = "Time: 00:00",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            infoPanel.Controls.Add(TimeLabel);

            MistakesLabel = new Label
            {
                Text = "Mistakes: 0",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(400, 10),
                AutoSize = true
            };
            infoPanel.Controls.Add(MistakesLabel);
        }

        private void CreateGameGrid(Panel parent)
        {
            Panel gridPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(370, 370),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };
            parent.Controls.Add(gridPanel);

            Cells = new SudokuCell[9, 9];

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    SudokuCell cell = new SudokuCell(row, col)
                    {
                        Location = new Point(
                            col * 40 + (col / 3) * 2 + 2,
                            row * 40 + (row / 3) * 2 + 2
                        )
                    };
                    cell.CellValueChanged += (s, e) => CellValueChanged?.Invoke(s, e);
                    Cells[row, col] = cell;
                    gridPanel.Controls.Add(cell);
                }
            }
        }

        private void CreateControlButtons(Panel parent)
        {
            Panel buttonPanel = new Panel
            {
                Location = new Point(400, 100),
                Size = new Size(160, 370)
            };
            parent.Controls.Add(buttonPanel);

            AddButton(buttonPanel, "New Game", new Point(10, 10), (s, e) => NewGameClicked?.Invoke(s, EventArgs.Empty), Color.LightBlue);

            string[] difficulties = { "Easy", "Medium", "Hard", "Expert" };
            DifficultyLevel[] levels = { DifficultyLevel.Easy, DifficultyLevel.Medium, DifficultyLevel.Hard, DifficultyLevel.Expert };

            for (int i = 0; i < difficulties.Length; i++)
            {
                var level = levels[i];
                AddButton(buttonPanel, difficulties[i], new Point(10, 60 + i * 35), (s, e) => DifficultyClicked?.Invoke(s, level), Color.LightGreen);
            }

            AddButton(buttonPanel, "Reset", new Point(10, 210), (s, e) => ResetClicked?.Invoke(s, EventArgs.Empty), Color.LightCoral);
            AddButton(buttonPanel, "Save Game", new Point(10, 250), (s, e) => SaveClicked?.Invoke(s, EventArgs.Empty), Color.LightYellow);
            AddButton(buttonPanel, "Load Game", new Point(10, 290), (s, e) => LoadClicked?.Invoke(s, EventArgs.Empty), Color.LightYellow);
            AddButton(buttonPanel, "Check Solution", new Point(10, 330), (s, e) => CheckClicked?.Invoke(s, EventArgs.Empty), Color.Lavender);
        }

        private void AddButton(Control parent, string text, Point location, EventHandler clickHandler, Color color)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(140, 30),
                Location = location,
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = color
            };
            button.Click += clickHandler;
            parent.Controls.Add(button);
        }
    }
} 
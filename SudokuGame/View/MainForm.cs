using System;
using System.Drawing;
using System.Windows.Forms;
using SudokuGame.Models;
using SudokuGame.Services;
using SudokuGame.UI;
using Timer = System.Windows.Forms.Timer;

namespace SudokuGame
{
    public partial class MainForm : Form
    {
        private readonly IGameManager _gameManager;
        private readonly IGamePersistenceService _persistenceService;
        private SudokuCell[,] _cells;
        private Label _timeLabel;
        private Label _mistakesLabel;
        private Timer _gameTimer;

        public MainForm()
        {
            var generator = new SudokuGenerator();
            var persistence = new JsonGamePersistenceService();
            _gameManager = new GameManager(generator, persistence);
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            InitializeFormSettings();

            Panel mainPanel = CreateMainPanel();
            Controls.Add(mainPanel);

            AddTitleLabel(mainPanel);
            AddInfoPanel(mainPanel);
            CreateGameGrid(mainPanel);
            CreateControlButtons(mainPanel);
            InitializeGameTimer();
        }

        private void InitializeFormSettings()
        {
            Text = "Sudoku Game";
            Size = new Size(600, 700);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private Panel CreateMainPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
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

            _timeLabel = new Label
            {
                Text = "Time: 00:00",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            infoPanel.Controls.Add(_timeLabel);

            _mistakesLabel = new Label
            {
                Text = "Mistakes: 0",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(400, 10),
                AutoSize = true
            };
            infoPanel.Controls.Add(_mistakesLabel);
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

            _cells = new SudokuCell[9, 9];

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
                    cell.CellValueChanged += OnCellValueChanged;

                    _cells[row, col] = cell;
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

            AddButton(buttonPanel, "New Game", new Point(10, 10), OnNewGameClick, Color.LightBlue);

            string[] difficulties = { "Easy", "Medium", "Hard", "Expert" };
            DifficultyLevel[] levels = { DifficultyLevel.Easy, DifficultyLevel.Medium, DifficultyLevel.Hard, DifficultyLevel.Expert };

            for (int i = 0; i < difficulties.Length; i++)
            {
                AddButton(buttonPanel, difficulties[i], new Point(10, 60 + i * 35), OnDifficultyClick, Color.LightGreen, levels[i]);
            }

            AddButton(buttonPanel, "Reset", new Point(10, 210), OnResetClick, Color.LightCoral);
            AddButton(buttonPanel, "Save Game", new Point(10, 250), OnSaveClick, Color.LightYellow);
            AddButton(buttonPanel, "Load Game", new Point(10, 290), OnLoadClick, Color.LightYellow);
            AddButton(buttonPanel, "Check Solution", new Point(10, 330), OnCheckClick, Color.Lavender);
        }

        private void AddButton(Control parent, string text, Point location, EventHandler clickHandler, Color color, object tag = null)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(140, 30),
                Location = location,
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = color,
                Tag = tag
            };
            button.Click += clickHandler;
            parent.Controls.Add(button);
        }

        private void InitializeGameTimer()
        {
            _gameTimer = new Timer { Interval = 1000 };
            _gameTimer.Tick += OnTimerTick;
        }

        private void InitializeGame()
        {
            _gameManager.StartNewGame(DifficultyLevel.Medium);
            UpdateUI();
            _gameTimer.Start();
        }

        private void UpdateUI()
        {
            if (_gameManager.CurrentGame?.Grid == null) return;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int value = _gameManager.CurrentGame.Grid.GetCell(row, col);
                    bool isInitial = _gameManager.CurrentGame.Grid.IsInitialCell(row, col);
                    _cells[row, col].SetValue(value, isInitial);
                }
            }

            _mistakesLabel.Text = $"Mistakes: {_gameManager.CurrentGame.Mistakes}";
        }

        private void OnCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (_gameManager.CurrentGame == null) return;

            _cells[e.Row, e.Column].ClearHighlight();

            if (e.Value == 0)
            {
                _gameManager.ClearCell(e.Row, e.Column);
                return;
            }

            bool isValidMove = _gameManager.MakeMove(e.Row, e.Column, e.Value);

            if (!isValidMove)
            {
                _cells[e.Row, e.Column].HighlightError();
            }

            _mistakesLabel.Text = $"Mistakes: {_gameManager.CurrentGame.Mistakes}";

            if (_gameManager.CheckWin())
            {
                _gameTimer.Stop();
                MessageBox.Show($"Congratulations! You solved the puzzle!\nTime: {_gameManager.CurrentGame.ElapsedTime:mm\\:ss}\nMistakes: {_gameManager.CurrentGame.Mistakes}",
                               "Victory!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnNewGameClick(object sender, EventArgs e) => InitializeGame();

        private void OnDifficultyClick(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is DifficultyLevel difficulty)
            {
                _gameManager.StartNewGame(difficulty);
                UpdateUI();
                _gameTimer.Start();
            }
        }

        private void OnResetClick(object sender, EventArgs e)
        {
            _gameManager.ResetGame();
            UpdateUI();
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            using SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Sudoku files (*.sud)|*.sud|All files (*.*)|*.*",
                DefaultExt = "sud"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _gameManager.SaveGame(dialog.FileName);
                    MessageBox.Show("Game saved successfully!", "Save Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnLoadClick(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Sudoku files (*.sud)|*.sud|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (_gameManager.LoadGame(dialog.FileName))
                {
                    UpdateUI();
                    _gameTimer.Start();
                    MessageBox.Show("Game loaded successfully!", "Load Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error loading game file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnCheckClick(object sender, EventArgs e)
        {
            if (_gameManager.CheckWin())
            {
                MessageBox.Show("Congratulations! The solution is correct!", "Solution Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("The solution is not complete or contains errors.", "Solution Check", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_gameManager.CurrentGame != null && !_gameManager.CurrentGame.IsCompleted)
            {
                _gameManager.CurrentGame.ElapsedTime = DateTime.Now - _gameManager.CurrentGame.StartTime;
                _timeLabel.Text = $"Time: {_gameManager.CurrentGame.ElapsedTime:mm\\:ss}";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gameTimer?.Stop();
            _gameTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
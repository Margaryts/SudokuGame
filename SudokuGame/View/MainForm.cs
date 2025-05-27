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
        private SudokuCell[,] _cells;
        private Label _timeLabel;
        private Label _mistakesLabel;
        private Timer _gameTimer;

        public MainForm()
        {
            _gameManager = new GameManager(new SudokuGenerator());
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            Text = "Sudoku Game";
            Size = new Size(600, 700);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Create main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            Controls.Add(mainPanel);

            // Create title
            Label titleLabel = new Label
            {
                Text = "SUDOKU",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(250, 10)
            };
            mainPanel.Controls.Add(titleLabel);

            // Create info panel
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

            // Create game grid
            CreateGameGrid(mainPanel);

            // Create control buttons
            CreateControlButtons(mainPanel);

            // Initialize timer
            _gameTimer = new Timer();
            _gameTimer.Interval = 1000; // 1 second
            _gameTimer.Tick += OnTimerTick;
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
                    SudokuCell cell = new SudokuCell(row, col);
                    cell.Location = new Point(
                        col * 40 + (col / 3) * 2 + 2,
                        row * 40 + (row / 3) * 2 + 2
                    );
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

            Button newGameButton = new Button
            {
                Text = "New Game",
                Size = new Size(140, 40),
                Location = new Point(10, 10),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightBlue
            };
            newGameButton.Click += OnNewGameClick;
            buttonPanel.Controls.Add(newGameButton);

            // Difficulty buttons
            string[] difficulties = { "Easy", "Medium", "Hard", "Expert" };
            DifficultyLevel[] levels = { DifficultyLevel.Easy, DifficultyLevel.Medium, DifficultyLevel.Hard, DifficultyLevel.Expert };

            for (int i = 0; i < difficulties.Length; i++)
            {
                Button diffButton = new Button
                {
                    Text = difficulties[i],
                    Size = new Size(140, 30),
                    Location = new Point(10, 60 + i * 35),
                    Tag = levels[i],
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    BackColor = Color.LightGreen
                };
                diffButton.Click += OnDifficultyClick;
                buttonPanel.Controls.Add(diffButton);
            }

            Button resetButton = new Button
            {
                Text = "Reset",
                Size = new Size(140, 30),
                Location = new Point(10, 210),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = Color.LightCoral
            };
            resetButton.Click += OnResetClick;
            buttonPanel.Controls.Add(resetButton);

            Button saveButton = new Button
            {
                Text = "Save Game",
                Size = new Size(140, 30),
                Location = new Point(10, 250),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = Color.LightYellow
            };
            saveButton.Click += OnSaveClick;
            buttonPanel.Controls.Add(saveButton);

            Button loadButton = new Button
            {
                Text = "Load Game",
                Size = new Size(140, 30),
                Location = new Point(10, 290),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = Color.LightYellow
            };
            loadButton.Click += OnLoadClick;
            buttonPanel.Controls.Add(loadButton);

            Button checkButton = new Button
            {
                Text = "Check Solution",
                Size = new Size(140, 30),
                Location = new Point(10, 330),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = Color.Lavender
            };
            checkButton.Click += OnCheckClick;
            buttonPanel.Controls.Add(checkButton);
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

        private void OnNewGameClick(object sender, EventArgs e)
        {
            _gameManager.StartNewGame(DifficultyLevel.Medium);
            UpdateUI();
            _gameTimer.Start();
        }

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
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Sudoku files (*.sud)|*.sud|All files (*.*)|*.*";
                dialog.DefaultExt = "sud";

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
        }

        private void OnLoadClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Sudoku files (*.sud)|*.sud|All files (*.*)|*.*";

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
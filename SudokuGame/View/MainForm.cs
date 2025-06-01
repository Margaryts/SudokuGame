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
        private Timer _gameTimer;
        private readonly SudokuUI _ui;

        public MainForm()
        {
            var generator = new SudokuGenerator();
            var persistence = new JsonGamePersistenceService();
            _gameManager = new GameManager(generator, persistence);
            _ui = new SudokuUI();
            Controls.Add(_ui.MainPanel);
            SubscribeOnUIEvents();
            InitializeGameTimer();
            InitializeGame();
        }

        private void SubscribeOnUIEvents()
        {
            _ui.CellValueChanged += OnCellValueChanged;
            _ui.NewGameClicked += (s, e) => InitializeGame();
            _ui.DifficultyClicked += (s, level) =>
            {
                _gameManager.StartNewGame(level);
                UpdateUI();
                _gameTimer.Start();
            };
            _ui.ResetClicked += (s, e) => { _gameManager.ResetGame(); UpdateUI(); };
            _ui.SaveClicked += OnSaveClick;
            _ui.LoadClicked += OnLoadClick;
            _ui.CheckClicked += OnCheckClick;
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
                    _ui.Cells[row, col].SetValue(value, isInitial);
                }
            }
            _ui.MistakesLabel.Text = $"Mistakes: {_gameManager.CurrentGame.Mistakes}";
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_gameManager.CurrentGame != null && !_gameManager.CurrentGame.IsCompleted)
            {
                _gameManager.CurrentGame.ElapsedTime = DateTime.Now - _gameManager.CurrentGame.StartTime;
                _ui.TimeLabel.Text = $"Time: {_gameManager.CurrentGame.ElapsedTime:mm\\:ss}";
            }
        }
        private void OnCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (_gameManager.CurrentGame == null) return;
            _ui.Cells[e.Row, e.Column].ClearHighlight();
            if (e.Value == 0)
            {
                _gameManager.ClearCell(e.Row, e.Column);
                return;
            }
            bool isValidMove = _gameManager.MakeMove(e.Row, e.Column, e.Value);
            if (!isValidMove)
            {
                _ui.Cells[e.Row, e.Column].HighlightError();
            }
            _ui.MistakesLabel.Text = $"Mistakes: {_gameManager.CurrentGame.Mistakes}";
            if (_gameManager.CheckWin())
            {
                _gameTimer.Stop();
                MessageBox.Show($"Congratulations! You solved the puzzle!\nTime: {_gameManager.CurrentGame.ElapsedTime:mm\\:ss}\nMistakes: {_gameManager.CurrentGame.Mistakes}",
                               "Victory!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gameTimer?.Stop();
            _gameTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
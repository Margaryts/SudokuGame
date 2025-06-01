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
            InitializeGameTimer();
            InitializeGame();
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _gameTimer?.Stop();
            _gameTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
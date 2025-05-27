using System;
using System.Drawing;
using System.Windows.Forms;

namespace SudokuGame.UI
{
    public class SudokuCell : TextBox
    {
        public int Row { get; }
        public int Column { get; }
        private bool _isInitial;

        public event EventHandler<CellValueChangedEventArgs> CellValueChanged;

        public SudokuCell(int row, int column) : base()
        {
            Row = row;
            Column = column;
            InitializeCell();
        }

        private void InitializeCell()
        {
            Size = new Size(40, 40);
            TextAlign = HorizontalAlignment.Center;
            Font = new Font("Arial", 12, FontStyle.Bold);
            MaxLength = 1;

            TextChanged += OnTextChanged;
            KeyPress += OnKeyPress;
        }

        public void SetValue(int value, bool isInitial = false)
        {
            _isInitial = isInitial;
            Text = value == 0 ? "" : value.ToString();

            if (isInitial)
            {
                BackColor = Color.LightGray;
                ForeColor = Color.Black;
                ReadOnly = true;
            }
            else
            {
                BackColor = Color.White;
                ForeColor = Color.Blue;
                ReadOnly = false;
            }
        }

        public int GetValue()
        {
            return int.TryParse(Text, out int value) ? value : 0;
        }

        public bool IsInitialCell => _isInitial;

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!_isInitial)
            {
                int value = GetValue();
                CellValueChanged?.Invoke(this, new CellValueChangedEventArgs(Row, Column, value));
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (char.IsDigit(e.KeyChar) && e.KeyChar == '0')
            {
                e.Handled = true;
            }
        }

        public void HighlightError()
        {
            if (!_isInitial)
            {
                BackColor = Color.LightPink;
            }
        }

        public void ClearHighlight()
        {
            if (!_isInitial)
            {
                BackColor = Color.White;
            }
        }
    }

    public class CellValueChangedEventArgs : EventArgs
    {
        public int Row { get; }
        public int Column { get; }
        public int Value { get; }

        public CellValueChangedEventArgs(int row, int column, int value)
        {
            Row = row;
            Column = column;
            Value = value;
        }
    }
}
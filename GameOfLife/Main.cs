using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GameOfLife
{
    public partial class Main : Form
    {
        private Graphics _graphics;
        private bool _isRunning;

        private bool[,] _field;
        private int _rows;
        private int _cols;
        private int _resolution;
        private int _generationNumber = 0;

        public Main()
        {
            InitializeComponent();
            _isRunning = false;
        }

        private void timer_Tick(object sender, EventArgs e) => DrawNextGeneration();

        private void bRun_Click(object sender, EventArgs e)
        {
            if (_isRunning)
                Stop();
            else
                Run();
        }

        private void Run() 
        {
            if (timer.Enabled)
                return;

            _generationNumber = 0;

            _isRunning = true;
            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            bRun.Text = "Стоп";

            _resolution = (int)nudResolution.Value;
        
            _rows = pictureBox.Height / _resolution;
            _cols = pictureBox.Width / _resolution;
            _field = new bool[_cols, _rows];

            Random random = new Random();

            for (int x = 0; x < _cols; x++)
                for (int y = 0; y < _rows; y++)
                    _field[x, y] = random.Next((int)nudDensity.Value) == 0;

            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            _graphics = Graphics.FromImage(pictureBox.Image);

            timer.Start();
        }

        private void Stop() 
        {
            if (!timer.Enabled)
                return;

            timer.Stop();

            _isRunning = false;
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
            bRun.Text = "Старт"; 
        }

        private void DrawNextGeneration() 
        {
            _graphics.Clear(Color.Black);

            var newField = new bool[_cols, _rows];

            // - 1 в последних 2 аргументах для рамочек
            for (int x = 0; x < _cols; x++) 
            {
                for (int y = 0; y < _rows; y++)
                {
                    var neighboursCount = GetNeighboursCount(x, y);
                    var isHasLife = _field[x, y];

                    if (!isHasLife && neighboursCount == 3)
                        newField[x, y] = true;
                    else if (isHasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;
                    else
                        newField[x, y] = _field[x, y];

                    if (isHasLife)
                        _graphics.FillRectangle(Brushes.Crimson, x * _resolution, y * _resolution, _resolution - 1, _resolution - 1);
                }
            }   

            _field = newField;
            pictureBox.Refresh();
            Text = $"Генерация: {++_generationNumber}";
        }

        private int GetNeighboursCount(int x, int y) 
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            { 
                for (int j = -1; j < 2; j++) 
                {
                    // здесь предусмотрено, что может быть выход за пределы массива
                    int col = (x + i + _cols) % _cols;
                    int row = (y + j + _rows) % _rows;

                    bool isSelfChecking = col == x && row == y;
                    var isHasLife = _field[col, row];

                    if (isHasLife && !isSelfChecking)
                        count++;
                }
            }

            return count;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer.Enabled)
                return;

            if (e.Button == MouseButtons.Left) 
            { 
                var x = e.Location.X / _resolution;
                var y = e.Location.Y / _resolution;

                if (IsValidMousePosition(x, y))
                    _field[x, y] = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / _resolution;
                var y = e.Location.Y / _resolution;

                if (IsValidMousePosition(x, y))
                    _field[x, y] = false;
            }
        }

        private bool IsValidMousePosition(int x, int y) => x >= 0 && y >= 0 && x < _cols && y < _rows;
    }
}

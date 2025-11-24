using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Main : Form
    {
        private Graphics _graphics;
        private Engine _engine;
        private int _resolution;
        private bool _isRunning;

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

            _isRunning = true;
            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            _resolution = (int)nudResolution.Value;
            bRun.Text = "Стоп";
            _engine = new Engine
            (
                rows: pictureBox.Height / _resolution,
                cols: pictureBox.Width / _resolution,
                density: (int)(nudDensity.Minimum + nudDensity.Maximum - nudDensity.Value)
            );
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

            var field = _engine.CurrentGeneration;

            // - 1 в последних 2 аргументах FillRectangle() для рамочек
            for (int x = 0; x < field.GetLength(0); x++)
                for (int y = 0; y < field.GetLength(1); y++)
                    if (field[x, y])  
                        _graphics.FillRectangle(Brushes.Crimson, x * _resolution, y * _resolution, _resolution - 1, _resolution - 1);

            Text = $"Генерация: {_engine.GenerationNumber}";

            pictureBox.Refresh();
            _engine.StartNextGeneration();
        }

        

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer.Enabled)
                return;

            if (e.Button == MouseButtons.Left) 
            { 
                var x = e.Location.X / _resolution;
                var y = e.Location.Y / _resolution;
                _engine.AddCell(x, y);
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / _resolution;
                var y = e.Location.Y / _resolution;
                _engine.DelCell(x, y);
            }
        }
    }
}

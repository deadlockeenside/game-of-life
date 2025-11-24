using System;

namespace ConsoleLife
{
    public class Engine
    {
        private readonly int _rows;
        private readonly int _cols;

        private bool[,] _field;

        public Engine(int rows, int cols, int density)
        {
            _rows = rows;
            _cols = cols;
            _field = new bool[_cols, _rows];

            Random random = new Random();

            for (int x = 0; x < _cols; x++)
                for (int y = 0; y < _rows; y++)
                    _field[x, y] = random.Next(density) == 0;
        }

        public uint GenerationNumber { get; private set; }

        public bool[,] CurrentGeneration
        {
            get
            {
                var generation = new bool[_cols, _rows];

                for (int x = 0; x < _cols; x++)
                    for (int y = 0; y < _rows; y++)
                        generation[x, y] = _field[x, y];

                return generation;
            }
        }

        public void StartNextGeneration()
        {
            var newField = new bool[_cols, _rows];

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
                }
            }

            _field = newField;
            GenerationNumber++;
        }

        public void AddCell(int x, int y) => UpdateCell(x, y, state: true);
        public void DelCell(int x, int y) => UpdateCell(x, y, state: false);

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

        private void UpdateCell(int x, int y, bool state)
        {
            if (IsValidCellPosition(x, y))
                _field[x, y] = state;
        }

        private bool IsValidCellPosition(int x, int y) => x >= 0 && y >= 0 && x < _cols && y < _rows;
    }
}

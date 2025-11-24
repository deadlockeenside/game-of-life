using System;
using System.Runtime.InteropServices;

namespace ConsoleLife
{
    internal class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWmd, int nCmdShow);

        private const int MAXIMAZE = 3;

        static void Main(string[] args)
        {
            Console.ReadLine();
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(GetConsoleWindow(), MAXIMAZE);
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            var engine = new Engine
            (
                rows: Console.WindowHeight - 1,
                cols: Console.WindowWidth - 1,
                density: 2
            );

            while (true) 
            { 
                Console.Title = engine.GenerationNumber.ToString();

                var field = engine.CurrentGeneration;

                for (int y = 0; y < field.GetLength(1); y++) 
                {
                    var fullRow = new char[field.GetLength(0)];

                    for (int x = 0; x < field.GetLength(0); x++) 
                    {
                        if (field[x, y])
                            fullRow[x] = '#';
                        else
                            fullRow[x] = ' ';
                    }

                    Console.WriteLine(fullRow);
                }

                Console.SetCursorPosition(0, 0);
                engine.StartNextGeneration();
            }
        }
    }
}

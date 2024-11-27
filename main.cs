using System;
using System.Threading;

class Program
{
    static int gridHeight = 20;
    static int gridWidth = 10;
    static (string symbol, ConsoleColor color)?[,] grid = new (string, ConsoleColor)?[gridHeight, gridWidth];

    static int blockX = 0, blockY = gridWidth / 2;
    static Random random = new Random();
    static int score = 0;
    static bool gameOver = false;

    static readonly (int x, int y)[][] tetrominoShapes = new (int x, int y)[][]
    {
        new (int, int)[] { (0, 0), (1, 0), (2, 0), (3, 0) }, // I-Shape
        new (int, int)[] { (0, 0), (0, 1), (1, 0), (1, 1) }, // O-Shape
        new (int, int)[] { (0, 1), (1, 0), (1, 1), (1, 2) }, // T-Shape
        new (int, int)[] { (0, 0), (1, 0), (2, 0), (2, 1) }, // L-Shape
        new (int, int)[] { (0, 0), (0, 1), (1, 1), (1, 2) }  // Z-Shape
    };

    static ConsoleColor[] colors = new ConsoleColor[]
    {
        ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue,
        ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Magenta
    };

    static (int x, int y)[] currentTetromino;
    static int currentTetrominoType = 0;
    static ConsoleColor currentTetrominoColor;

    static void Main()
    {
        InitializeGrid();
        SpawnTetromino();

        while (!gameOver)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                HandleKeyPress(key.Key);
            }

            DisplayGrid();
            MoveTetrominoDown();

            Thread.Sleep(500);
        }

        Console.Clear();
        Console.WriteLine("Game Over!");
        Console.WriteLine("Final Score: " + score);
    }

    static void InitializeGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                grid[y, x] = null;
            }
        }
    }

    static void DisplayGrid()
    {
        Console.Clear();

        Console.WriteLine("Score: " + score);

        Console.Write("╔");
        for (int i = 0; i < gridWidth; i++) Console.Write("══");
        Console.WriteLine("╗");

        for (int y = 0; y < gridHeight; y++)
        {
            Console.Write("║");

            for (int x = 0; x < gridWidth; x++)
            {
                bool isTetrominoPart = false;

                foreach (var part in currentTetromino)
                {
                    if (y == blockX + part.x && x == blockY + part.y)
                    {
                        Console.ForegroundColor = currentTetrominoColor;
                        Console.Write("[]");
                        Console.ResetColor();
                        isTetrominoPart = true;
                        break;
                    }
                }

                if (!isTetrominoPart)
                {
                    if (grid[y, x].HasValue)
                    {
                        Console.ForegroundColor = grid[y, x]?.color ?? ConsoleColor.White;
                        Console.Write(grid[y, x]?.symbol ?? " ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(". ");
                    }
                }
            }

            Console.WriteLine("║");
        }

        Console.Write("╚");
        for (int i = 0; i < gridWidth; i++) Console.Write("══");
        Console.WriteLine("╝");
    }

    static void SpawnTetromino()
    {
        currentTetrominoType = random.Next(0, tetrominoShapes.Length);
        currentTetromino = tetrominoShapes[currentTetrominoType];
        currentTetrominoColor = colors[random.Next(colors.Length)];

        blockX = 0;
        blockY = random.Next(0, gridWidth - GetTetrominoWidth(currentTetromino));

        if (!CanMoveTetromino(0, 0))
        {
            gameOver = true;
        }
    }

    static int GetTetrominoWidth((int x, int y)[] tetromino)
    {
        int minY = int.MaxValue, maxY = int.MinValue;
        foreach (var part in tetromino)
        {
            if (part.y < minY) minY = part.y;
            if (part.y > maxY) maxY = part.y;
        }
        return maxY - minY + 1;
    }

    static void MoveTetrominoDown()
    {
        if (CanMoveTetromino(1, 0))
        {
            blockX++;
        }
        else
        {
            PlaceTetromino();
            ClearFullLines();
            SpawnTetromino();
        }
    }

    static void HandleKeyPress(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                if (CanMoveTetromino(0, -1)) blockY--;
                break;
            case ConsoleKey.RightArrow:
                if (CanMoveTetromino(0, 1)) blockY++;
                break;
            case ConsoleKey.DownArrow:
                if (CanMoveTetromino(1, 0)) blockX++;
                break;
        }
    }

    static bool CanMoveTetromino(int deltaX, int deltaY)
    {
        foreach (var part in currentTetromino)
        {
            int newX = blockX + part.x + deltaX;
            int newY = blockY + part.y + deltaY;

            if (newX >= gridHeight || newY < 0 || newY >= gridWidth || (grid[newX, newY] != null))
            {
                return false;
            }
        }
        return true;
    }

    static void PlaceTetromino()
    {
        foreach (var part in currentTetromino)
        {
            int gridX = blockX + part.x;
            int gridY = blockY + part.y;
            grid[gridX, gridY] = ("[]", currentTetrominoColor);
        }
    }

    static void ClearFullLines()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            bool isFullLine = true;
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[y, x] == null)
                {
                    isFullLine = false;
                    break;
                }
            }
            if (isFullLine)
            {
                for (int i = y; i > 0; i--)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        grid[i, x] = grid[i - 1, x];
                    }
                }
                for (int x = 0; x < gridWidth; x++)
                {
                    grid[0, x] = null;
                }
                score += 100;
            }
        }
    }
}

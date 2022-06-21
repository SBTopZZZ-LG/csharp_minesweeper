using Minesweeper.Models;

namespace Minesweeper
{
    partial class Program
    {
        public static void Main()
        {
            int width;
            do
            {
                Console.Write("Matrix width? (default 10): ");
                try
                {
                    width = int.Parse(Console.ReadLine()!);
                }
                catch (Exception)
                {
                    width = 10;
                }
            } while (width <= 3 || width > 100);

            int height;
            do
            {
                Console.Write("Matrix height? (default 10): ");
                try
                {
                    height = int.Parse(Console.ReadLine()!);
                }
                catch (Exception)
                {
                    height = 10;
                }
            } while (height <= 3 || height > 100);

            Game gameInstance = new Game(width, height);

            Utils.Position gameTogglePos = new Utils.Position(0, 0);
            bool mustBreak = false;

            do
            {
                Console.Clear();

                gameInstance.PrintMatrix(mustBreak ? null : gameTogglePos);

                if (mustBreak)
                {
                    Console.WriteLine("Game Over");
                    Thread.Sleep(5000);
                    break;
                }

                while (!Console.KeyAvailable) ;
                ConsoleKeyInfo cki = Console.ReadKey();

                switch (cki.Key)
                {
                    case ConsoleKey.D:
                        {
                            if (gameTogglePos.x + 1 < width)
                                gameTogglePos.x++;
                            break;
                        }
                    case ConsoleKey.A:
                        {
                            if (gameTogglePos.x - 1 >= 0)
                                gameTogglePos.x--;
                            break;
                        }
                    case ConsoleKey.W:
                        {
                            if (gameTogglePos.y - 1 >= 0)
                                gameTogglePos.y--;
                            break;
                        }
                    case ConsoleKey.S:
                        {
                            if (gameTogglePos.y + 1 < height)
                                gameTogglePos.y++;
                            break;
                        }
                    case ConsoleKey.Spacebar:
                        {
                            if (!gameInstance.isReady)
                                gameInstance.Ready(gameTogglePos);

                            gameInstance.Reveal(gameTogglePos);
                            break;
                        }
                    case ConsoleKey.F:
                        {
                            gameInstance.FlagToggle(gameTogglePos);
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            mustBreak = true;
                            break;
                        }
                }

                if (gameInstance.GameOver)
                    mustBreak = true;
            } while (true);
        }
    }
}
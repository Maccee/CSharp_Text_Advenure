// modules/InputHandler.cs

using System;

namespace ConsoleApp.Modules
{
    public class InputHandler
    {
        public (int, int, bool) GetMovement()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            int dx = 0;
            int dy = 0;
            bool quit = false;

            switch (keyInfo.Key)
            {
                case ConsoleKey.D1: // bottom-left
                    dx = -1;
                    dy = 1;
                    break;
                case ConsoleKey.D2: // down
                    dx = 0;
                    dy = 1;
                    break;
                case ConsoleKey.D3: // bottom-right
                    dx = 1;
                    dy = 1;
                    break;
                case ConsoleKey.D4: // left
                    dx = -1;
                    dy = 0;
                    break;
                case ConsoleKey.D6: // right
                    dx = 1;
                    dy = 0;
                    break;
                case ConsoleKey.D7: // top-left
                    dx = -1;
                    dy = -1;
                    break;
                case ConsoleKey.D8: // up
                    dx = 0;
                    dy = -1;
                    break;
                case ConsoleKey.D9: // top-right
                    dx = 1;
                    dy = -1;
                    break;
                case ConsoleKey.X:

                    quit = true;
                    break;
            }
            
            return (dx, dy, quit);
        }
        public char GetCombatInput()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            while (true)
            {
                if (keyInfo.Key == ConsoleKey.A)
                {
                    return 'A';
                }
                else if (keyInfo.Key == ConsoleKey.R)
                {
                    return 'R';
                }
                else
                {
                    keyInfo = Console.ReadKey(intercept: true);
                }
            }
        }
    }
}

// modules/InputHandler.cs

using System;
using System.Collections;

namespace ConsoleApp.Modules
{
    public class InputHandler
    {
        public static (int, int, bool) GetMovement()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            int dx = 0;
            int dy = 0;
            bool quit = false;

            switch (keyInfo.KeyChar)
            {
                case '1': // bottom-left
                    dx = -1;
                    dy = 1;
                    break;
                case '2': // down
                    dx = 0;
                    dy = 1;
                    break;
                case '3': // bottom-right
                    dx = 1;
                    dy = 1;
                    break;
                case '4': // left
                    dx = -1;
                    dy = 0;
                    break;
                case '6': // right
                    dx = 1;
                    dy = 0;
                    break;
                case '7': // top-left
                    dx = -1;
                    dy = -1;
                    break;
                case '8': // up
                    dx = 0;
                    dy = -1;
                    break;
                case '9': // top-right
                    dx = 1;
                    dy = -1;
                    break;
                case 'X':
                    quit = true;
                    break;
                case 'x':
                    quit = true;
                    break;
            }

            return (dx, dy, quit);
        }
        public static char GetCombatInput()
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

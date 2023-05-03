// modules/ViewPort.cs
using System;

namespace ConsoleApp.Modules
{
    public class ViewPort
    {
        // Viewport dimensions
        public const int ViewPortWidth = 15;
        public const int ViewPortHeight = 7;

        private static char playerTile = Program.previousTile;
        public static bool statusWindow = false;
        private static Player? player = Program.player;
        private static TileManager? tileManager = Program.tileManager;

        public static void StatusWindow()
        {
            Console.CursorVisible = false;
            if (player == null) { return; }
            // Print player stats
            Console.SetCursorPosition(20, 1);
            PrintColored($"► Level {player.Level} {player.Name}\n");
            Console.SetCursorPosition(20, 2);
            
            PrintColored($"► Experience: {player.Exp}\n");
            Console.SetCursorPosition(20, 4);
            PrintColored($"► Hitpoints: {player.HP}/{player.MaxHp}\n");

        }

        static bool IsLatinLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
        static void PrintColored(string value)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            foreach (char c in value)
            {
                if (char.IsDigit(c))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else if (char.IsLetter(c))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(c);
            }
            Console.ForegroundColor = originalColor;
        }
        public static void CheckTilePlayerIsOn()
        {
            if (tileManager == null) { return; }
            foreach (var tile in tileManager.Tiles.Values)
            {
                if (tile.Char == Program.previousTile.ToString())
                {
                     Console.CursorVisible = false;
                    PrintColored(tile.Name + "   ");
                    break;
                }
            }
        }

        //
        // Print the visible map area around the player
        //
        public static void Print(char[,] mapArray)
        {
            if (player == null || tileManager == null) { return; }
            int mapHeight = mapArray.GetLength(0);
            int mapWidth = mapArray.GetLength(1);

            // Calculate the start and end positions of the viewport on the map
            int startX = Math.Max(0, player.X - ViewPortWidth / 2);
            int startY = Math.Max(0, player.Y - ViewPortHeight / 2);
            int endX = Math.Min(mapWidth, startX + ViewPortWidth);
            int endY = Math.Min(mapHeight, startY + ViewPortHeight);

            // Adjust viewport if it goes out of bounds
            if (endX == mapWidth)
            {
                startX = Math.Max(0, endX - ViewPortWidth);
            }
            if (endY == mapHeight)
            {
                startY = Math.Max(0, endY - ViewPortHeight);
            }

            if (!statusWindow)
            {
                StatusWindow();
                statusWindow = true;

            }


            // Start printing
            Console.SetCursorPosition(0, 1);
            
            // Top Border of the map
            Console.Write(" ╔");
            Console.WriteLine("".PadLeft(ViewPortWidth, '═') + "╗");

            // Print the map with the frame
            for (int y = startY; y < endY; y++)
            {
                // Print the top frame
                Console.ResetColor();
                Console.Write(" ║");

                for (int x = startX; x < endX; x++)
                {
                    // Check line of sight before printing the tile
                    if (HasLineOfSight(mapArray, player.X, player.Y, x, y))
                    {
                        char currentTile = mapArray[y, x];
                        // Set color for each tile type
                        ChangeTileColor(currentTile, tileManager);
                        // Print the tile
                        Console.Write(currentTile);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                // Print the right frame border
                Console.ResetColor();
                Console.WriteLine("║");
            }
            // Print the bottom frame border
            Console.ResetColor();
            Console.Write(" ╚");
            Console.WriteLine("".PadLeft(ViewPortWidth, '═') + "╝");

            // Location in map
            Console.ResetColor();
            PrintColored($" {player.Y + 1}, {player.X + 1} : ");
            

            // Print the tile description player is on
            CheckTilePlayerIsOn();
            
        }


        private static void ChangeTileColor(char currentTile, TileManager tileManager)
        {
            foreach (var tile in tileManager.Tiles.Values)
            {
                if (tile.Char == currentTile.ToString())
                {
                    if (!string.IsNullOrEmpty(tile.Color))
                    {
                        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), tile.Color);
                    }
                    else
                    {
                        // Set a default color if tile.Color is null or empty, e.g., ConsoleColor.White
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    break;
                }
            }
        }

        // Check if there's a line of sight between two points on the map
        private static bool HasLineOfSight(char[,] map, int x0, int y0, int x1, int y1)
        {
            // Calculate the differences and signs for x and y directions
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            // Initialize the error variable for the Bresenham algorithm
            int err = dx - dy;

            // Flag to allow only the first forest tile in the line of sight
            bool firstForestTile = true;

            // Bresenham line algorithm
            while (true)
            {
                // If the start and end points are the same, return true (line of sight exists)
                if (x0 == x1 && y0 == y1) return true;

                // If the current tile is a forest tile and not the starting point
                if (map[y0, x0] == 'F' && (x0 != x1 || y0 != y1))
                {
                    // If the first forest tile has already been encountered, return false (line of sight blocked)
                    if (!firstForestTile) return false;

                    // Mark the first forest tile as encountered
                    firstForestTile = false;
                }

                // If the current tile is a wall tile and not the starting point, return false (line of sight blocked)
                if (map[y0, x0] == '0' && (x0 != x1 || y0 != y1)) return false;

                // Update the error variable
                int e2 = 2 * err;

                // Move in the x direction if needed
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                // Move in the y direction if needed
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
}

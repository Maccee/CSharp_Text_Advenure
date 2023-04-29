using ConsoleApp.Modules;
using System.Threading;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize dictionary for map tiles
            var tileManager = new TileManager("./tile.json");

            // Generate game map
            var mapGenerator = new MapGenerator();
            mapGenerator.GenerateMap();
            char[,] mapArray = mapGenerator.GetMap();

            // InputHandler is responsible for any input
            var inputHandler = new InputHandler();

            // Check if there is a random encounter (1% chance)
            int encounterChance = 1;
            bool quit = false;

            Start();

            void Start()
            {
                Console.Clear();

                // Initialize player start position
                var player = new Player(mapArray, tileManager);
                char previousTile;
                previousTile = mapArray[player.Y, player.X];
                mapArray[player.Y, player.X] = player.Character;

                // Initialize the viewPort which is responsible for printing main screen
                var viewPort = new ViewPort();

                while (true) // Main game loop
                {
                    Thread.Sleep(100);
                    // Player dead, restart in same map.
                    if (player.playerDead)
                    {
                        mapArray[player.Y, player.X] = previousTile;
                        Start();
                    }
                    // Player is or is not in combat
                    if (!player.IsInCombat)
                    {
                        Console.Clear();

                        // Update main screen
                        // mapArray for mapData, player for playerData, tileManager for tileData and previousTile where the player moved away
                        viewPort.Print(mapArray, player, tileManager, previousTile);

                        // Get player's movement input
                        (int dx, int dy, quit) = inputHandler.GetMovement();

                        // Pressing X sets bool quit to true and ends the main loop
                        if (quit)
                        {
                            Environment.Exit(0);
                        }

                        // Saves the tile where player moved out for viewport to print the tile back
                        mapArray[player.Y, player.X] = previousTile;

                        // Try to move the player and update their position on the map
                        // The Move logic is in player class
                        bool moveSuccessful = player.Move(dx, dy, mapArray);

                        if (moveSuccessful)
                        {
                            previousTile = mapArray[player.Y, player.X];
                        }
                        // Update the player's position on the map
                        mapArray[player.Y, player.X] = player.Character;
                    }
                    else
                    {
                        var combat = new Combat();
                        combat.StartCombat(player, inputHandler);
                    }
                    // Check if there is a random encounter
                    if (new Random().Next(0, 100) < encounterChance)
                    {
                        if (player.moveFirst > 0)
                        {
                            player.IsInCombat = true;
                            player.moveFirst = 0;
                        }
                        player.moveFirst++;
                    }
                }
            }
        }
    }
}

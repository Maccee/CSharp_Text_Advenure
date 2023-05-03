using ConsoleApp.Modules;

namespace ConsoleApp
{
    class Program
    {
        //public static MapGenerator mapGenerator = new MapGenerator();
        public static Player? player;
        public static TileManager? tileManager;
        public static InputHandler? inputHandler;
        public static Combat? combat;



        public static char previousTile;


        static void Main(string[] args)
        {
            Console.Clear();
            Console.CursorVisible = false;
            // Initialize dictionary for map tiles
            tileManager = new TileManager("./tile.json");

            // Generate game map
            MapGenerator.GenerateMap();
            char[,] mapArray = MapGenerator.GetMap();

            // Check if there is a random encounter (1% chance)
            int encounterChance = 1;
            bool quit = false;

            Start();

            void Start()
            {
                Console.Clear();

                // Initialize player and start position
                player = new Player(mapArray);
                // Store the tile where player is placed
                previousTile = mapArray[player.Y, player.X];
                // Put player character in the map
                mapArray[player.Y, player.X] = player.Character;


                while (true) // Main game loop
                {

                    // Player dead, restart in same map.
                    if (player.playerDead)
                    {
                        // Put tile back to map where player died and start from beginning.
                        mapArray[player.Y, player.X] = previousTile;
                        Start();
                    }

                    // Player is not in combat
                    if (!player.IsInCombat)
                    {

                        // Update main screen
                        ViewPort.Print(mapArray);

                        // Loop waits to get player's movement input
                        (int dx, int dy, quit) = InputHandler.GetMovement();

                        // Pressing X sets 'bool quit true' and ends the main loop
                        if (quit)
                        {
                            Console.CursorVisible = true;
                            Environment.Exit(0);
                        }

                        // Sets the tile, where player moved out, for viewport to print the tile back
                        mapArray[player.Y, player.X] = previousTile;

                        // Try to move the player, check if tile is passable
                        // The Move logic is in player class
                        // Returns true if tile is passable
                        bool moveSuccessful = player.Move(dx, dy, mapArray);

                        if (moveSuccessful)
                        {
                            // Stores the tile player is going to move to previousTile
                            previousTile = mapArray[player.Y, player.X];

                        }
                        // Puts player.Character to players new position
                        mapArray[player.Y, player.X] = player.Character;

                    }
                    else
                    {
                        var combat = new Combat();
                        combat.StartCombat();
                    }
                    // Check if there is a random encounter
                    if (new Random().Next(0, 100) < encounterChance)
                    {
                        // Give player chance to skip 2 combats before stargin new combat
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

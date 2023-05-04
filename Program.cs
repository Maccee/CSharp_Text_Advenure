using ConsoleApp.Modules;

namespace ConsoleApp
{
    class Program
    {
        public static Player? player;
        public static TileManager? tileManager;
        public static char[,]? mapArray;
        public static char previousTile;
        public static int prevY;
        public static int prevX;

        static void Main(string[] args)
        {
            InitializeGame();
            GameLoop();
        }

        static void InitializeGame()
        {
            Console.Clear();
            Console.CursorVisible = false;

            tileManager = new TileManager("./tile.json");
            MapGenerator.GenerateMap();
            mapArray = MapGenerator.GetMap();

            player = new Player(mapArray);
            previousTile = mapArray[player.Y, player.X];
            mapArray[player.Y, player.X] = player.Character;
        }

        static void GameLoop()
        {
            int encounterChance = 10;
            bool quit = false;

            while (true)
            {
                if (player == null || mapArray == null) { return; }
                if (player.playerDead)
                {
                    HandlePlayerDeath();
                }

                if (!player.IsInCombat)
                {
                    ViewPort.Print(mapArray);
                    (int dx, int dy, quit) = InputHandler.GetMovement();

                    if (quit)
                    {
                        QuitGame();
                    }

                    HandlePlayerMovement(dx, dy);
                }
                else
                {
                    HandleCombat();
                }

                CheckForRandomEncounter(encounterChance);
            }
        }

        static void HandlePlayerDeath()
        {
            if (player == null || mapArray == null) { return; }
            player.playerDead = false;
        }

        static void HandlePlayerMovement(int dx, int dy)
        {
            if (player == null || mapArray == null) { return; }
            mapArray[player.Y, player.X] = previousTile;
            (prevY, prevX) = (player.Y, player.X);
            bool moveSuccessful = player.Move(dx, dy, mapArray);

            if (moveSuccessful)
            {
                previousTile = mapArray[player.Y, player.X];
            }

            mapArray[player.Y, player.X] = player.Character;
        }

        static void HandleCombat()
        {
            var combat = new Combat();
            combat.StartCombat();
        }

        static void CheckForRandomEncounter(int encounterChance)
        {
            if (player == null || mapArray == null) { return; }
            int randomNumber = new Random().Next(0, 100);
            
            if (randomNumber < encounterChance)
            {
                if (player.moveFirst > 0)
                {
                    player.IsInCombat = true;
                    player.moveFirst = 0;
                }
                player.moveFirst++;
            }
        }

        static void QuitGame()
        {
            Console.CursorVisible = true;
            Environment.Exit(0);
        }
    }
}

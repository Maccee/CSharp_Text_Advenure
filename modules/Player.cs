// modules/Player.cs
using System;

namespace ConsoleApp.Modules
{
    public class Player
    {
        // Player properties
        public int X { get; set; }
        public int Y { get; set; }
        public char Character { get; private set; }
        public int Gold { get; private set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public string Name { get; private set; }
        public int MaxHp { get; private set; }
        public int HP { get; set; }
        public char PlayerTile { get; private set; }
        public bool IsInCombat { get; set; }
        public int moveFirst { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int requiredExp { get; set; }
        private TileManager? tileManager = Program.tileManager;
        public bool playerDead;


        // Constructor with default values for character, name, attack, defense, and health points
        public Player(char[,] mapArray)
        {
            //this.tileManager = tileManager;
            this.moveFirst = 1;
            this.playerDead = false;

            Character = '@';
            Name = "Adventurer";
            ATK = 10;
            DEF = 10;
            MaxHp = 100;
            HP = 100;
            Level = 1;
            (Y, X) = GetRandomStartPosition(mapArray); // Swap the returned values
        }

        // Get a random starting position for the player on the map
        public (int, int) GetRandomStartPosition(char[,] mapArray)
        {
            int mapHeight = mapArray.GetLength(0);
            int mapWidth = mapArray.GetLength(1);
            Random rnd = new Random();
            int x, y;

            // Add a buffer zone around the map for the player's initial position
            int bufferX = ViewPort.ViewPortWidth / 2;
            int bufferY = ViewPort.ViewPortHeight / 2;

            // Loop until a valid starting position is found
            while (true)
            {
                x = rnd.Next(bufferY, mapHeight - bufferY);
                y = rnd.Next(bufferX, mapWidth - bufferX);

                // Check if the position is valid (not a wall)
                if (mapArray[x, y] == '.')
                {
                    break;
                }
            }
            // Reset player
            this.MaxHp = 100;
            this.HP = this.MaxHp;
            this.Level = 1;
            this.Exp = 0;
            this.requiredExp = 100;
            this.ATK = 10;
            this.DEF = 10;
            Console.Clear();
            return (x, y);
        }

        // Move the player and update their position on the map
        public bool Move(int dx, int dy, char[,] mapArray)
        {
            if (tileManager == null) { return false; }
            // gets the new movement direction from the inputhandler
            int newX = X + dx;
            int newY = Y + dy;

            // Check if the new position is within the map bounds
            if (newX >= 0 && newX < mapArray.GetLength(1) && newY >= 0 && newY < mapArray.GetLength(0))
            {
                char newPositionTile = mapArray[newY, newX];

                // Check if the tile newPositionTile is passable using the TileManager instance
                bool isPassable = false;
                foreach (var tileKey in tileManager.Tiles.Keys)
                {
                    Tile tile = tileManager.Tiles[tileKey];
                    if (tile?.Char?[0] == newPositionTile)
                    {
                        isPassable = tile.Passable;
                        break;
                    }
                }

                // Moves the player char to new position if tile.passable is true
                if (isPassable)
                {
                    // saves the tile player is on and update player position
                    PlayerTile = newPositionTile;
                    X = newX;
                    Y = newY;

                    // Regenerate player hp logic if move is successful
                    if (this.HP < this.MaxHp)
                    {
                        Random rnd = new Random();
                        int chance = rnd.Next(1, 3);
                        if (chance == 1)
                        {
                            this.HP++;
                            ViewPort.StatusWindow();
                        }
                    }
                    // move is successful and main updates mapArray with the previous tile
                    return true;
                }
            }
            // move is not successful, player does not move and mapArray is not updated
            return false;
        }
    }
}


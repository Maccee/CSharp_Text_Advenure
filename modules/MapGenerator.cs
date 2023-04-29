// modules/MapGenerator.cs

namespace ConsoleApp.Modules
{
    public class MapGenerator
    {
        private int mapWidth = 100;
        private int mapHeight = 25;
        private int forestPercentage = 25;
        private int requiredAdjacentForest = 2;

        private int rockFormations = 10;
        private char[,] map;
        public char OldTreasureTile { get; private set; }



        public MapGenerator()
        {
            this.map = new char[mapHeight, mapWidth];
        }

        public void GenerateMap()
        {
            FillMapWithForest();
            LonelyForestRemoval();
            FillSurroundedGrassWithForest();
            GenerateRockFormations();
            //AddTreasure();
            WriteMapToFile();


        }
        public char[,] GetMap()
        {
            char[,] currentMap = new char[mapHeight, mapWidth];
            Array.Copy(map, currentMap, map.Length);
            return currentMap;
        }
        public char[,] CreateMapArray(string[] map)
        {
            char[,] mapArray = new char[map.Length, map[0].Length];

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    mapArray[i, j] = map[i][j];
                }
            }

            return mapArray;
        }

        private void FillMapWithForest()
        {
            Random rnd = new Random();
            for (int x = 0; x < mapHeight; x++)
            {
                for (int y = 0; y < mapWidth; y++)
                {
                    if (rnd.Next(100) < forestPercentage)
                    {
                        map[x, y] = 'F';  // Randomly add forest in open areas
                    }
                    else
                    {
                        map[x, y] = '.';  // Default to grass
                    }
                }
            }
        }

        private void LonelyForestRemoval()
        {
            for (int x = 0; x < mapHeight; x++)
            {
                for (int y = 0; y < mapWidth; y++)
                {
                    if (map[x, y] == 'F')
                    {
                        int adjacentForestCount = 0;
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                int nx = x + dx;
                                int ny = y + dy;
                                if (nx >= 0 && nx < mapHeight && ny >= 0 && ny < mapWidth)
                                {
                                    if (map[nx, ny] == 'F')
                                    {
                                        adjacentForestCount++;
                                    }
                                }
                            }
                        }
                        if (adjacentForestCount < requiredAdjacentForest)
                        {
                            map[x, y] = '.';
                        }
                    }
                }
            }
        }

        private void FillSurroundedGrassWithForest()
        {
            char[,] currentMap = GetMap();

            for (int x = 1; x < mapHeight - 1; x++)
            {
                for (int y = 1; y < mapWidth - 1; y++)
                {
                    if (map[x, y] == '.')
                    {
                        int adjacentForestCount = 0;
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                int nx = x + dx;
                                int ny = y + dy;
                                if (currentMap[nx, ny] == 'F')
                                {
                                    adjacentForestCount++;
                                }
                            }
                        }
                        if (adjacentForestCount >= requiredAdjacentForest)
                        {
                            map[x, y] = 'F';
                        }
                    }
                }
            }
        }

        private void GenerateRockFormations()
        {
            Random rnd = new Random();
            for (int i = 0; i < rockFormations; i++)
            {
                int chunkSize = rnd.Next(10, 35); // Random size between 6 and 24
                int startX = rnd.Next(0, mapWidth);
                int startY = rnd.Next(0, mapHeight);

                // Initialize the queue for BFS
                Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
                queue.Enqueue((startX, startY));
                int rocksGenerated = 0;

                while (queue.Count > 0 && rocksGenerated < chunkSize)
                {
                    var position = queue.Dequeue();
                    int x = position.x;
                    int y = position.y;

                    if (map[y, x] != '0')
                    {
                        map[y, x] = '0';
                        rocksGenerated++;

                        AddGravel(x, y); // Add gravel around the rock

                        // Enqueue adjacent positions
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                int newX = x + dx;
                                int newY = y + dy;

                                if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapHeight)
                                {
                                    queue.Enqueue((newX, newY));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddGravel(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int newX = x + dx;
                    int newY = y + dy;

                    if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapHeight)
                    {
                        if (map[newY, newX] != '0' && map[newY, newX] != 'o') // Check if the tile is not rock or gravel
                        {
                            map[newY, newX] = 'o'; // Set the tile to gravel
                        }
                    }
                }
            }
        }
        private void AddTreasure()
        {
            Random rnd = new Random();
            int x, y;

            while (true)
            {
                x = rnd.Next(mapHeight);
                y = rnd.Next(mapWidth);

                if (map[x, y] == '.' || map[x, y] == 'F' || map[x, y] == 'o')
                {
                    OldTreasureTile = map[x, y]; // Store the original tile before the treasure was added
                    map[x, y] = '$';
                    break;
                }
            }
        }

        public char GetOldTreasureTile()
        {
            return OldTreasureTile;
        }


        private void WriteMapToFile()
        {
            string filename = "map.txt";
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int x = 0; x < mapHeight; x++)
                {
                    for (int y = 0; y < mapWidth; y++)
                    {
                        writer.Write(map[x, y]);
                    }
                    writer.WriteLine();
                }
            }
            //Console.WriteLine("Map saved to file.");
        }
    }
}
// modules/MapGenerator.cs
namespace ConsoleApp.Modules
{
    public class MapGenerator
    {
        private static int mapWidth = 100;
        private static int mapHeight = 25;
        private static int forestPercentage = 25;
        private static int requiredAdjacentForest = 2;
        private static int rockFormations = 10;
        private static char[,] map = new char[mapHeight, mapWidth];
        private static Random rnd = new Random();

        public static void GenerateMap()
        {
            FillMapWithForest();
            LonelyForestRemoval();
            FillSurroundedGrassWithForest();
            GenerateRockFormations();
            WriteMapToFile();
        }
        public static char[,] GetMap()
        {
            char[,] currentMap = new char[mapHeight, mapWidth];
            Array.Copy(map, currentMap, map.Length);
            return currentMap;
        }
        public static char[,] CreateMapArray(string[] map)
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
        private static void FillMapWithForest()
        {
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
        private static void LonelyForestRemoval()
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
        private static void FillSurroundedGrassWithForest()
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
        private static void GenerateRockFormations()
        {
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
        private static void AddGravel(int x, int y)
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
        private static void WriteMapToFile()
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
        }
    }
}
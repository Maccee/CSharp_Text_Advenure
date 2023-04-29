using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ConsoleApp.Modules
{
    public class TileManager
    {
        public Dictionary<string, Tile> Tiles { get; set; }

        public TileManager(string filename)
        {
            // Initialize the Tiles dictionary
            Tiles = new Dictionary<string, Tile>();

            using (StreamReader reader = new StreamReader(filename))
            {
                string json = reader.ReadToEnd();
                Tiles = JsonConvert.DeserializeObject<Dictionary<string, Tile>>(json) ?? new Dictionary<string, Tile>();
            }
        }
        public void PrintTiles()
    {
        foreach (var keyValuePair in Tiles)
        {
            Console.WriteLine($"Key: {keyValuePair.Key}, Value: {keyValuePair.Value}");
            Console.WriteLine($"Key: {keyValuePair.Key}, Value: {keyValuePair.Value}, Char: {keyValuePair.Value.Char}");
        }
    }
    }
}

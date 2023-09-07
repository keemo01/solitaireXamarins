using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class GameData
{
    public int[,] gameBoard; // The game board data
    public int kills;        // The number of kills

    // Constructor to initialize game data with provided values
    public GameData(int[,] board, int killCount)
    {
        gameBoard = board;
        kills = killCount;
    }

    // Default constructor (parameterless) for deserialization
    public GameData()
    {
    }

    // Save the game data to a JSON file
    public void SaveGame(string filePath)
    {
        // Serialize the current instance of GameData to JSON
        string jsonData = JsonConvert.SerializeObject(this);

        // Write the JSON data to the specified file
        File.WriteAllText(filePath, jsonData);
    }

    // Load game data from a JSON file
    public static GameData LoadGame(string filePath)
    {
        if (File.Exists(filePath)) // Check if the file exists
        {
            // Read the JSON data from the file
            string jsonData = File.ReadAllText(filePath);

            // Deserialize the JSON data back into a GameData instance
            return JsonConvert.DeserializeObject<GameData>(jsonData);
        }
        else
        {
            return null; // Return null if the file doesn't exist
        }
    }
}

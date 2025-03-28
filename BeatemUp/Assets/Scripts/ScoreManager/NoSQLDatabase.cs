using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class NoSQLDatabase
{
    // Define the data structure for a player's score.  This is our "document"
    [System.Serializable]
    public class PlayerScore
    {
        public string PlayerName;
        public int Score;
        public string Timestamp; 
    }

    private string _databaseName; // Name of the database (folder)
    private string _collectionName; // Name of the collection (file)
    private string _databasePath;  // Full path to the database folder

    private List<PlayerScore> _data = new List<PlayerScore>(); // In-memory representation of the data.  Important for performance.
    private bool _isDataLoaded = false; // Flag to track if data has been loaded

    // Constructor:  Pass in the database name and collection.  Good practice to allow flexibility.
    public NoSQLDatabase(string databaseName, string collectionName)
    {
        _databaseName = databaseName;
        _collectionName = collectionName;
        _databasePath = Path.Combine(Application.persistentDataPath, _databaseName); // Use persistentDataPath
        Debug.Log("Database Path: " + _databasePath); //helpful
        LoadDatabase(); // Load data on initialization.
    }

    // Ensure the database directory exists.  Called by LoadDatabase.  Made public for more control
    public void EnsureDatabaseDirectoryExists()
    {
        if (!Directory.Exists(_databasePath))
        {
            Directory.CreateDirectory(_databasePath);
            Debug.Log("Created Directory: " + _databasePath);
        }
    }


    // Load the database from JSON file.  Use async for better performance.
    public async Task LoadDatabase()
    {
        // Check if data is already loaded
        if (_isDataLoaded)
        {
            Debug.Log("Data already loaded.");
            return;
        }

        EnsureDatabaseDirectoryExists(); //moved

        string filePath = Path.Combine(_databasePath, _collectionName + ".json");
        Debug.Log("Loading from: " + filePath);
        if (File.Exists(filePath))
        {
            try
            {
                string json = await File.ReadAllTextAsync(filePath); // Use async
                _data = JsonConvert.DeserializeObject<List<PlayerScore>>(json);
                if (_data == null)  // Handle the case where the file is empty or contains invalid JSON
                {
                    _data = new List<PlayerScore>();
                }
                Debug.Log("Database loaded successfully.");
            }
            catch (JsonException e)
            {
                Debug.LogError("Error loading database: " + e.Message);
                _data = new List<PlayerScore>(); // Initialize to an empty list on error to prevent null issues.
                // Consider logging the error to a file or displaying a message to the user.
            }
            catch (IOException e)
            {
                Debug.LogError("IO Error loading database: " + e.Message);
                _data = new List<PlayerScore>();
            }
        }
        else
        {
            Debug.Log("Database file does not exist.  Creating a new one.");
            _data = new List<PlayerScore>(); // Initialize if the file doesn't exist
            await SaveDatabase(); //saves empty file.
        }
        _isDataLoaded = true; //set to true
    }

    // Save the database to a JSON file.  Use async.
    public async Task SaveDatabase()
    {
        if (!_isDataLoaded)
        {
            Debug.LogWarning("Data not loaded, cannot save.");
            return; // Don't save if data hasn't been loaded
        }
        string filePath = Path.Combine(_databasePath, _collectionName + ".json");
        try
        {
            string json = JsonConvert.SerializeObject(_data, Newtonsoft.Json.Formatting.Indented); // Use Indented for readability
            await File.WriteAllTextAsync(filePath, json); // Use async
            Debug.Log("Database saved successfully to " + filePath);
        }
        catch (JsonException e)
        {
            Debug.LogError("Error saving database: " + e.Message);
            // Consider more robust error handling (e.g., retrying, notifying the user)
        }
        catch (IOException e)
        {
            Debug.LogError("IO Error saving database: " + e.Message);
        }
    }

    // Insert a new player score.  Added error handling and checks.
    public async Task InsertScore(string playerName, int score)
    {
        if (!_isDataLoaded)
        {
            await LoadDatabase(); // Ensure data is loaded before inserting
        }

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name cannot be null or empty.");
            return; // IMPORTANT:  Don't proceed with invalid data.
        }

        if (score < 0)
        {
            Debug.LogError("Score cannot be negative.");
            return;
        }
        PlayerScore newScore = new PlayerScore
        {
            PlayerName = playerName,
            Score = score,
            Timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // Store with timestamp
        };

        _data.Add(newScore); // Add to the in-memory list
        await SaveDatabase(); // Save the updated data to the file
    }

    // Get all scores.  Returns a copy to prevent external modification of the original data.
    public List<PlayerScore> GetAllScores()
    {
        if (!_isDataLoaded)
        {
            LoadDatabase();
        }
        return new List<PlayerScore>(_data); // Return a *copy* of the list.
    }

    // Get the top N scores, ordered by score descending.
    public List<PlayerScore> GetTopNScores(int n)
    {
        if (!_isDataLoaded)
        {
            LoadDatabase();
        }
        // Sort a copy to avoid modifying the original data.
        List<PlayerScore> sortedScores = new List<PlayerScore>(_data);
        sortedScores.Sort((a, b) => b.Score.CompareTo(a.Score)); // Sort descending
        if (n > sortedScores.Count)
        {
            n = sortedScores.Count; // prevent out of bounds
        }
        return sortedScores.GetRange(0, n); // Return the top N
    }

    // Find all scores for a specific player.
    public List<PlayerScore> GetScoresForPlayer(string playerName)
    {
        if (!_isDataLoaded)
        {
            LoadDatabase();
        }
        List<PlayerScore> playerScores = new List<PlayerScore>();
        foreach (var score in _data)
        {
            if (score.PlayerName == playerName) // Use == for string comparison
            {
                playerScores.Add(score);
            }
        }
        return playerScores;
    }

    // Clear all data.  Use with caution!  Added async.
    public async Task ClearDatabase()
    {
        if (!_isDataLoaded)
        {
            LoadDatabase();
        }
        _data.Clear(); // Clear the in-memory data
        await SaveDatabase(); // Save the empty data to the file
        Debug.Log("Database cleared.");
    }

    // Delete a specific score
    public async Task DeleteScore(string playerName, int score)
    {
        if (!_isDataLoaded)
        {
            LoadDatabase();
        }

        // Find the index of the score to delete.  Handles multiple entries.
        int indexToDelete = -1;
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].PlayerName == playerName && _data[i].Score == score)
            {
                indexToDelete = i;
                break; // Exit the loop after finding the first match
            }
        }

        if (indexToDelete != -1)
        {
            _data.RemoveAt(indexToDelete);
            await SaveDatabase();
            Debug.Log($"Deleted score for {playerName}: {score}");
        }
        else
        {
            Debug.LogWarning($"Score not found for {playerName}: {score}");
        }
    }
}

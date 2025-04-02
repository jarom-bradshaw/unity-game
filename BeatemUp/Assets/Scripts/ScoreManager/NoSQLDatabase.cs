using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class NoSQLDatabase
{
    [System.Serializable]
    public class PlayerScore
    {
        public float? CompletionTime;  // Time to complete the level, stored as a string
        public string Timestamp;       // Store the time the score was achieved
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
        }
        catch (IOException e)
        {
             Debug.LogError("IO Error saving database: " + e.Message);
        }
    }

    // Insert a new player score.  Added error handling and checks.
    public async Task InsertScore(float? completionTime)
    {
        if (!_isDataLoaded)
        {
            await LoadDatabase(); // Ensure data is loaded before inserting
        }

        if (completionTime == null)
        {
            Debug.LogError("Completion time cannot be null or empty.");
            return; // IMPORTANT:  Don't proceed with invalid data.
        }

        // Get the current high score (if exists)
        PlayerScore currentHighScore = _data.Count > 0 ? _data[0] : null;

        // If there's an existing high score, compare it
        if (currentHighScore != null)
        {
            float? storedTime = currentHighScore.CompletionTime; // Now it's a float
            if (completionTime >= storedTime) return; // Only update if new time is lower
        }

        // Store the new best time
        _data.Clear(); // Remove previous high score
        _data.Add(new PlayerScore
        {
            CompletionTime = completionTime,
            Timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });

        await SaveDatabase();
    }
    public float? GetHighScore()
    {
        if (!_isDataLoaded) LoadDatabase();
        
        return _data.Count > 0 ? _data[0].CompletionTime : null;
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

    // Get the top N scores, ordered by score descending.  In this case, the fastest times.
    public List<PlayerScore> GetTopNScores(int n)
    {
        if (!_isDataLoaded)
    {
        LoadDatabase();
    }

    // Sort in ascending order (fastest times first)
    List<PlayerScore> sortedScores = new List<PlayerScore>(_data);
    sortedScores.Sort((a, b) =>
    {
        if (!a.CompletionTime.HasValue) return 1;  // Null values go to the end
        if (!b.CompletionTime.HasValue) return -1; 
        return a.CompletionTime.Value.CompareTo(b.CompletionTime.Value);
    });

    if (n > sortedScores.Count)
    {
        n = sortedScores.Count; // prevent out of bounds
    }
    return sortedScores.GetRange(0, n); // Return the top N
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
    public async Task DeleteScore(float completionTime)
    {
        if (!_isDataLoaded)
        {
            LoadDatabase();
        }

        // Find the index of the score to delete.  Handles multiple entries.
        int indexToDelete = -1;
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].CompletionTime == completionTime)
            {
                indexToDelete = i;
                break; // Exit the loop after finding the first match
            }
        }

        if (indexToDelete != -1)
        {
            _data.RemoveAt(indexToDelete);
            await SaveDatabase();
            Debug.Log($"Deleted score: {completionTime}");
        }
        else
        {
            Debug.LogWarning($"Score not found: {completionTime}");
        }
    }
}

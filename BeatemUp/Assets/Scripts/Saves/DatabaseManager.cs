using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        // Define the database path and log it for debugging
        dbPath = "URI=file:" + Application.persistentDataPath + "/GameDatabase.db";
        Debug.Log("Database path: " + dbPath);

        // Initialize database tables
        CreateSaveStatesTable();
        CreateWeaponsTable();
        CreatePlayersTable();

    }

    // Creates the save_states table if it doesn't exist
    private void CreateSaveStatesTable()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS save_states (
                    save_states_id INTEGER PRIMARY KEY, 
                    save_slot INT, 
                    playtime INT, 
                    game_location TEXT, 
                    last_saved TEXT DEFAULT (datetime('now'))
                );";
            command.ExecuteNonQuery();
        }

        Debug.Log("save_states table created successfully");
    }

    // Creates the players table if it doesn't exist
    private void CreatePlayersTable()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS players (
                    player_id INTEGER PRIMARY KEY, 
                    name TEXT, 
                    health INT DEFAULT (100), 
                    weapons_id INT,
                    save_states_id INT,
                    FOREIGN KEY (weapons_id) REFERENCES weapons (weapons_id), 
                    FOREIGN KEY (save_states_id) REFERENCES save_states (save_states_id)
                );";
            command.ExecuteNonQuery();
        }

        Debug.Log("players table created successfully");
    }

    // Creates the weapons table if it doesn't exist
    private void CreateWeaponsTable()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS weapons (
                    weapons_id INTEGER PRIMARY KEY, 
                    weapon_name TEXT
                );";
            command.ExecuteNonQuery();
        }

        Debug.Log("weapons table created successfully");
    }

    // Retrieves save data for a given save slot
    public SaveData GetSaveData(int saveSlot)
    {
        SaveData saveData = null;

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            // Query to retrieve game location for the save slot
            string locationQuery = @"
            SELECT game_location FROM save_states 
            WHERE save_slot = @saveSlot
            LIMIT 1";

            using (var command = new SqliteCommand(locationQuery, connection))
            {
                command.Parameters.AddWithValue("@saveSlot", saveSlot);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        saveData = new SaveData(reader.GetString(0));
                    }
                    else
                    {
                        return null; // No save data found
                    }
                }
            }

            // Query to retrieve players and their weapons for the save slot
            string playerQuery = @"
            SELECT p.player_id, p.name, p.HEALTH, w.weapon_name 
            FROM players p
            LEFT JOIN weapons w ON p.weapons_id = w.weapons_id
            WHERE p.save_states_id = (SELECT save_states_id FROM save_states WHERE save_slot = @saveSlot LIMIT 1)";

            using (var command = new SqliteCommand(playerQuery, connection))
            {
                command.Parameters.AddWithValue("@saveSlot", saveSlot);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int playerId = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int health = reader.GetInt32(2);
                        string weaponName = reader.IsDBNull(3) ? "None" : reader.GetString(3);

                        saveData.players.Add(new PlayerData(playerId, name, health, weaponName));
                    }
                }
            }
        }

        return saveData;
    }

    // Saves game data, replacing any existing data for the given save slot
    public void SaveGameData(int saveSlot, string location, List<PlayerData> players)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction()) // Ensure atomic operations
            {
                try
                {
                    // Delete old save state & associated players
                    string deleteQuery = @"
                    DELETE FROM players WHERE save_states_id = (SELECT save_states_id FROM save_states WHERE save_slot = @saveSlot);
                    DELETE FROM save_states WHERE save_slot = @saveSlot;
                ";

                    using (var command = new SqliteCommand(deleteQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@saveSlot", saveSlot);
                        command.ExecuteNonQuery();
                    }

                    // Insert new save state
                    string insertSaveStateQuery = @"
                    INSERT INTO save_states (save_slot, game_location, playtime, last_saved)
                    VALUES (@saveSlot, @location, 0, datetime('now'));
                ";

                    using (var command = new SqliteCommand(insertSaveStateQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@saveSlot", saveSlot);
                        command.Parameters.AddWithValue("@location", location);
                        command.ExecuteNonQuery();
                    }

                    // Get the newly created save_state_id
                    long saveStateId;
                    using (var command = new SqliteCommand("SELECT last_insert_rowid();", connection, transaction))
                    {
                        saveStateId = (long)command.ExecuteScalar();
                    }

                    // Insert players & weapons
                    foreach (var player in players)
                    {
                        long weaponId = GetOrCreateWeaponId(player.weaponName, connection, transaction);

                        string insertPlayerQuery = @"
                        INSERT INTO players (name, HEALTH, weapons_id, save_states_id) 
                        VALUES (@name, @health, @weaponId, @saveStateId);
                    ";

                        using (var command = new SqliteCommand(insertPlayerQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@name", player.name);
                            command.Parameters.AddWithValue("@health", player.health);
                            command.Parameters.AddWithValue("@weaponId", weaponId);
                            command.Parameters.AddWithValue("@saveStateId", saveStateId);
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    Debug.Log("Game saved successfully!");
                }
                catch (System.Exception ex)
                {
                    transaction.Rollback();
                    Debug.LogError("Error saving game: " + ex.Message);
                }
            }
        }
    }

    private long GetOrCreateWeaponId(string weaponName, SqliteConnection connection, SqliteTransaction transaction)
    {
        string selectWeaponQuery = "SELECT weapons_id FROM weapons WHERE weapon_name = @weaponName";

        using (var command = new SqliteCommand(selectWeaponQuery, connection, transaction))
        {
            command.Parameters.AddWithValue("@weaponName", weaponName);
            var result = command.ExecuteScalar();
            if (result != null)
            {
                return (long)result;
            }
        }

        // If weapon doesn't exist, insert it
        string insertWeaponQuery = "INSERT INTO weapons (weapon_name) VALUES (@weaponName)";

        using (var command = new SqliteCommand(insertWeaponQuery, connection, transaction))
        {
            command.Parameters.AddWithValue("@weaponName", weaponName);
            command.ExecuteNonQuery();
        }

        using (var command = new SqliteCommand("SELECT last_insert_rowid();", connection, transaction))
        {
            return (long)command.ExecuteScalar();
        }
    }

}

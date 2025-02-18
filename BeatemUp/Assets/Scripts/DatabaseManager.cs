using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/GameDatabase.db";

        CreateSaveStatesTable();
        CreateWeaponsTable();
        CreatePlayersTable();
    }

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
}

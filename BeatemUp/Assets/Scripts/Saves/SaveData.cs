using UnityEngine;
using System.Collections.Generic;

public class SaveData
{
    public string location;
    public List<PlayerData> players = new List<PlayerData>();

    public SaveData(string location)
    {
        this.location = location;
    }
}

public class PlayerData
{
    public int playerId;
    public string name;
    public int health;
    public string weaponName;

    public PlayerData(int playerId, string name, int health, string weaponName)
    {
        this.playerId = playerId;
        this.name = name;
        this.health = health;
        this.weaponName = weaponName;
    }
}

using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    // *** this Object is for save data on game runtime ***
    // *** PlayerStats -> for save and load -> convert to PlayerData for runtime ***

    // player profile
    [Header("Player Profile")]
    public string playerName;
    public string savePath;
    public int playerShipIcon;

    // hangar
    [Header("Hangar and Shop")]
    public List<int> moduleCounts = new();
    public List<ModuleData> moduleData;
    public int credits;
    public int shopLevelVisited = 0;

    // Game
    [Header("Game")]
    public int bossLevel = 0;
}

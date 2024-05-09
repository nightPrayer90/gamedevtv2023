using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerStats 
{
    // ** Important: this class is only temporay for save and load data *** 
    // *** PlayerStats -> for save and load -> convert to PlayerData for runtime ***

    // player profile
    public string playerName = "default-player";
    public int shipPanelIndex = 0;
    public string savePath = "/player-statsDefault.json";

    // Hangar + Shop
    public List<int> moduleCounts = new();
    public List<ModuleData>[] shipsModuleData = new List<ModuleData>[4];
    public int activeShip = 0;
    public int credits = 0;
    public int shopLevelVisited = 0;

    // Game
    public int bossLevel = 0;

    // Skill Bord
    public List<bool> skillsSpotted = new();

    public PlayerStats() { }
    public PlayerStats(PlayerData data)
    {
        // PlayerProfil
        playerName = data.playerName;
        savePath = data.savePath;
        shipPanelIndex = data.playerShipIcon;

        // Hangar and Shop
        shipsModuleData = data.ShipsModuleData;
        moduleCounts = data.moduleCounts;
        credits = data.credits;
        bossLevel = data.bossLevel;
        shopLevelVisited = data.shopLevelVisited;

        // Skillboard
        skillsSpotted = data.skillsSpotted;
    }

}

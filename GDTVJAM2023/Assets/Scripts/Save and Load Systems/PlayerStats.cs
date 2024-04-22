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
    public List<ModuleData> moduleData = new();
    public int credits = 100;
    public int shopLevelVisited = 0;

    // Game
    public int bossLevel = 0;

    // Skill Bord
    public List<bool> skillsSpotted = new();

}

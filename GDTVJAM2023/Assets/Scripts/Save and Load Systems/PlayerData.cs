using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

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
    private List<ModuleData>[] shipsModuleData;
    public List<ModuleData>[] ShipsModuleData { get => shipsModuleData; set => shipsModuleData = value; }
    private int activeShip;
    public int ActiveShip
    {
        get => activeShip;
        set
        {
            if (value < 0 || value > 3)
            {
                throw new ArgumentOutOfRangeException();
            }
            activeShip = value;
        }
    }

    public int credits;
    public int shopLevelVisited = 0;
    public ShipPreset defaultShip;

    // Game
    [Header("Game")]
    public int bossLevel = 0;

    // Skill board
    [Header("Skill Bord")]
    public List<bool> skillsSpotted = new();

    // Set PlayerData to Default
    public void Initialize(string savePath, int numModules, int numUpgrades)
    {
        playerName = "Player";
        this.savePath = savePath; //"playerData.json";
        playerShipIcon = 0;

        // Hangar and Shop
        credits = 0;
        bossLevel = 0;
        moduleCounts = new();
        ShipsModuleData = new List<ModuleData>[4];
        for (int i = 0; i < 4; i++)
        {
            ShipsModuleData[i] = defaultShip.baseModules;
        }
        skillsSpotted = new();
        shopLevelVisited = 0;

        // pre-fill module counts with zeros
        moduleCounts = new List<int>();
        for (int i = 0; i < numModules; i++)
        {
            moduleCounts.Add(0);
        }


        // pre-fill skill board with falses
        for (int i = 0; i < numUpgrades; i++)
        {
            skillsSpotted.Add(false);
        }
    }
    // Load Player Data
    public void LoadStats(PlayerStats data)
    {
        // PlayerProfil
        playerName = data.playerName;
        savePath = data.savePath;
        playerShipIcon = data.shipPanelIndex;

        // Hangar and Shop
        ShipsModuleData = data.shipsModuleData;
        moduleCounts.Clear();
        moduleCounts = data.moduleCounts;
        credits = data.credits;
        bossLevel = data.bossLevel;
        shopLevelVisited = data.shopLevelVisited;

        // Skillboard
        skillsSpotted.Clear();
        skillsSpotted = data.skillsSpotted;
    }

    public List<ModuleData> GetActiveShip()
    {
        #if UNITY_EDITOR
                if (ShipsModuleData == null)
                {
                    Debug.Log("ShipsModuleData cant be initialized  - PlayerData Load - the last player profile");
                    AudioManager.Instance.LoadPlayerData(savePath);
                    return null;
                }
        #endif

        return ShipsModuleData[ActiveShip];
    }

    public void SetActiveShip(List<ModuleData> ship)
    {
        ShipsModuleData[ActiveShip] = ship;
    }
}

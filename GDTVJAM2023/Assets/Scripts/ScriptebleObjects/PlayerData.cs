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
    [Header("Hangar")]
    public int playerShip = 0;
    public int playerShipCount = 1; //unlocking ships
    public int expBullet = 0;
    public int expRocket = 0;
    public int expLaser = 0;
    public int globalUpgradeCountBullet = 0;
    public int globalUpgradeCountRocket = 0;
    public int globalUpgradeCountLaser = 0;
    public List<int> bulletResearchedSkills;
    public List<int> rocketResearchedSkills;
    public List<int> laserResearchedSkills;
}

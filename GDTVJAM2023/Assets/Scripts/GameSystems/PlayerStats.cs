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

    // Hangar
    public int playerShip = 0;
    public int playerShipCount = 1; //unlocking ships
    public int expBullet = 0;
    public int expRocket = 0;
    public int expLaser = 0;
    public int globalUpgradeCountBullet = 0;
    public int globalUpgradeCountRocket = 0;
    public int globalUpgradeCountLaser = 0;

    public bool bulletRS_1 = false;
    public bool bulletRS_2 = false;
    public bool bulletRS_3 = false;
    public bool bulletRS_4 = false;
    public bool bulletRS_5 = false;
    public bool bulletRS_6 = false;
    public bool bulletRS_7 = false;
    public bool bulletRS_8 = false;
    public bool bulletRS_9 = false;
    public bool bulletRS_10 = false;

    public bool rocketRS_1 = false;
    public bool rocketRS_2 = false;
    public bool rocketRS_3 = false;
    public bool rocketRS_4 = false;
    public bool rocketRS_5 = false;
    public bool rocketRS_6 = false;
    public bool rocketRS_7 = false;
    public bool rocketRS_8 = false;
    public bool rocketRS_9 = false;
    public bool rocketRS_10 = false;

    public bool laserRS_1 = false;
    public bool laserRS_2 = false;
    public bool laserRS_3 = false;
    public bool laserRS_4 = false;
    public bool laserRS_5 = false;
    public bool laserRS_6 = false;
    public bool laserRS_7 = false;
    public bool laserRS_8 = false;
    public bool laserRS_9 = false;
    public bool laserRS_10 = false;
}

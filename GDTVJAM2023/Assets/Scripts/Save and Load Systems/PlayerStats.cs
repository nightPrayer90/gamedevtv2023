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

    public int bulletRS_1 = 0;
    public int bulletRS_2 = 0;
    public int bulletRS_3 = 0;
    public int bulletRS_4 = 0;
    public int bulletRS_5 = 0;
    public int bulletRS_6 = 0;
    public int bulletRS_7 = 0;
    public int bulletRS_8 = 0;
    public int bulletRS_9 = 0;
    public int bulletRS_10 = 0;

    public int rocketRS_1 = 0;
    public int rocketRS_2 = 0;
    public int rocketRS_3 = 0;
    public int rocketRS_4 = 0;
    public int rocketRS_5 = 0;
    public int rocketRS_6 = 0;
    public int rocketRS_7 = 0;
    public int rocketRS_8 = 0;
    public int rocketRS_9 = 0;
    public int rocketRS_10 = 0;

    public int laserRS_1 = 0;
    public int laserRS_2 = 0;
    public int laserRS_3 = 0;
    public int laserRS_4 = 0;
    public int laserRS_5 = 0;
    public int laserRS_6 = 0;
    public int laserRS_7 = 0;
    public int laserRS_8 = 0;
    public int laserRS_9 = 0;
    public int laserRS_10 = 0;
}

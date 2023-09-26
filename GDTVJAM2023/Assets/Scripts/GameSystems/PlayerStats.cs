using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerStats 
{
    // ** Important: this class is only temporay for save and load data *** 
    // *** PlayerStats -> for save and load -> convert to PlayerData for runtime ***

    // player profile
    public string playerName = "player";
    public int shipPanelIndex = 0;
    public string savePath = "/ player - stats01.json";

    // Hangar
    public int playerShip = 0;
    public int playerShipCount = 1; //unlocking ships
    public int expBullet = 0;
    public int expRocket = 0;
    public int expLaser = 0;
}

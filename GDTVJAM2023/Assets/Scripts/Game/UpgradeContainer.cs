using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class UpgradeContainer
{
    
    public enum MainClass : int
    {
        Nothing = 8,
        Bullet = 0,
        Explosion = 1,
        Laser = 2,
        Support = 3
    }

    public enum SubClass : int
    {
        Nothing = 8,
        Swarm = 4,
        Defence = 5,
        Targeting = 6,
        Direction = 7
    }

    public enum UpgradeTyp 
    {
        WeaponUpgrade,
        NormalUpgrade,
        ClassUpgrade,
        SubClassUpgrade,
        SpecialUpgrade
    }

    public string headerStr;
    public string descriptionStr;
    public Sprite iconPanel;
    public MainClass mainClass;
    public SubClass subClass;
    public int upgradeIndex;
    public int reqBullet;
    public int reqRocket;
    public int reqLaser;
    public int reqSupport;
    public UpgradeTyp upgradeTyp;
}

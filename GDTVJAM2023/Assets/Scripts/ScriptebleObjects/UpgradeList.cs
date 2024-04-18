using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct Upgrade
{
    public enum MainClass : int
    {
        Nothing = 8,
        Bullet = 0,
        Explosion = 1,
        Laser = 2,
        Support = 3
    }

    public enum UpgradeTyp
    {
        WeaponUpgrade,
        NormalUpgrade,
        ClassUpgrade,
        SpecialUpgrade,
        OverflowUpgrade
    }

    [Header("Description")]
    public string headerStr;
    public int upgradeIndex;
    public string passivUpgradeString;
    [Multiline] public string descriptionStr;
    public Sprite iconPanel;
    public MainClass mainClass;
    public UpgradeTyp upgradeTyp;
    public int colorIndex;

    [Header("Requierments")]
    public int reqBullet;
    public int reqRocket;
    public int reqLaser;
    public int reqSupport;
    public string reqAbility;

    [Header("Quantity")]
    public int UpgradeCount;
    public int upgradeStartCount;
}

[CreateAssetMenu(fileName = "UpgradeList", menuName = "Scriptable Objects/UpgradeList")]
public class UpgradeList : ScriptableObject
{
    public List<Upgrade> upgradeList = new List<Upgrade>();
}

using UnityEngine;


/*public enum UpgradeTyp
{
    WeaponUpgrade,
    NormalUpgrade,
    ClassUpgrade,
    SpecialUpgrade,
    OverflowUpgrade
}
*/

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

    public string headerStr;
    public string descriptionStr;
    public Sprite iconPanel;
    public MainClass mainClass;
    public int upgradeIndex;
    public int reqBullet;
    public int reqRocket;
    public int reqLaser;
    public int reqSupport;
    public string reqAbility;
    public int colorIndex;
    public Upgrade.UpgradeTyp upgradeTyp;
    public string passivUpgrades;
    public int UpgradeCount;
}

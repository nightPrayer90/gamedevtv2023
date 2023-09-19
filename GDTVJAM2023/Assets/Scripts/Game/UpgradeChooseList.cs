using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChooseList : MonoBehaviour
{
    [Header("Upgrade System")]
    public UpgradeContainer[] weaponUpgrades;
    public List<bool> weaponIndexInstalled = new List<bool>();

    [Header("Class level")]
    public int mcBulletLvl = 0;
    public int mcExplosionLvl = 0;
    public int mcLaserLvl = 0;
    public int mcSupportLvl = 0;
    public int scSwarmLvl = 0;
    public int scDefenceLvl = 0;
    public int scTargetingLvl = 0;
    public int scDirectionLvl = 0;

    [Header("Basic Ability Values")]
    public float percBulletDamage = 0;
    public int baseBulletCritChance = 0;
    public int baseBulletCritDamage = 100;
    public float percRocketDamage = 0;
    public float baseRocketAOERadius = 0;
    public int baseLaserBurnDamageChance = 0;
    public float percLaserDamage = 0;
    public int baseSupportRealoadTime = 0;
    public int baseLaserTicks = 4;
    public int baseLaserTickDamage = 5;
    public float baseAttackSpeed = 0;
    public float baseBoostInvulnerability = 0.5f;
    public float chanceToGetTwoExp = 0;
    public float chanceToGet1Health = 0;
    public float rocketLifeTime = 0;
    public int shieldHealth = 0;
    public int shieldDamage = 0;

    [Header("Class Upgrade Values")]
    public int critChance = 7;
    public int critDamage = 20;
    public float aoeRange = 20;
    public int burningChance = 2;
    public int supportRealodTime = 10;
    public int laserBurningTickDamangePercent = 100;

    public int normalUpgradeCount = 0;
    public int weaponUpgradeCount = 0;


    private void Start()
    {
        // this list was filled fom playerWeaponController
        for (int i = 0; i < weaponUpgrades.Length; i++)
        {
            weaponIndexInstalled.Add(false);
        }
        buildlistCountAfterUpdate();
    }

    public List<int> BuildUpgradeList(UpgradeTyp upgradeTyp)
    {
        List<int> buildList = new List<int>();

        for( int i=0; i < weaponUpgrades.Length; i++ )
        {
            if (weaponUpgrades[i].upgradeTyp == upgradeTyp && weaponIndexInstalled[i] == false &&
               mcBulletLvl >= weaponUpgrades[i].reqBullet && mcExplosionLvl >= weaponUpgrades[i].reqRocket && mcLaserLvl >= weaponUpgrades[i].reqLaser && mcSupportLvl >= weaponUpgrades[i].reqSupport )   
            {
                buildList.Add(i);
            }
        }

        if (upgradeTyp == UpgradeTyp.NormalUpgrade) normalUpgradeCount = buildList.Count;
        if (upgradeTyp == UpgradeTyp.WeaponUpgrade) weaponUpgradeCount = buildList.Count;

        Debug.Log("buildListcount befor update " + buildList.Count);
        return buildList;
    }


    public string buildlistCountAfterUpdate()
    {
        // save old values
        int oldNC = normalUpgradeCount;
        int oldWC = weaponUpgradeCount;

        // reset
        normalUpgradeCount = 0;
        weaponUpgradeCount = 0;

        // count new
        for (int i = 0; i < weaponUpgrades.Length; i++)
        {
            if (weaponIndexInstalled[i] == false &&
               mcBulletLvl >= weaponUpgrades[i].reqBullet && mcExplosionLvl >= weaponUpgrades[i].reqRocket && mcLaserLvl >= weaponUpgrades[i].reqLaser && mcSupportLvl >= weaponUpgrades[i].reqSupport)
            {
                if (weaponUpgrades[i].upgradeTyp == UpgradeTyp.NormalUpgrade)
                {
                    normalUpgradeCount++;
                }
                if (weaponUpgrades[i].upgradeTyp == UpgradeTyp.WeaponUpgrade)
                {
                    weaponUpgradeCount++;
                }
            }
        }

        // build the new values
        string returnString = " ";
        
        if (oldNC < normalUpgradeCount)
        {
            returnString = "You unlocked " + (normalUpgradeCount - oldNC).ToString() + " new upgrade(s).";
        }
        if (oldWC < weaponUpgradeCount)
        {
            returnString += " You unlocked " + (weaponUpgradeCount - oldWC).ToString() + " new weapon upgrade(s).";
        }

        return returnString;
    }
}

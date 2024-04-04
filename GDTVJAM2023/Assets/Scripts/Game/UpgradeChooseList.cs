using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChooseList : MonoBehaviour
{
    [Header("Upgrade System")]
    public UpgradeContainer[] weaponUpgrades;
    public List<int> weaponIndexInstalled = new List<int>();
    public PlayerWeaponController playerWeaponController;

    [Header("Class level")]
    public int mcBulletLvl = 0;
    public int mcExplosionLvl = 0;
    public int mcLaserLvl = 0;
    public int mcSupportLvl = 0;
 

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
    public float chanceToGetfullEnergy = 0;
    public float rocketLifeTime = 0;
    public int shieldHealth = 0;
    public int shieldDamage = 0;
    public int bossBonusDamage = 0;

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
            weaponIndexInstalled.Add(0);
        }
        BuildlistCountAfterUpdate();

        // Update Weaponcontroller after loading all modules
        Invoke("UpdatePlayerWeaponController", 1f);
    }

    private void UpdatePlayerWeaponController()
    {
        baseBulletCritChance += critChance * mcBulletLvl;
        baseBulletCritDamage += critDamage * mcBulletLvl;
        baseRocketAOERadius += aoeRange * mcExplosionLvl;
        baseLaserBurnDamageChance += burningChance * mcLaserLvl;
        baseSupportRealoadTime = mcSupportLvl;

        playerWeaponController.UpdateWeaponValues();
    }

    public List<int> BuildUpgradeList(UpgradeTyp upgradeTyp)
    {
        List<int> buildList = new List<int>();

        for( int i=0; i < weaponUpgrades.Length; i++ )
        {
            if (weaponUpgrades[i].upgradeTyp == upgradeTyp && weaponIndexInstalled[i] < weaponUpgrades[i].UpgradeCount &&
               mcBulletLvl >= weaponUpgrades[i].reqBullet && mcExplosionLvl >= weaponUpgrades[i].reqRocket && mcLaserLvl >= weaponUpgrades[i].reqLaser && mcSupportLvl >= weaponUpgrades[i].reqSupport )   
            {
                buildList.Add(i);
            }
        }

        //Debug.Log("buildListcount" + buildList.Count);

        // Overflow
        if (buildList.Count < 3)
        {
            buildList.Add(0);
        }

        if (buildList.Count < 3)
        {
            buildList.Add(1);
        }

        if (buildList.Count < 3)
        {
            buildList.Add(2);
        }

        if (upgradeTyp == UpgradeTyp.NormalUpgrade) normalUpgradeCount = buildList.Count;
        if (upgradeTyp == UpgradeTyp.WeaponUpgrade) weaponUpgradeCount = buildList.Count;

        //Debug.Log("buildListcount befor update " + buildList.Count);
        return buildList;
    }


    public string BuildlistCountAfterUpdate()
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
            if (weaponIndexInstalled[i] < weaponUpgrades[i].UpgradeCount &&
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

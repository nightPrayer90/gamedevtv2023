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
    public int baseBulletCritChance = 0;
    public int baseBulletCritDamage = 100;
    public float baseRocketAOERadius = 0;
    public int baseLaserBurnDamageChance = 0;
    public int baseSupportRealoadTime = 0;
    public int baseLaserTicks = 4;
    public int baseLaserTickDamage = 5;
    public float baseAttackSpeed = 0;
    public float baseBoostInvulnerability = 0.5f;
    public float chanceToGetTwoExp = 0;
    public float chanceToGet1Health = 0;
    public float rocketLifeTime = 0;

    [Header("Class Upgrade Values")]
    public int critChance = 7;
    public int critDamage = 20;
    public float aoeRange = 20;
    public int buringChance = 2;
    public int supportRealodTime = 10;
    public int laserBurningTickDamancePercent = 100;
    

    private void Start()
    {
        // this list was filled fom playerWeaponController
        for (int i = 0; i < weaponUpgrades.Length; i++)
        {
            weaponIndexInstalled.Add(false);
        }
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

        //Debug.Log(buildList.Count);
        return buildList;
    }
}

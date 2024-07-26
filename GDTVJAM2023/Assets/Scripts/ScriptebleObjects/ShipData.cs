using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "ShipData", menuName = "Scriptable Objects/ShipData")]
public class ShipData : ScriptableObject
{
    [Header("Class level")]
    public int mcBulletLvl = 0;
    public int mcExplosionLvl = 0;
    public int mcLaserLvl = 0;
    public int mcSupportLvl = 0;


    [Header("Start Values")]
    public int baseLaserTicks = 4;
    public int baseLaserTickDamage = 5;
    public int laserBurningTickDamangePercent = 100;
    public float boostInvulnerability = 0.5f;

    [Header("Basic Ability Values")]
    public float percBulletDamage = 0;
    public float percRocketDamage = 0;
    public float percLaserDamage = 0;
    
    public float chanceToGetTwoExp = 0;
    public float chanceToGetHealth = 0;
    public float chanceToGetFullEnergy = 0;
    public float rocketLifeTime = 0;

    public int shieldHealth = 0;
    public int shieldDamage = 3;
    public int bossBonusDamage = 0;

    public float supportReloadTime = 0;

    

    [Header("Weapon buffs")]
    public int bulletCritChance = 0;
    public int bulletCritDamage = 0;
    public int burnDamageChance = 0;
    public float rocketAOERadius = 0;


    [Header("Const Class Upgrade Values")]
    public int critChance = 7;
    public int critDamage = 20;
    public float aoeRange = 20;
    public int burningChance = 2;
    public int realodTime = 10;


    //public int baseBulletCritChance = 0;
    //public int baseBulletCritDamage = 100;
    //public int baseLaserBurnDamageChance = 0;
    //public float baseRocketAOERadius = 0;
    //public int baseSupportRealoadTime = 0;

}

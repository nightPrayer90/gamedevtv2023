using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class DefaultShipStats 
{
    // ** This Serializable Object is to save all default values of the player Ships *** 
    public string shipName = "";

    //player controller
    public float speed = 11f;
    public float rotateSpeed = 3.5f;
    public int health = 10;
    public int baseDamage = 5;
    public float fireRate = 0.4f;
    public float pickupRange = 0.8f;
    public float boostPower = 4f;
    public float boostDuration = 2f;
    public int protectionLevel = 0;

    // weapon controller
    public int bulletClass = 0;
    public int explosionClass = 0;
    public int laserClass = 0;
    public int supportClass = 0;

    // upgrade stuff
    public int moreUpgradePoints = 0;
    public int chanceToDoubleExp = 0;
    public int pecBulletDamage = 0;
    public int pecExplosionDamage = 0;
    public int pecLaserDamage = 0;
    public int pecCritChance = 0;
    public int pecAOE = 0;
    public int pecBurningChance = 0;
    public int pecBulletCritDamage = 0;
    public float rocketLifeTime = 0f;
    public int burningDamagePerTick = 0;
    public int pecMainWeaponDamageBoss = 0;
    public float laserShootingTime = 0;
}   

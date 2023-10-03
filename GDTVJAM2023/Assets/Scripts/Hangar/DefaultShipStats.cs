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
    public float PickupRange = 0.8f;
    public float boostPower = 4f;
    public float boostDuration = 2f;
    public int protectionLevel = 0;

    // main weapon controller
    public float rocketLifeTime = 2f;
    public float laserShooingTime = 3f;

    // weapon controller
    public int bulletClass = 0;
    public int explosionClass = 0;
    public int laserClass = 0;
    public int supportClass = 0;
}

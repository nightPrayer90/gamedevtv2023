using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HangarManager : MonoBehaviour
{
    public GameObject playerStartPoint;
    public GameObject playerHangarShip;
    public List<Color> classColors;
    public HangarShipDefaultValues defalutValues;
    public ShipData bulletShipData;
    public ShipData rocketShipData;
    public ShipData laserShipData;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 positionOffset = new Vector3(0f, 0.05f, 0f);
        playerHangarShip.transform.position = playerStartPoint.transform.position + positionOffset;

        // Build Ship Values
        BuildShipValues();
    }

    // Build ship values - default ship Stats + Upgrades = shipData Object
    private void BuildShipValues()
    {
        // bullet ship -------------
        bulletShipData.speed = defalutValues.shipStats[0].speed;
        bulletShipData.rotateSpeed = defalutValues.shipStats[0].rotateSpeed;
        bulletShipData.health = defalutValues.shipStats[0].health;
        bulletShipData.baseDamage = defalutValues.shipStats[0].baseDamage;
        bulletShipData.fireRate = defalutValues.shipStats[0].fireRate;
        bulletShipData.PickupRange = defalutValues.shipStats[0].PickupRange;
        bulletShipData.boostPower = defalutValues.shipStats[0].boostPower;
        bulletShipData.boostDuration = defalutValues.shipStats[0].boostDuration;
        bulletShipData.protectionLevel = defalutValues.shipStats[0].protectionLevel;
        bulletShipData.rocketLifeTime = defalutValues.shipStats[0].rocketLifeTime;
        bulletShipData.laserShooingTime = defalutValues.shipStats[0].laserShooingTime;
        bulletShipData.bulletClass = defalutValues.shipStats[0].bulletClass;
        bulletShipData.explosionClass = defalutValues.shipStats[0].explosionClass;
        bulletShipData.laserClass = defalutValues.shipStats[0].laserClass;
        bulletShipData.supportClass = defalutValues.shipStats[0].supportClass;


        // rocket ship -------------
        rocketShipData.speed = defalutValues.shipStats[1].speed;
        rocketShipData.rotateSpeed = defalutValues.shipStats[1].rotateSpeed;
        rocketShipData.health = defalutValues.shipStats[1].health;
        rocketShipData.baseDamage = defalutValues.shipStats[1].baseDamage;
        rocketShipData.fireRate = defalutValues.shipStats[1].fireRate;
        rocketShipData.PickupRange = defalutValues.shipStats[1].PickupRange;
        rocketShipData.boostPower = defalutValues.shipStats[1].boostPower;
        rocketShipData.boostDuration = defalutValues.shipStats[1].boostDuration;
        rocketShipData.protectionLevel = defalutValues.shipStats[1].protectionLevel;
        rocketShipData.rocketLifeTime = defalutValues.shipStats[1].rocketLifeTime;
        rocketShipData.laserShooingTime = defalutValues.shipStats[1].laserShooingTime;
        rocketShipData.bulletClass = defalutValues.shipStats[1].bulletClass;
        rocketShipData.explosionClass = defalutValues.shipStats[1].explosionClass;
        rocketShipData.laserClass = defalutValues.shipStats[1].laserClass;
        rocketShipData.supportClass = defalutValues.shipStats[1].supportClass;


        // laser ship --------------
        laserShipData.speed = defalutValues.shipStats[2].speed;
        laserShipData.rotateSpeed = defalutValues.shipStats[2].rotateSpeed;
        laserShipData.health = defalutValues.shipStats[2].health;
        laserShipData.baseDamage = defalutValues.shipStats[2].baseDamage;
        laserShipData.fireRate = defalutValues.shipStats[2].fireRate;
        laserShipData.PickupRange = defalutValues.shipStats[2].PickupRange;
        laserShipData.boostPower = defalutValues.shipStats[2].boostPower;
        laserShipData.boostDuration = defalutValues.shipStats[2].boostDuration;
        laserShipData.protectionLevel = defalutValues.shipStats[2].protectionLevel;
        laserShipData.rocketLifeTime = defalutValues.shipStats[2].rocketLifeTime;
        laserShipData.laserShooingTime = defalutValues.shipStats[2].laserShooingTime;
        laserShipData.bulletClass = defalutValues.shipStats[2].bulletClass;
        laserShipData.explosionClass = defalutValues.shipStats[2].explosionClass;
        laserShipData.laserClass = defalutValues.shipStats[2].laserClass;
        laserShipData.supportClass = defalutValues.shipStats[2].supportClass;

    }

}

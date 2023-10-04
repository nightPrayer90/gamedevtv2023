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
    public HangerUIController hangarUIController;
    public PlayerData playerData;

    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */

    // Start is called before the first frame update
    void Start()
    {
        Vector3 positionOffset = new Vector3(0f, 0.05f, 0f);
        playerHangarShip.transform.position = playerStartPoint.transform.position + positionOffset;

        // Build Ship Values
        SetShipDataValues(0);
        SetShipDataValues(1);
        SetShipDataValues(2);
    }

    // set shipData values - default ship Stats + Upgrades = shipData Object
    public void SetShipDataValues(int shipIndex)
    {
        switch (shipIndex)
        {
            case 0: // bullet ship -------------
                bulletShipData.speed = defalutValues.shipStats[shipIndex].speed  ;
                bulletShipData.rotateSpeed = defalutValues.shipStats[shipIndex].rotateSpeed;
                bulletShipData.health = defalutValues.shipStats[shipIndex].health;
                bulletShipData.baseDamage = defalutValues.shipStats[shipIndex].baseDamage;
                bulletShipData.fireRate = defalutValues.shipStats[shipIndex].fireRate + (defalutValues.shipStats[shipIndex].fireRate * (playerData.bulletResearchedSkills[9] * 0.07f));
                bulletShipData.pickupRange = defalutValues.shipStats[shipIndex].pickupRange + (playerData.bulletResearchedSkills[3] * 0.2f);
                bulletShipData.boostPower = defalutValues.shipStats[shipIndex].boostPower + (defalutValues.shipStats[shipIndex].boostPower * (playerData.bulletResearchedSkills[6] * 0.07f));
                bulletShipData.boostDuration = defalutValues.shipStats[shipIndex].boostDuration;
                bulletShipData.protectionLevel = defalutValues.shipStats[shipIndex].protectionLevel + (playerData.bulletResearchedSkills[4] * 1);

                bulletShipData.bulletClass = defalutValues.shipStats[shipIndex].bulletClass + playerData.bulletResearchedSkills[0];
                bulletShipData.explosionClass = defalutValues.shipStats[shipIndex].explosionClass;
                bulletShipData.laserClass = defalutValues.shipStats[shipIndex].laserClass;
                bulletShipData.supportClass = defalutValues.shipStats[shipIndex].supportClass;

                bulletShipData.moreUpgradePoints = defalutValues.shipStats[shipIndex].moreUpgradePoints + (playerData.bulletResearchedSkills[1] * 2);
                bulletShipData.chanceToDoubleExp = defalutValues.shipStats[shipIndex].chanceToDoubleExp + (playerData.bulletResearchedSkills[2] * 3);
                bulletShipData.pecBulletDamage = defalutValues.shipStats[shipIndex].pecBulletDamage + (playerData.bulletResearchedSkills[5] * 3);
                bulletShipData.pecExplosionDamage = defalutValues.shipStats[shipIndex].pecExplosionDamage;
                bulletShipData.pecLaserDamage = defalutValues.shipStats[shipIndex].pecLaserDamage;
                bulletShipData.pecCritChance = defalutValues.shipStats[shipIndex].pecCritChance + (playerData.bulletResearchedSkills[7] * 1); ;
                bulletShipData.pecAOE = defalutValues.shipStats[shipIndex].pecAOE;
                bulletShipData.pecBurningChance = defalutValues.shipStats[shipIndex].pecBurningChance;
                bulletShipData.pecBulletCritDamage = defalutValues.shipStats[shipIndex].pecBulletCritDamage + (playerData.bulletResearchedSkills[8] * 10);
                bulletShipData.rocketLifeTime = defalutValues.shipStats[shipIndex].rocketLifeTime;
                bulletShipData.burningDamagePerTick = defalutValues.shipStats[shipIndex].burningDamagePerTick;
                bulletShipData.pecMainWeaponDamageBoss = defalutValues.shipStats[shipIndex].pecMainWeaponDamageBoss;
                bulletShipData.laserShootingTime = defalutValues.shipStats[shipIndex].laserShootingTime;
                break;

            case 1: // rocket ship -------------
                rocketShipData.speed = defalutValues.shipStats[shipIndex].speed;
                rocketShipData.rotateSpeed = defalutValues.shipStats[shipIndex].rotateSpeed;
                rocketShipData.health = defalutValues.shipStats[shipIndex].health;
                rocketShipData.baseDamage = defalutValues.shipStats[shipIndex].baseDamage;
                rocketShipData.fireRate = defalutValues.shipStats[shipIndex].fireRate;
                rocketShipData.pickupRange = defalutValues.shipStats[shipIndex].pickupRange;
                rocketShipData.boostPower = defalutValues.shipStats[shipIndex].boostPower;
                rocketShipData.boostDuration = defalutValues.shipStats[shipIndex].boostDuration;
                rocketShipData.protectionLevel = defalutValues.shipStats[shipIndex].protectionLevel;

                rocketShipData.bulletClass = defalutValues.shipStats[shipIndex].bulletClass;
                rocketShipData.explosionClass = defalutValues.shipStats[shipIndex].explosionClass;
                rocketShipData.laserClass = defalutValues.shipStats[shipIndex].laserClass;
                rocketShipData.supportClass = defalutValues.shipStats[shipIndex].supportClass;

                rocketShipData.moreUpgradePoints = defalutValues.shipStats[shipIndex].moreUpgradePoints;
                rocketShipData.chanceToDoubleExp = defalutValues.shipStats[shipIndex].chanceToDoubleExp;
                rocketShipData.pecBulletDamage = defalutValues.shipStats[shipIndex].pecBulletDamage;
                rocketShipData.pecExplosionDamage = defalutValues.shipStats[shipIndex].pecExplosionDamage;
                rocketShipData.pecLaserDamage = defalutValues.shipStats[shipIndex].pecLaserDamage;
                rocketShipData.pecCritChance = defalutValues.shipStats[shipIndex].pecCritChance;
                rocketShipData.pecAOE = defalutValues.shipStats[shipIndex].pecAOE;
                rocketShipData.pecBurningChance = defalutValues.shipStats[shipIndex].pecBurningChance;
                rocketShipData.pecBulletCritDamage = defalutValues.shipStats[shipIndex].pecBulletCritDamage;
                rocketShipData.rocketLifeTime = defalutValues.shipStats[shipIndex].rocketLifeTime;
                rocketShipData.burningDamagePerTick = defalutValues.shipStats[shipIndex].burningDamagePerTick;
                rocketShipData.pecMainWeaponDamageBoss = defalutValues.shipStats[shipIndex].pecMainWeaponDamageBoss;
                rocketShipData.laserShootingTime = defalutValues.shipStats[shipIndex].laserShootingTime;
                break;

            case 2: // laser ship --------------
                laserShipData.speed = defalutValues.shipStats[shipIndex].speed;
                laserShipData.rotateSpeed = defalutValues.shipStats[shipIndex].rotateSpeed;
                laserShipData.health = defalutValues.shipStats[shipIndex].health;
                laserShipData.baseDamage = defalutValues.shipStats[shipIndex].baseDamage;
                laserShipData.fireRate = defalutValues.shipStats[shipIndex].fireRate;
                laserShipData.pickupRange = defalutValues.shipStats[shipIndex].pickupRange;
                laserShipData.boostPower = defalutValues.shipStats[shipIndex].boostPower;
                laserShipData.boostDuration = defalutValues.shipStats[shipIndex].boostDuration;
                laserShipData.protectionLevel = defalutValues.shipStats[shipIndex].protectionLevel;

                laserShipData.bulletClass = defalutValues.shipStats[shipIndex].bulletClass;
                laserShipData.explosionClass = defalutValues.shipStats[shipIndex].explosionClass;
                laserShipData.laserClass = defalutValues.shipStats[shipIndex].laserClass;
                laserShipData.supportClass = defalutValues.shipStats[shipIndex].supportClass;

                laserShipData.moreUpgradePoints = defalutValues.shipStats[shipIndex].moreUpgradePoints;
                laserShipData.chanceToDoubleExp = defalutValues.shipStats[shipIndex].chanceToDoubleExp;
                laserShipData.pecBulletDamage = defalutValues.shipStats[shipIndex].pecBulletDamage;
                laserShipData.pecExplosionDamage = defalutValues.shipStats[shipIndex].pecExplosionDamage;
                laserShipData.pecLaserDamage = defalutValues.shipStats[shipIndex].pecLaserDamage;
                laserShipData.pecCritChance = defalutValues.shipStats[shipIndex].pecCritChance;
                laserShipData.pecAOE = defalutValues.shipStats[shipIndex].pecAOE;
                laserShipData.pecBurningChance = defalutValues.shipStats[shipIndex].pecBurningChance;
                laserShipData.pecBulletCritDamage = defalutValues.shipStats[shipIndex].pecBulletCritDamage;
                laserShipData.rocketLifeTime = defalutValues.shipStats[shipIndex].rocketLifeTime;
                laserShipData.burningDamagePerTick = defalutValues.shipStats[shipIndex].burningDamagePerTick;
                laserShipData.pecMainWeaponDamageBoss = defalutValues.shipStats[shipIndex].pecMainWeaponDamageBoss;
                laserShipData.laserShootingTime = defalutValues.shipStats[shipIndex].laserShootingTime;
                break;

        }

        // TODO Upgrade UI VALUES
        hangarUIController.UpdateShipUIValues(shipIndex);
    }
}

using UnityEngine;
using static ModuleValues;

public class BaseModule : MonoBehaviour
{
    public ModuleValues moduleValues;
    public int moduleIndex = -1;
   
    protected void UpdateModuleValues()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");

        if (go != null)
        {
            NewPlayerController playerController = go.GetComponent<NewPlayerController>();
            PlayerWeaponController playerWeaponController = playerController.playerWeaponController;
            Rigidbody playerRigidbody = playerController.playerRigidbody;

            playerRigidbody.mass += moduleValues.costMass;

            playerController.energieProduction += moduleValues.energieProduction;
            playerController.energieMax += moduleValues.energieStorage;

            playerController.playerMaxHealth += moduleValues.health;
            playerController.playerCurrentHealth += moduleValues.health;

            playerController.protectionLvl += (int)moduleValues.protection; //ToDo - float in ModuleValues to int!

            // Change the center of Mass
            //playerRigidbody.centerOfMass = playerRigidbody.centerOfMass + new Vector3((transform.localPosition.x / 2) * mass, 0f, (transform.localPosition.z / 2) * mass);
            
            playerWeaponController.shipData.mcBulletLvl += moduleValues.bulletClass;
            playerWeaponController.shipData.mcExplosionLvl += moduleValues.rocketClass;
            playerWeaponController.shipData.mcLaserLvl += moduleValues.laserClass;
            playerWeaponController.shipData.mcSupportLvl += moduleValues.supportClass;

            if (moduleValues.moduleUpgrades.Count > 0)
            {
                UpgradeChooseList upgradeChooseList = playerController.upgradeChooseList;

                foreach (ModuleUpgrade item in moduleValues.moduleUpgrades)
                {
                    upgradeChooseList.upgrades[item.moduleUpgradeIndex].upgradeStartCount += item.moduleUpgradeQuantity;
                }
            }
        }
    }
}

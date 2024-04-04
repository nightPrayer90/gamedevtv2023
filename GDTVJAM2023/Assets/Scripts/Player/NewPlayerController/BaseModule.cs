using UnityEngine;

public class BaseModule : MonoBehaviour
{
    public ModuleValues moduleValues;
   
    protected void UpdateModuleValues()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        GameObject gm = GameObject.Find("Game Manager");

        if (go != null)
        {
            NewPlayerController playerController = go.GetComponent<NewPlayerController>();
            Rigidbody playerRigidbody = playerController.GetComponent<Rigidbody>();
            playerRigidbody.mass += moduleValues.costMass;

            playerController.energieProduction += moduleValues.energieProduction;
            playerController.energieProduction -= moduleValues.costEnergie;
            playerController.energieMax += moduleValues.energieStorage;

            playerController.playerMaxHealth += moduleValues.health;
            playerController.playerCurrentHealth += moduleValues.health;

            playerController.protectionLvl += (int)moduleValues.protection; //ToDo - float in ModuleValues to int!

            // Change the center of Mass
            //playerRigidbody.centerOfMass = playerRigidbody.centerOfMass + new Vector3((transform.localPosition.x / 2) * mass, 0f, (transform.localPosition.z / 2) * mass);
        }

        if (gm != null)
        {
            UpgradeChooseList upgradeChooseList = gm.GetComponent<UpgradeChooseList>();
            upgradeChooseList.mcBulletLvl = moduleValues.bulletClass;
            upgradeChooseList.mcExplosionLvl = moduleValues.rocketClass;
            upgradeChooseList.mcLaserLvl = moduleValues.laserClass;
            upgradeChooseList.mcSupportLvl = moduleValues.supportClass;
        }
    }
}

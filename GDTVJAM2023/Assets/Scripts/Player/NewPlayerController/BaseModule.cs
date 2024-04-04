using UnityEngine;

public class BaseModule : MonoBehaviour
{
    public ModuleValues moduleVales;
    private NewPlayerController playerController;
    private Rigidbody playerRigidbody;
    private UpgradeChooseList upgradeChooseList;

    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        GameObject gm = GameObject.Find("Game Manager");

        if (go != null)
        {
            playerController = go.GetComponent<NewPlayerController>();
            playerRigidbody = playerController.GetComponent<Rigidbody>();
            playerRigidbody.mass += moduleVales.costMass;

            playerController.energieProduction += moduleVales.energieProduction;
            playerController.energieProduction -= moduleVales.costEnergie;
            playerController.energieMax += moduleVales.energieStorage;

            playerController.playerMaxHealth += moduleVales.health;
            playerController.playerCurrentHealth += moduleVales.health;

            playerController.protectionLvl += (int)moduleVales.protection; //ToDo - float in ModuleValues to int!

            // Change the center of Mass
            //playerRigidbody.centerOfMass = playerRigidbody.centerOfMass + new Vector3((transform.localPosition.x / 2) * mass, 0f, (transform.localPosition.z / 2) * mass);
        }

        if (gm != null)
        {
            upgradeChooseList = gm.GetComponent<UpgradeChooseList>();
            upgradeChooseList.mcBulletLvl = moduleVales.bulletClass;
            upgradeChooseList.mcExplosionLvl = moduleVales.rocketClass;
            upgradeChooseList.mcLaserLvl = moduleVales.laserClass;
            upgradeChooseList.mcSupportLvl = moduleVales.supportClass;
        }
    }
}

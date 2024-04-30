using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeQuanityObserver
{
    public int upgradeIndexInstalled;
    public int upgradeStartCount;
}

public class UpgradeChooseList : MonoBehaviour
{
    
    [Header("Upgrade System")]
    public UpgradeList uLObject;
    public List<UpgradeQuanityObserver> upgrades = new List<UpgradeQuanityObserver>();
    
    //public UpgradeContainer[] weaponUpgrades; //TODO DELETE

    public PlayerWeaponController playerWeaponController;
    public NewPlayerController playerController;

    private int normalUpgradeCount = 0;
    private int weaponUpgradeCount = 0; 

    private void Start()
    {
        // this list was filled fom playerWeaponController
        for (int i = 0; i < uLObject.upgradeList.Count; i++)
        {
            UpgradeQuanityObserver aUpdate = new();
            aUpdate.upgradeIndexInstalled = 0;
            aUpdate.upgradeStartCount = uLObject.upgradeList[i].upgradeStartCount;
            upgrades.Add(aUpdate);
        }
        
        BuildlistCountAfterUpdate();
    }

 
    public List<int> BuildUpgradeList(Upgrade.UpgradeTyp upgradeTyp)
    {
        List<int> buildList = new List<int>();

        for (int i = 0; i < uLObject.upgradeList.Count; i++)
        {
            if (uLObject.upgradeList[i].upgradeTyp == upgradeTyp && upgrades[i].upgradeIndexInstalled < upgrades[i].upgradeStartCount &&
               playerWeaponController.shipData.mcBulletLvl >= uLObject.upgradeList[i].reqBullet && playerWeaponController.shipData.mcExplosionLvl >= uLObject.upgradeList[i].reqRocket && playerWeaponController.shipData.mcLaserLvl >= uLObject.upgradeList[i].reqLaser && playerWeaponController.shipData.mcSupportLvl >= uLObject.upgradeList[i].reqSupport)
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

        if (upgradeTyp == Upgrade.UpgradeTyp.NormalUpgrade) normalUpgradeCount = buildList.Count;
        if (upgradeTyp == Upgrade.UpgradeTyp.WeaponUpgrade) weaponUpgradeCount = buildList.Count;

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
        for (int i = 0; i < uLObject.upgradeList.Count; i++)
        {
            if (upgrades[i].upgradeIndexInstalled < upgrades[i].upgradeStartCount &&
               playerWeaponController.shipData.mcBulletLvl >= uLObject.upgradeList[i].reqBullet && playerWeaponController.shipData.mcExplosionLvl >= uLObject.upgradeList[i].reqRocket && playerWeaponController.shipData.mcLaserLvl >= uLObject.upgradeList[i].reqLaser && playerWeaponController.shipData.mcSupportLvl >= uLObject.upgradeList[i].reqSupport)
            {
                if (uLObject.upgradeList[i].upgradeTyp == Upgrade.UpgradeTyp.NormalUpgrade)
                {
                    normalUpgradeCount++;
                }
                if (uLObject.upgradeList[i].upgradeTyp == Upgrade.UpgradeTyp.WeaponUpgrade)
                {
                    weaponUpgradeCount++;
                }
            }
        }

        // build the new values
        string returnString = "";

        if (oldNC < normalUpgradeCount)
        {
            returnString = "You unlocked " + (normalUpgradeCount - oldNC).ToString() + " new upgrade(s).";
        }
        if (oldWC < weaponUpgradeCount)
        {
            returnString += " You unlocked " + (weaponUpgradeCount - oldWC).ToString() + " new weapon upgrade(s).";
        }

        // Maybe TODO - Skip the first massage
        if (playerController.playerLevel <= 2) returnString = "";
        if (returnString != "") AudioManager.Instance.PlaySFX("GetNewUpgrades");

        return returnString;
    }
}

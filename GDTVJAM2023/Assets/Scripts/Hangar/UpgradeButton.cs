using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [Header("Button")]
    public PlayerData playerData;
    public int upgradePanelIndex; // 0=bullet; 1=rocket; 2=laser
    public int upgradeButtonIndex;
    public int Colorindex;
    public Image btnImage;
    public HangarManager gameManager;
    public HangerUIController hangerUIController;

    [Header("Requirements")]
    public int otherUpgrades;
    private int otherUpgradesValue;
    public int upgradePoint;
    private int upgradePointValue;

    [Header("Button Description")]
    public string tooltipHeaderString;

    //privates
    private bool isComplete = false;
    private TooltipTrigger tooltipTrigger;
    private bool canUpgrade = false;


    private void OnEnable()
    {
        tooltipTrigger = gameObject.GetComponent<TooltipTrigger>();
        tooltipTrigger.header = tooltipHeaderString;

        RequirementSet();

        if (isComplete == true)
        {
            btnImage.color = gameManager.classColors[Colorindex];
            canUpgrade = false;
            tooltipTrigger.content = "";
        }
        else
        {
            if (otherUpgradesValue >= otherUpgrades)
            {
                if (upgradePointValue >= upgradePoint)
                {
                    btnImage.color = Color.white;
                    canUpgrade = true;
                    tooltipTrigger.content = "cost " + upgradePoint + " upgrade points";
                }
                else
                {
                    btnImage.color = Color.gray;
                    canUpgrade = false;
                    tooltipTrigger.content = "cost " + upgradePoint + " upgrade points";
                }
            }
            else
            {
                btnImage.color = Color.gray;
                canUpgrade = false;
                tooltipTrigger.content = "need " + (otherUpgrades - otherUpgradesValue).ToString() + " other upgrades";
            }
        }
    }

    

    public void UpgradeAbility()
    {
        if (canUpgrade == true)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");
            isComplete = true;
            btnImage.color = gameManager.classColors[Colorindex];
            UpgradeControl();
        }
        else
        {
            AudioManager.Instance.PlaySFX("MouseNo");
        }
    }

    public void UpgradeControl()
    {
        switch (upgradePanelIndex)
        {
            case 0: // bullet
                playerData.expBullet -= upgradePoint;
                hangerUIController.SetUpgradePointText(playerData.expBullet);
                playerData.globalUpgradeCountBullet += 1;
                break;

            case 1: // rocket
                playerData.expRocket -= upgradePoint;
                hangerUIController.SetUpgradePointText(playerData.expRocket);
                playerData.globalUpgradeCountRocket += 1;
                break;

            case 2: // laser
                playerData.expLaser -= upgradePoint;
                hangerUIController.SetUpgradePointText(playerData.expLaser);
                playerData.globalUpgradeCountLaser += 1;
                break;
        }

        RequirementSet();


    }

    public void RequirementSet()
    {
        switch (upgradePanelIndex)
        {
            case 0: // bullet
                otherUpgradesValue = playerData.globalUpgradeCountBullet;
                upgradePointValue = playerData.expBullet;
                break;

            case 1: // rocket
                otherUpgradesValue = playerData.globalUpgradeCountRocket;
                upgradePointValue = playerData.expRocket;
                break;

            case 2: // laser
                otherUpgradesValue = playerData.globalUpgradeCountLaser;
                upgradePointValue = playerData.expLaser;
                break;
        }
    }
}

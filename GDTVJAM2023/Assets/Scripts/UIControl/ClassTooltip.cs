using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


[System.Serializable]
public class ClassTooltipC
{
    public string header;
    [Multiline]
    public string content;
}


public class ClassTooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWarpLimit;
    private Vector2 triggerPosition_;
    public CanvasGroup cg;

    public ClassTooltipC[] tooltipContent;

    public Color contentTextValueenabled;
    public Color contentTextValuenull;
    public UpgradeChooseList upgradeChooseList;
    public GameManager gameManager;

    private GameObject player;
    private NewPlayerController playerController;
    private PlayerWeaponController playerWeaponController;

    void OnEnable()
    {
        if (triggerPosition_ != null)
        {
            transform.position = triggerPosition_; // + new Vector2(0f, 20f);
        }
        // FadeIn
        cg.alpha = 0;
        cg.DOFade(1f, 0.1f).SetUpdate(true);
    }


    public void Show(Vector2 triggerPosition, int contentType)
    {
        //Debug.Log("contentType " + contentType);

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<NewPlayerController>();
            playerWeaponController = player.GetComponent<PlayerWeaponController>();
        }

        // set tooltip positon
        triggerPosition_ = triggerPosition;

        // set tooltip content
        string content_ = "";

        switch (contentType)
        {
            case 0: // bullet
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.cCPrefab.classColor[contentType];
                if (playerWeaponController.shipData.mcBulletLvl > 0) headerField.text = headerField.text + " - " + playerWeaponController.shipData.mcBulletLvl.ToString();
                content_ = content_.Replace("XXX", playerWeaponController.shipData.percBulletDamage.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.shipData.bulletCritChance.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.shipData.bulletCritDamage.ToString());
                break;
            case 1: // explosion
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.cCPrefab.classColor[contentType];
                if (playerWeaponController.shipData.mcExplosionLvl > 0) headerField.text = headerField.text + " - " + playerWeaponController.shipData.mcExplosionLvl.ToString();
                content_ = content_.Replace("XXX", playerWeaponController.shipData.percRocketDamage.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.shipData.rocketAOERadius.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.shipData.rocketLifeTime.ToString());
                break;
            case 2: // laser
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.cCPrefab.classColor[contentType];
                if (playerWeaponController.shipData.mcLaserLvl > 0) headerField.text = headerField.text + " - " + playerWeaponController.shipData.mcLaserLvl.ToString();
                content_ = content_.Replace("XXX", playerWeaponController.shipData.percLaserDamage.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.shipData.burnDamageChance.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.shipData.baseLaserTicks.ToString() + "x" + playerWeaponController.shipData.baseLaserTickDamage.ToString());
                content_ = content_.Replace("AAA", playerWeaponController.shipData.laserBurningTickDamangePercent.ToString());
                break;
            case 3: // support
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.cCPrefab.classColor[contentType];
                if (playerWeaponController.shipData.mcSupportLvl > 0) headerField.text = headerField.text + " - " + playerWeaponController.shipData.mcSupportLvl.ToString();
                content_ = content_.Replace("XXX", playerWeaponController.shipData.supportReloadTime.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.shipData.chanceToGetTwoExp.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.shipData.chanceToGetHealth.ToString());
                content_ = content_.Replace("AAA", playerWeaponController.shipData.boostInvulnerability.ToString());
                break;
            case 4: // ship
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.cCPrefab.classColor[contentType];

                content_ = content_.Replace("ZZZ", playerController.playerCurrentHealth.ToString() + " / " + playerController.playerMaxHealth.ToString());
                content_ = content_.Replace("AAA", playerController.protectionLvl.ToString() + " protection - " + playerController.protectionPerc.ToString());
                content_ = content_.Replace("BBB", playerController.energieMax.ToString());
                content_ = content_.Replace("CCC", playerController.energieProduction.ToString());
                content_ = content_.Replace("DDD", playerController.pickupRange.ToString());
                break;
            // Weapon-------------------------------------------------------------------------------------------------
            case 6: // Head Cannon
                headerField.text = tooltipContent[5].header;
                headerField.color = gameManager.cCPrefab.classColor[0];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[5].content; 
                content_ = content_.Replace("XXX", playerWeaponController.hcBulletDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.hcReloadTime_.ToString());
                 content_ = content_.Replace("ZZZ", playerWeaponController.hcSalveCount_.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.playerWeaponController.shipData.ToString() + "%/ " + upgradeChooseList.playerWeaponController.shipData.ToString() +  "% (" + Mathf.CeilToInt(((float)playerWeaponController.shipData.bulletCritDamage/100)* playerWeaponController.hcBulletDamage_).ToString() + ")");
                break;
            case 7: // Rocked Launcher
                headerField.text = tooltipContent[6].header;
                headerField.color = gameManager.cCPrefab.classColor[1];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[6].content;
                content_ = content_.Replace("XXX", playerWeaponController.rlDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.rlReloadTime_.ToString());
                content_ = content_.Replace("AAA", (playerWeaponController.rlAOERange_ * (1 + playerWeaponController.shipData.rocketAOERadius / 100)).ToString());
                break;
            case 8: // Fire Flies
                headerField.text = tooltipContent[7].header;
                headerField.color = gameManager.cCPrefab.classColor[0];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[7].content;
                content_ = content_.Replace("XXX", playerWeaponController.ffDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.ffReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.ffbulletCount_.ToString());
                content_ = content_.Replace("AAA", playerWeaponController.shipData.bulletCritChance.ToString() + "%/ " + playerWeaponController.shipData.bulletCritDamage.ToString() + "% (" + Mathf.CeilToInt(((float)playerWeaponController.shipData.bulletCritDamage/100) * playerWeaponController.ffDamage_).ToString() + ")");
                break;
            case 9: // Bullet Wings
                headerField.text = tooltipContent[8].header;
                headerField.color = gameManager.cCPrefab.classColor[0];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[8].content;
                content_ = content_.Replace("XXX", playerWeaponController.bwDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.bwRealoadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.bwSalveCount_.ToString());
                content_ = content_.Replace("AAA", playerWeaponController.shipData.bulletCritChance.ToString() + "%/ " + playerWeaponController.shipData.bulletCritDamage.ToString() + "% (" + Mathf.CeilToInt(((float)playerWeaponController.shipData.bulletCritDamage/100) * playerWeaponController.bwDamage_).ToString() + ")");
                break;
            case 10: // Life Modul
                headerField.text = tooltipContent[9].header;
                headerField.color = gameManager.cCPrefab.classColor[3];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[9].content;
                content_ = content_.Replace("XXX", playerWeaponController.lmLifePerTick_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.lmReloadTime_.ToString());
                break;
            case 11: // Spread Gun
                headerField.text = tooltipContent[10].header;
                headerField.color = gameManager.cCPrefab.classColor[0];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[10].content;
                content_ = content_.Replace("XXX", playerWeaponController.sgDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.sgReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.sgBulletCount_.ToString());
                content_ = content_.Replace("AAA", playerWeaponController.shipData.bulletCritChance.ToString() + "%/ " + playerWeaponController.shipData.bulletCritDamage.ToString() + "% (" + Mathf.CeilToInt(((float)playerWeaponController.shipData.bulletCritDamage/100) * playerWeaponController.sgDamage_).ToString() + ")");
                break;
            case 12: // Front Shield
                headerField.text = tooltipContent[11].header;
                headerField.color = gameManager.cCPrefab.classColor[3];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[11].content;
                content_ = content_.Replace("XXX", (playerWeaponController.fsShieldLife_ + playerWeaponController.shipData.shieldHealth).ToString());
                content_ = content_.Replace("YYY", (playerWeaponController.fsSpawnTime_).ToString());
                content_ = content_.Replace("ZZZ", (playerWeaponController.shipData.shieldDamage).ToString());
                break;
            case 13: // Back Schield
                headerField.text = tooltipContent[12].header;
                headerField.color = gameManager.cCPrefab.classColor[3]; 
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[12].content;
                content_ = content_.Replace("XXX", (playerWeaponController.bsShildLife_ + playerWeaponController.shipData.shieldHealth).ToString());
                content_ = content_.Replace("YYY", (playerWeaponController.bsSpawnTime_).ToString());
                content_ = content_.Replace("ZZZ", (playerWeaponController.shipData.shieldDamage).ToString());
                break;
            case 14: // Shock Nova
                headerField.text = tooltipContent[13].header;
                headerField.color = gameManager.cCPrefab.classColor[1]; 
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[13].content;
                content_ = content_.Replace("XXX", playerWeaponController.neDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.neReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.neRadius_.ToString());
                break;
            case 15: // Rocket Wings
                headerField.text = tooltipContent[14].header;
                headerField.color = gameManager.cCPrefab.classColor[1];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[14].content;
                content_ = content_.Replace("XXX", playerWeaponController.rwDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.rwReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.rwSalveCount_.ToString());
                content_ = content_.Replace("AAA", (0.4 * (1 + playerWeaponController.shipData.rocketAOERadius/100)).ToString());
                break;
            case 16: // Front Laser
                headerField.text = tooltipContent[15].header;
                headerField.color = gameManager.cCPrefab.classColor[2];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[15].content;
                content_ = content_.Replace("XXX", playerWeaponController.flDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.flReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.flShootingTime_.ToString());
                content_ = content_.Replace("AAA", playerWeaponController.shipData.burnDamageChance.ToString());
                break;
            case 17: // Orbital Laser
                headerField.text = tooltipContent[16].header;
                headerField.color = gameManager.cCPrefab.classColor[2];
                content_ = upgradeChooseList.uLObject.upgradeList[contentType].descriptionStr + " " + upgradeChooseList.uLObject.upgradeList[contentType].passivUpgradeString + Environment.NewLine;
                content_ += tooltipContent[16].content;
                content_ = content_.Replace("XXX", playerWeaponController.olDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.olReloadTime_.ToString());
                content_ = content_.Replace("AAA", playerWeaponController.shipData.burnDamageChance.ToString());
                break;
            case 69: // Thermal Sphere
                headerField.text = tooltipContent[17].header;
                headerField.color = gameManager.cCPrefab.classColor[2];
                content_ = "Work in Progress";
                break;

            default:
                break;
        }


        contentField.text = content_;


        // layout Element
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit) ? true : false;
    }
}

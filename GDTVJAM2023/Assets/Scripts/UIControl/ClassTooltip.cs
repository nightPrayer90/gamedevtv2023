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
    private PlayerController playerController;
    private PlayerMWController playerMWController;
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
            playerController = player.GetComponent<PlayerController>();
            playerMWController = player.GetComponent<PlayerMWController>();
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
                headerField.color = gameManager.globalClassColor[contentType];
                if (upgradeChooseList.mcBulletLvl > 0) headerField.text = headerField.text + " - " + upgradeChooseList.mcBulletLvl.ToString();
                content_ = content_.Replace("XXX", upgradeChooseList.percBulletDamage.ToString());
                content_ = content_.Replace("YYY", upgradeChooseList.baseBulletCritChance.ToString());
                content_ = content_.Replace("ZZZ", upgradeChooseList.baseBulletCritDamage.ToString());
                break;
            case 1: // explosion
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.globalClassColor[contentType];
                if (upgradeChooseList.mcExplosionLvl > 0) headerField.text = headerField.text + " - " + upgradeChooseList.mcExplosionLvl.ToString();
                content_ = content_.Replace("XXX", upgradeChooseList.percRocketDamage.ToString());
                content_ = content_.Replace("YYY", upgradeChooseList.baseRocketAOERadius.ToString());
                content_ = content_.Replace("ZZZ", upgradeChooseList.rocketLifeTime.ToString());
                break;
            case 2: // laser
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.globalClassColor[contentType];
                if (upgradeChooseList.mcLaserLvl > 0) headerField.text = headerField.text + " - " + upgradeChooseList.mcLaserLvl.ToString();
                content_ = content_.Replace("XXX", upgradeChooseList.percLaserDamage.ToString());
                content_ = content_.Replace("YYY", upgradeChooseList.baseLaserBurnDamageChance.ToString());
                content_ = content_.Replace("ZZZ", upgradeChooseList.baseLaserTicks.ToString() + "x" + upgradeChooseList.baseLaserTickDamage.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.laserBurningTickDamangePercent.ToString());
                break;
            case 3: // support
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.globalClassColor[contentType];
                if (upgradeChooseList.mcSupportLvl > 0) headerField.text = headerField.text + " - " + upgradeChooseList.mcSupportLvl.ToString();
                content_ = content_.Replace("XXX", upgradeChooseList.baseSupportRealoadTime.ToString());
                content_ = content_.Replace("YYY", upgradeChooseList.chanceToGetTwoExp.ToString());
                content_ = content_.Replace("ZZZ", upgradeChooseList.chanceToGet1Health.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.baseBoostInvulnerability.ToString());
                break;
            case 4: // ship
                content_ = tooltipContent[contentType].content;
                headerField.text = tooltipContent[contentType].header;
                headerField.color = gameManager.globalClassColor[contentType];

                switch (gameManager.ship)
                {
                    case 0:
                        headerField.color = gameManager.globalClassColor[0];
                        headerField.text = "Bullet Ship";
                        content_ = content_.Replace("MAINWEAPON", "Bullet Shooter");
                        content_ = content_.Replace("XXX", "Bullet damage: " + playerMWController.bulletBaseDamage.ToString());
                        content_ = content_.Replace("YYY", "Fire rate: " + playerMWController.fireRate.ToString());
                        break;
                    case 1:
                        headerField.text = "Rocket Ship";
                        headerField.color = gameManager.globalClassColor[1];
                        content_ = content_.Replace("MAINWEAPON", "Auto Rockets");
                        content_ = content_.Replace("XXX", "Rocket damage: " + playerMWController.bulletBaseDamage.ToString());
                        content_ = content_.Replace("YYY", "Fire rate " + playerMWController.fireRate.ToString() + " s" + Environment.NewLine + "Extra boss damage: " + upgradeChooseList.bossBonusDamage.ToString() + " %" );
                        break;
                    case 2:
                        headerField.text = "Laser Ship";
                        headerField.color = gameManager.globalClassColor[2];
                        content_ = content_.Replace("MAINWEAPON", "Twin Laser");
                        content_ = content_.Replace("XXX", "Laser damage: " + playerMWController.bulletBaseDamage.ToString());
                        content_ = content_.Replace("YYY", "Laser reload time: " + playerMWController.fireRate.ToString() + " s / shooting time: " + playerMWController.laserShootTime.ToString());
                        break;
                }

                content_ = content_.Replace("ZZZ", playerController.playerCurrentHealth.ToString() + " / " + playerController.playerMaxHealth.ToString());
                content_ = content_.Replace("AAA", "(" + playerController.protectionLvl.ToString() + ") " + playerController.protectionPerc.ToString());
                content_ = content_.Replace("BBB", gameManager.boostSlider.maxValue.ToString());
                content_ = content_.Replace("CCC", playerController.rotateSpeed.ToString());
                content_ = content_.Replace("DDD", playerController.pickupRange.ToString());
                break;
            // Weapon-------------------------------------------------------------------------------------------------
            case 6: // Head Cannon
                headerField.text = tooltipContent[5].header;
                headerField.color = gameManager.globalClassColor[0];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[5].content; 
                content_ = content_.Replace("XXX", playerWeaponController.hcBulletDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.hcReloadTime_.ToString());
                 content_ = content_.Replace("ZZZ", playerWeaponController.hcSalveCount_.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.baseBulletCritChance.ToString() + "%/ " + upgradeChooseList.baseBulletCritDamage.ToString() +  "% (" + Mathf.CeilToInt(((float)upgradeChooseList.baseBulletCritDamage/100)* playerWeaponController.hcBulletDamage_).ToString() + ")");
                break;
            case 7: // Rocked Launcher
                headerField.text = tooltipContent[6].header;
                headerField.color = gameManager.globalClassColor[1];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[6].content;
                content_ = content_.Replace("XXX", playerWeaponController.rlDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.rlReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.rlLifeTime_.ToString());
                content_ = content_.Replace("AAA", (1.2 * (1 + upgradeChooseList.baseRocketAOERadius / 100)).ToString());
                break;
            case 8: // Fire Flies
                headerField.text = tooltipContent[7].header;
                headerField.color = gameManager.globalClassColor[0];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[7].content;
                content_ = content_.Replace("XXX", playerWeaponController.ffDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.ffReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.ffbulletCount_.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.baseBulletCritChance.ToString() + "%/ " + upgradeChooseList.baseBulletCritDamage.ToString() + "% (" + Mathf.CeilToInt(((float)upgradeChooseList.baseBulletCritDamage/100) * playerWeaponController.ffDamage_).ToString() + ")");
                break;
            case 9: // Bullet Wings
                headerField.text = tooltipContent[8].header;
                headerField.color = gameManager.globalClassColor[0];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[8].content;
                content_ = content_.Replace("XXX", playerWeaponController.bwDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.bwRealoadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.bwSalveCount_.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.baseBulletCritChance.ToString() + "%/ " + upgradeChooseList.baseBulletCritDamage.ToString() + "% (" + Mathf.CeilToInt(((float)upgradeChooseList.baseBulletCritDamage/100) * playerWeaponController.bwDamage_).ToString() + ")");
                break;
            case 10: // Life Modul
                headerField.text = tooltipContent[9].header;
                headerField.color = gameManager.globalClassColor[3];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[9].content;
                content_ = content_.Replace("XXX", playerWeaponController.lmLifePerTick_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.lmReloadTime_.ToString());
                break;
            case 11: // Spread Gun
                headerField.text = tooltipContent[10].header;
                headerField.color = gameManager.globalClassColor[0];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[10].content;
                content_ = content_.Replace("XXX", playerWeaponController.sgDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.sgReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.sgBulletCount_.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.baseBulletCritChance.ToString() + "%/ " + upgradeChooseList.baseBulletCritDamage.ToString() + "% (" + Mathf.CeilToInt(((float)upgradeChooseList.baseBulletCritDamage/100) * playerWeaponController.sgDamage_).ToString() + ")");
                break;
            case 12: // Front Shield
                headerField.text = tooltipContent[11].header;
                headerField.color = gameManager.globalClassColor[3];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[11].content;
                content_ = content_.Replace("XXX", (playerWeaponController.fsShieldLife_ + upgradeChooseList.shieldHealth).ToString());
                content_ = content_.Replace("YYY", (playerWeaponController.fsSpawnTime_).ToString());
                content_ = content_.Replace("ZZZ", (upgradeChooseList.shieldDamage).ToString());
                break;
            case 13: // Back Schield
                headerField.text = tooltipContent[12].header;
                headerField.color = gameManager.globalClassColor[3]; 
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[12].content;
                content_ = content_.Replace("XXX", (playerWeaponController.bsShildLife_ + upgradeChooseList.shieldHealth).ToString());
                content_ = content_.Replace("YYY", (playerWeaponController.bsSpawnTime_).ToString());
                content_ = content_.Replace("ZZZ", (upgradeChooseList.shieldDamage).ToString());
                break;
            case 14: // Shock Nova
                headerField.text = tooltipContent[13].header;
                headerField.color = gameManager.globalClassColor[1]; 
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[13].content;
                content_ = content_.Replace("XXX", playerWeaponController.neDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.neReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.neRadius_.ToString());
                break;
            case 15: // Rocket Wings
                headerField.text = tooltipContent[14].header;
                headerField.color = gameManager.globalClassColor[1];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[14].content;
                content_ = content_.Replace("XXX", playerWeaponController.rwDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.rwReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.rwSalveCount_.ToString());
                content_ = content_.Replace("AAA", (0.4 * (1 + upgradeChooseList.baseRocketAOERadius/100)).ToString());
                break;
            case 16: // Front Laser
                headerField.text = tooltipContent[15].header;
                headerField.color = gameManager.globalClassColor[2];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[15].content;
                content_ = content_.Replace("XXX", playerWeaponController.flDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.flReloadTime_.ToString());
                content_ = content_.Replace("ZZZ", playerWeaponController.flShootingTime_.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.baseLaserBurnDamageChance.ToString());
                break;
            case 17: // Orbital Laser
                headerField.text = tooltipContent[16].header;
                headerField.color = gameManager.globalClassColor[2];
                content_ = upgradeChooseList.weaponUpgrades[contentType].descriptionStr + " " + upgradeChooseList.weaponUpgrades[contentType].passivUpgrades + Environment.NewLine;
                content_ += tooltipContent[16].content;
                content_ = content_.Replace("XXX", playerWeaponController.olDamage_.ToString());
                content_ = content_.Replace("YYY", playerWeaponController.olReloadTime_.ToString());
                content_ = content_.Replace("AAA", upgradeChooseList.baseLaserBurnDamageChance.ToString());
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

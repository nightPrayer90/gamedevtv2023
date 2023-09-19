using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("contentType " + contentType);

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
            playerMWController = player.GetComponent<PlayerMWController>();
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
                        content_ = content_.Replace("XXX", "Rocket damage");
                        content_ = content_.Replace("YYY", "Fire rate");
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

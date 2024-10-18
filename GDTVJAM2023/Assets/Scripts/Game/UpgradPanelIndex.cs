using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class UpgradPanelIndex : MonoBehaviour
{
    [Header("Panel Settings")]

    public int index;
    public Image panelImage;

    [Header("Panel Values")]
    public UpgradePanelController upgradePanelController;
    public Sprite spPanelSelect;
    public Sprite spPanelDeselcet;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;
    public Image iconPanel;
    public TextMeshProUGUI mainClass;
    private bool isTweening = true;
    public bool isSelected = false;

    private int upgradeIndex = -1;

    [Header("Requirement System")]
    public Image req1;
    public TextMeshProUGUI req1Text;
    public Image req2;
    public TextMeshProUGUI req2Text;
    public Image req3;
    public TextMeshProUGUI req3Text;
    public GameObject unique;
    public TextMeshProUGUI uniqueText;
    public TextMeshProUGUI upgradeText;
    public GameManager gameManager;
    public UpgradeChooseList upgradeChooseList;
    public NewPlayerController playerController;
    public PlayerWeaponController playerWeaponController;
    public Transform panelTransform;


    public void EnablePanel()
    {
        isTweening = true;
        isSelected = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 400f, transform.position.z);

        // Get upgradeIndex number
        upgradeIndex = upgradePanelController.selectedNumbers_[index];
        SetDescription();

        // fade in
        float fadeinDelay = (index / 15f);
        panelImage.sprite = spPanelDeselcet;
        transform.DOLocalMoveY(33f, 0.22f, true).SetUpdate(UpdateType.Normal, true).SetDelay(fadeinDelay).OnComplete(() =>
        {
            panelTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 5, 1).SetUpdate(true).OnComplete(() =>
            {
                upgradePanelController.isTweening = false;
                AudioManager.Instance.PlaySFX("MouseKlick");
                isTweening = false;

                if (upgradePanelController.selectetPanel == index)
                {
                    if (isSelected == false)
                    {
                        SelectPanel();
                    }
                }
            });
        });
    }


    public void SetDescription()
    {

        Upgrade upgrade = upgradeChooseList.uLObject.upgradeList[upgradeIndex];

        // Panel values
        headerText.text = upgrade.headerStr;
        headerText.color = gameManager.cCPrefab.classColor[upgrade.colorIndex];

        iconPanel.sprite = upgrade.iconPanel;

        descriptionText.text = upgrade.descriptionStr;

        SetUpgradeText(upgradeIndex);

        mainClass.text = upgrade.mainClass.ToString();
        if (mainClass.text == "Nothing") mainClass.text = "";


        switch (upgrade.mainClass)
        {
            case Upgrade.MainClass.Bullet:
                mainClass.color = gameManager.cCPrefab.classColor[0];
                break;
            case Upgrade.MainClass.Explosion:
                mainClass.color = gameManager.cCPrefab.classColor[1];
                break;
            case Upgrade.MainClass.Laser:
                mainClass.color = gameManager.cCPrefab.classColor[2];
                break;
            case Upgrade.MainClass.Support:
                mainClass.color = gameManager.cCPrefab.classColor[3];
                break;
        }
       

        // requerments
        // reset
        int count_ = 0;
        req1.color = gameManager.cCPrefab.classColor[8];
        req1Text.text = "";
        req2.color = gameManager.cCPrefab.classColor[8];
        req2Text.text = "";
        req3.color = gameManager.cCPrefab.classColor[8];
        req3Text.text = "";

        Color[] reqColor = new Color[2] { gameManager.cCPrefab.classColor[8], gameManager.cCPrefab.classColor[8] };
        string[] reqText = new string[2] { "", "" };

        // set
        if (upgrade.reqBullet > 0)
        {
            reqColor[0] = gameManager.cCPrefab.classColor[0];
            reqText[0] = upgrade.reqBullet.ToString();
            count_ = 1;
        }
        if (upgrade.reqRocket > 0)
        {
            reqColor[count_] = gameManager.cCPrefab.classColor[1];
            reqText[count_] = upgrade.reqRocket.ToString();
            count_ = 1;
        }
        if (upgrade.reqLaser > 0)
        {
            reqColor[count_] = gameManager.cCPrefab.classColor[2];
            reqText[count_] = upgrade.reqLaser.ToString();
            count_ = 1;
        }
        if (upgrade.reqSupport > 0)
        {
            reqColor[count_] = gameManager.cCPrefab.classColor[3];
            reqText[count_] = upgrade.reqSupport.ToString();
        }


        req1.color = reqColor[0];
        req1Text.text = reqText[0];

        req2.color = reqColor[1];
        req2Text.text = reqText[1];

        if (upgrade.reqAbility != "")
        {
            req3.color = gameManager.cCPrefab.classColor[upgrade.colorIndex];
            req3Text.text = upgrade.reqAbility;
        }
        else
        {
            req3.color = gameManager.cCPrefab.classColor[8];
            req3Text.text = "";
        }

        // Quantity
        if (upgrade.UpgradeCount != 999)
        {
            unique.SetActive(true);
            int upgradeQuanitiy = upgradeChooseList.upgrades[upgradeIndex].upgradeStartCount - upgradeChooseList.upgrades[upgradeIndex].upgradeIndexInstalled;
            uniqueText.text = (upgradeQuanitiy).ToString();
        }
        else
        {
            unique.SetActive(false);
        }
    }

    private void SetUpgradeText(int upgradeIndex)
    {
        string upgradString = "";
        string upgradeColor = "color=#00BFFF";

        switch (upgradeIndex)
        {
            case 0: //upgrade: health
                upgradString = $"{playerController.playerMaxHealth} HP to" + $" <{upgradeColor}>{(playerController.playerMaxHealth + 1)} HP</color>";
                break;
            case 1: //upgrade: main Weapon damage
                upgradString = "";
                break;
            case 2: //upgrade: Protection
                float currentPercentage = playerController.protectionPerc;
                float normalizedLvl = Mathf.InverseLerp(0, 10, playerController.protectionLvl + 1);
                float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);
                descriptionText.text = descriptionText.text.Replace("XX", (targetPercentage - currentPercentage).ToString());

                upgradString = $"{currentPercentage}% to" + $" <{upgradeColor}>{targetPercentage}%</color>";
                break;
            case 3: //upgrade: boost
                upgradString = $"{playerController.energieMax} TJ to " + $"<{upgradeColor}>{playerController.energieMax * (1.15f)} TJ</color>";
                break;
            case 4: //upgrade: rotate speed
                upgradString = $"{playerWeaponController.shipData.percAgility}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.percAgility+10}%</color>";
                break;
            case 5: //upgrade: pickup Ranged
                upgradString = $"{Mathf.Round(playerController.pickupRange*10)/10} m to " + $"<{upgradeColor}>{ Mathf.Round( (playerController.pickupRange + 0.8f)*10)/10 } m</color>";
                break;
            case 6: //weapon: headgun
                upgradString = $"Crit chance to <{upgradeColor}>{playerWeaponController.shipData.bulletCritChance + playerWeaponController.shipData.critChance}% </color>";
                break;
            case 7: //weapon: rocket launcher
                upgradString = $"Explosion AOE to <{upgradeColor}>{playerWeaponController.shipData.rocketAOERadius + playerWeaponController.shipData.aoeRange}% </color>";
                break;
            case 8: //weapon: fire flys
                upgradString = $"Crit chance to <{upgradeColor}>{playerWeaponController.shipData.bulletCritChance + playerWeaponController.shipData.critChance}% </color>";
                break;
            case 9: //weapon: bullet wings
                upgradString = $"Crit chance to <{upgradeColor}>{playerWeaponController.shipData.bulletCritChance + playerWeaponController.shipData.critChance}% </color>";
                break;
            case 10: //weapon: -
                upgradString = $"Explosion AOE to <{upgradeColor}>{playerWeaponController.shipData.rocketAOERadius + playerWeaponController.shipData.aoeRange}% </color>";
                break;
            case 11: //weapon: spread gun
                upgradString = $"Crit chance to <{upgradeColor}>{playerWeaponController.shipData.bulletCritChance + playerWeaponController.shipData.critChance}% </color>";
                break;
            case 12: //weapon: -

                break;
            case 13: //weapon: -

                break;
            case 14: //weapon: schock nova
                upgradString = $"Explosion AOE to <{upgradeColor}>{playerWeaponController.shipData.rocketAOERadius + playerWeaponController.shipData.aoeRange}% </color>";
                break;
            case 15: //weapon: rocket wings
                upgradString = $"Explosion AOE to <{upgradeColor}>{playerWeaponController.shipData.rocketAOERadius + playerWeaponController.shipData.aoeRange}% </color>";
                break;
            case 16: //weapon: front laser
                upgradString = $"Burning chance to <{upgradeColor}>{playerWeaponController.shipData.burnDamageChance + playerWeaponController.shipData.burningChance}% </color>";
                break;
            case 17: //weapon: orbital laser
                upgradString = $"Burning chance to <{upgradeColor}>{playerWeaponController.shipData.burnDamageChance + playerWeaponController.shipData.burningChance}% </color>";
                break;
            case 18: //bullet class
                upgradString = $"Crit chance to <{upgradeColor}>{playerWeaponController.shipData.bulletCritChance + playerWeaponController.shipData.critChance}% </color>";
                break;
            case 19: //rocked class
                upgradString = $"Explosion AOE to <{upgradeColor}>{playerWeaponController.shipData.rocketAOERadius + playerWeaponController.shipData.aoeRange}% </color>";
                break;
            case 20: //laser class
                upgradString = $"Burning chance to <{upgradeColor}>{playerWeaponController.shipData.burnDamageChance + playerWeaponController.shipData.burningChance}% </color>";
                break;
            case 21: //support class
                upgradString = $"Reaload Speed to <{upgradeColor}>{playerWeaponController.shipData.supportReloadTime + playerWeaponController.shipData.realodTime}% </color>";
                break;
            case 22: //weapon: Backfire Beam
                upgradString = $"Burning chance to <{upgradeColor}>{playerWeaponController.shipData.burnDamageChance + playerWeaponController.shipData.burningChance}% </color>";
                break;
            case 23: // Guardian Drive
                upgradString = $"{playerWeaponController.shipData.boostSize}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.boostSize * 1.6f}%</color>";
                break;
            case 24: // Force Multiplier
                upgradString = $"{playerWeaponController.shipData.boostDamage} to " + $"<{upgradeColor}>{playerWeaponController.shipData.boostDamage * 2}</color>";
                break;
            case 25: // Nova Overdrive
                upgradString = "";
                break;
            case 26: // crit damage
                upgradString = $"{playerWeaponController.shipData.bulletCritDamage}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.bulletCritDamage + 7}%</color>";
                break;
            case 27: // crit chance
                upgradString = $"{playerWeaponController.shipData.bulletCritChance}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.bulletCritChance + 3}%</color>";
                break;
            case 28: // explosion range
                upgradString = $"{playerWeaponController.shipData.aoeRange}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.aoeRange + 8}%</color>";
                break;
            case 29: // burning tick damage
                upgradString = $"{playerWeaponController.shipData.laserBurningTickDamangePercent}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.laserBurningTickDamangePercent + 20}%</color>";
                break;
            case 30: // burning chance
                upgradString = $"{playerWeaponController.shipData.burnDamageChance}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.burnDamageChance + 1}%</color>";
                break;
            case 31: // invulnerability
                upgradString = $"{playerWeaponController.shipData.boostInvulnerability} s to " + $"<{upgradeColor}>{playerWeaponController.shipData.boostInvulnerability + 0.2} s</color>";
                break;
            case 32: // chance to get douple exp
                upgradString = $"{playerWeaponController.shipData.chanceToGetTwoExp}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.chanceToGetTwoExp + 10}%</color>";
                break;
            case 33: // 1 life to collect exp
                upgradString = $"{playerWeaponController.shipData.chanceToGetHealth}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.chanceToGetHealth + 2}%</color>";
                break;
            case 34: // rocket life time
                upgradString = $"{playerWeaponController.shipData.rocketLifeTime}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.rocketLifeTime + 0.2}%</color>";
                break;
            case 35: // Chance to trigger a Nova if u get hit
          
                break;
            case 36: // Rocket Overdrive

                break;
            case 37: // Bullet crit chance +10%
                upgradString = $"{playerWeaponController.shipData.bulletCritChance}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.bulletCritChance + 10}%</color>";
                break;
            case 38: // Extended Blast Expansion
                upgradString = $"{playerWeaponController.shipData.aoeRange}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.aoeRange + 15}%</color>";
                break;
            case 39: // Ignition Augment
                upgradString = $"{playerWeaponController.shipData.baseLaserTickDamage}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.baseLaserTickDamage + 3}%</color>";
                break;
            case 40: // Fortified Defense
                upgradString = $"{playerWeaponController.shipData.shieldHealth} to " + $"<{upgradeColor}>{playerWeaponController.shipData.shieldHealth + 1}</color>";
                break;
            case 41: // Shielded Strike
                upgradString = $"{playerWeaponController.shipData.shieldDamage} to " + $"<{upgradeColor}>{playerWeaponController.shipData.shieldDamage + 5}</color>";
                break;
            case 42: // Critical Impact Drive

                break;
            case 43: // Ignition Shield

                break;
            case 44: // Shield Nova

                break;
            case 45: // Shieldbreaker's Might

                break;
            case 46: // Ballistic Boost
                upgradString = $"{playerWeaponController.shipData.percBulletDamage}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.percBulletDamage + 12}%</color>";
                break;
            case 47: // Boom Boom Boost
                upgradString = $"{playerWeaponController.shipData.percRocketDamage}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.percRocketDamage + 12}%</color>";
                break;
            case 48: // Beam Boost
                upgradString = $"{playerWeaponController.shipData.percLaserDamage}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.percLaserDamage + 12}%</color>";
                break;
            case 49: // Rapid Laser Reload
                upgradString = $"{playerWeaponController.shipData.percMainDownTimeLaser}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.percMainDownTimeLaser + 10}%</color>";
                break;
            case 50: // Wide Spray Expansion
                upgradString = $"{playerWeaponController.sgBulletCount} to " + $"<{upgradeColor}>{playerWeaponController.sgBulletCount + 2}</color>";
                break;
            case 51: // Lightning Reload
                upgradString = $"{playerWeaponController.shipData.percMainAttackSpeedBullet}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.percMainAttackSpeedBullet + 7}%</color>";
                break;
            case 52: // Kaboomed Targets
                
                break;
            case 53: // Critical Explosion

                break;
            case 54: // Detonation Crits

                break;
            case 55: // Titan Slayer
                upgradString = $"{playerWeaponController.shipData.bossBonusDamage}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.bossBonusDamage + 25}%</color>";
                break;
            case 56: // Laser Orbit Accelerator
                upgradString = $"{playerWeaponController.olRotationSpeed}% to " + $"<{upgradeColor}>{playerWeaponController.olRotationSpeed * 1.25}%</color> \n" +
                    $"{playerWeaponController.olDamage} to " + $"<{upgradeColor}>{playerWeaponController.olDamage +2}</color>";
                break;
            case 57: // Explosive Impact
                upgradString = $"{playerWeaponController.rlAOERange}% to " + $"<{upgradeColor}>{playerWeaponController.rlAOERange * 1.2}%</color>";
                break;
            case 58: // Luminous Growth
               
                break;
            case 59: // Engine boost
                upgradString = $"{playerController.energieProduction}% to " + $"<{upgradeColor}>{playerController.energieProduction * (1.2f)}%</color>";
                break;
            case 60: // Engine Overload
                upgradString = $"{playerController.energieProduction}% to " + $"<{upgradeColor}>{playerController.energieProduction * (1.6f)}%</color>";
                break;
            case 61: // Torchshield Drive

                break;
            case 62: // Explosiv Shield 

                break;
            case 63: // Natural energie
                upgradString = $"{playerWeaponController.shipData.chanceToGetFullEnergy}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.chanceToGetFullEnergy + 5}%</color>";
                break;
            case 64: // Fast Head Cannon

                break;
            case 65: // Big Head Cannon

                break;
            case 66: // Sniper Head Cannon

                break;
            case 67: // Gatlin Head Cannon
                upgradString = $"{playerWeaponController.hcReloadTime} bullets/s to " + $"<{upgradeColor}>{0.5f} bullets/s</color>";
                break;
            case 68: // fast Spread gun
                upgradString = $"{playerWeaponController.sgReloadTime} bullets/s to " + $"<{upgradeColor}>{playerWeaponController.sgReloadTime - 0.5f} bullets/s</color>";
                break;
            case 69: // Therminal Spheres
                upgradString = $"Burning chance to <{upgradeColor}>{playerWeaponController.shipData.burnDamageChance + playerWeaponController.shipData.burningChance}% </color>";
                break;
            case 70: // Chaotic Sunfire Spheres

                break;
            case 71: // Power Spheres
                upgradString = $"{playerWeaponController.tsDamage} to " + $"<{upgradeColor}>{playerWeaponController.tsDamage + 2}</color>";
                break;
            case 72: // Creazy Spheres
                
                break;
            case 73: // Dynamic Spheres

                break;
            case 74: // Enhanced Plasma  Spheres
                upgradString = $"{playerWeaponController.tsLifetime} s to " + $"<{upgradeColor}>{playerWeaponController.tsLifetime + 1} s</color>";
                break;
            case 75: // Enhanced Plasma  Spheres

                break;
            case 76: // Cannon Power
                upgradString = $"{playerWeaponController.hcBulletDamage} to " + $"<{upgradeColor}>{playerWeaponController.hcBulletDamage + 1}</color>";
                break;
            case 77: // Fury Novas

                break;
            case 78: // Nuclear Novas

                break;
            case 79: // Power Nova
                upgradString = $"{playerWeaponController.neDamage} to " + $"<{upgradeColor}>{playerWeaponController.neDamage + 2}</color>";
                break;
            case 80: // Orbital Laser Array
                upgradString = $"{playerWeaponController.olCount} to " + $"<{upgradeColor}>{playerWeaponController.olCount + 1}</color>";
                break;
            case 81: // Tactical Minefield
                upgradString = $"Explosion AOE to <{upgradeColor}>{playerWeaponController.shipData.rocketAOERadius + playerWeaponController.shipData.aoeRange}% </color>";
                break;
            case 82: // Burning Spheres
                
                break;
            case 83: // Bloodlust Shurikens
                upgradString = $"Burning chance to <{upgradeColor}>{playerWeaponController.shipData.burnDamageChance + playerWeaponController.shipData.burningChance}% </color>";
                break;
            case 84: // Dancing Blades
                upgradString = $"{playerWeaponController.bbMainBeams} to " + $"<{upgradeColor}>{playerWeaponController.bbMainBeams + 1}</color>";
                break;
            case 85: // Blade Nova

                break;
            case 86: // Overheating Rockets

                break;
            case 87: // Corpse explosion

                break;
            case 88: // Life Orb Boost

                break;
            case 89: // EXP Orb Boost

                break;
            case 90: // Time Orb Boost

                break;
            case 91: // Orb Jackpot

                break;
            case 92: // Recoil Reducer

                break;
            case 93: // Growth Impulse

                break;
            case 94: // Infernal Zone

                break;
            case 95: // Astral Shockwave

                break;
            case 96: // Rising Force

                break;
            case 97: // Enduring Inferno

                break;
            case 98: // Collector
                upgradString = $"{playerWeaponController.shipData.chanceToGetScrap}% to " + $"<{upgradeColor}>{playerWeaponController.shipData.chanceToGetScrap+3}%</color>";
                break;
            case 99: // Lasting Shield

                break;
        }

        upgradeText.text = upgradString;
        if (isSelected == false) upgradeText.enabled = false;
    }

    public void OnMouseEnter_()
    {
        upgradePanelController.selectetPanel = index;

        if (isTweening == false)
        {
            // Farbe des Panels ändern, wenn die Maus über das Panel fährt
            upgradePanelController.UpdateValuePanelOnMouseEnter(index);
        }
    }

    public void SelectPanel()
    {
        isSelected = true;
        panelImage.sprite = spPanelSelect;
        upgradeText.enabled = true;

        AudioManager.Instance.PlaySFX("MouseHover");

        if (isTweening == false)
        {
            panelTransform.DOComplete();
            panelTransform.DOPunchScale(new Vector3(0.08f, 0.08f, 0.08f), 0.08f, 5, 1).SetUpdate(true);
        }
    }

    public void OnMouseExit_()
    {
        if (isTweening == false)
        {
            // Zurück zur Standardfarbe wechseln, wenn die Maus das Panel verlässt
            upgradePanelController.selectetPanel = -1;
            DeselectPanel();
        }
    }

    public void DeselectPanel()
    {
        isSelected = false;
        panelImage.sprite = spPanelDeselcet;
        upgradeText.enabled = false;
    }

    public void OnMouseDown_()
    {
        if (isTweening == false && upgradePanelController.isButtonPressed == false)
        {
            if (upgradePanelController.selectetPanel == index)
            {
                panelImage.sprite = spPanelSelect;

                upgradePanelController.ChooseAValue(index);
                AudioManager.Instance.PlaySFX("WindowOpen");

                panelTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), .2f, 5, 1).SetUpdate(true).OnComplete(() => { upgradePanelController.TriggerPanel(index); });

                isTweening = true;
            }
        }
    }

    public void FadeOut(int index_)
    {
        float duration = (float)index / 15;

        if (upgradePanelController.selectetPanel == index)
        {
            panelImage.sprite = spPanelSelect;

            transform.DOLocalMoveY(855f, .7f, true).SetUpdate(UpdateType.Normal, true).SetEase(Ease.InQuart).OnComplete(() =>
            {
                upgradePanelController.GetUpdate();
            });
        }
        else
        {
            transform.DOLocalMoveY(855f, .5f, true).SetUpdate(UpdateType.Normal, true).SetEase(Ease.InQuart).SetDelay(0.2f + duration);
        }
    }
}


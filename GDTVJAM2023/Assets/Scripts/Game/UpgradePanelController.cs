using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;


public class UpgradePanelController : MonoBehaviour
{
    [Header("Main Panel")]
    public List<UpgradPanelIndex> panelList;
    public UpgradeShipPanelController shipPanalController;
    private int rerolls = 3;
    private int upgradeType;
    public TMP_Text rerollText;
    public GameObject rerollImage;
    [HideInInspector] public int[] upgradeIndex;
    [HideInInspector] public int[] upgradeCount;
    [HideInInspector] public Sprite[] iconPanel;

    [Header("Upgrade Control")]
    [HideInInspector] public List<int> valueList;
    [HideInInspector] public int[] selectedNumbers_ = new int[3];

    [Header("Value Panel")]
    public Image bkImage;
    //public List<Color> cl lc assColors;
    public List<Image> classPanels = new List<Image>();
    public List<Image> selectedUpgradePanelList = new List<Image>();
    private int weaponCount = 0;

    // Objects
    public GameManager gameManager;
    public NewPlayerController playerController;
    public PlayerWeaponController playerWeaponController;
    public UpgradeChooseList upgradeChooseList;

    public int selectetPanel;
    public bool isButtonPressed = false;
    public bool isTweening = true;

    public PlayerInputHandler inputHandler;



    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle


    void OnEnable()
    {
        selectetPanel = -1;
        isTweening = true;
        isButtonPressed = false;

        bkImage.DOFade(1f, 0.2f).SetUpdate(true);


        // events
        inputHandler.DisableGameControls();
        inputHandler.EnableUIControls();

        inputHandler.OnNavigateUIInputChanged += HandleNavigateInput;
        inputHandler.OnClickInputChanged += HandleSubmitInput;
        inputHandler.OnReroll += HandleRerollInput;
    }

    private void OnDisable()
    {
        // events
        inputHandler.OnNavigateUIInputChanged -= HandleNavigateInput;
        inputHandler.OnClickInputChanged -= HandleSubmitInput;
        inputHandler.OnReroll -= HandleRerollInput;

        inputHandler.DisableUIControls();
        inputHandler.EnableGameControls();
    }


    private void HandleNavigateInput(Vector2 inputVector2)
    {
        if (inputVector2 != Vector2.zero && isTweening == false && isButtonPressed == false)
        {
            if (inputVector2.x >= 0.5)
            {
                switch (selectetPanel)
                {
                    case -1:
                        selectetPanel = 1;
                        break;
                    case 0:
                        selectetPanel = 1;
                        break;
                    case 1:
                        selectetPanel = 2;
                        break;
                    case 2:
                        selectetPanel = 0;
                        break;
                }
                UpdateValuePanelOnMouseEnter(selectetPanel);
            }
            else if (inputVector2.x <= -0.5)
            {
                switch (selectetPanel)
                {
                    case -1:
                        selectetPanel = 0;
                        break;
                    case 0:
                        selectetPanel = 2;
                        break;
                    case 1:
                        selectetPanel = 0;
                        break;
                    case 2:
                        selectetPanel = 1;
                        break;
                }
                UpdateValuePanelOnMouseEnter(selectetPanel);
            }
        }
    }

    private void HandleSubmitInput()
    {
        if (selectetPanel != -1 && isButtonPressed == false)
        {
            panelList[selectetPanel].OnMouseDown_();
            isButtonPressed = true;
        }
    }

    private void HandleRerollInput()
    {
        if (rerolls > 0)
        {
            rerolls--;
            CreateRandomNumbers(upgradeType);
            AudioManager.Instance.PlaySFX("Reroll");
        }
    }
    #endregion

    /* **************************************************************************** */
    /* Upgrade Prepare ------------------------------------------------------------ */
    /* **************************************************************************** */
    #region upgrade prepare


    // (help function) create 3 random numbers für the upgrade/ ability panel - trigger by levelup
    public void CreateRandomNumbers(int playerLevel)
    {
        List<int> selectedNumbers = new List<int>();
        List<int> valueList = new List<int>();

        upgradeType = playerLevel;

        // create temporary list from weapons or normal upgrades - depends on the player level
        if (playerLevel == -1) // new weapon
            valueList.AddRange(upgradeChooseList.BuildUpgradeList(Upgrade.UpgradeTyp.WeaponUpgrade));
        else if ((playerLevel % 5) == 0) // class update
            valueList.AddRange(upgradeChooseList.BuildUpgradeList(Upgrade.UpgradeTyp.ClassUpgrade));
        else
            valueList.AddRange((upgradeChooseList.BuildUpgradeList(Upgrade.UpgradeTyp.NormalUpgrade)));

        // create 3 random possible numbers that do not duplicate each other
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, valueList.Count);     // generate a random number
            selectedNumbers.Add(valueList[randomIndex]);            // save the number in a list
            valueList.RemoveAt(randomIndex);                        // remove the value from the temp list
        }

        selectedNumbers_ = selectedNumbers.ToArray();

        SetRerollText();
        StringLibrary();
    }

    public void StringLibrary()
    {
        upgradeIndex = new int[3] { 0, 0, 0 };
        iconPanel = new Sprite[3] { null, null, null };
        upgradeCount = new int[3] { 0, 0, 0 };

        for (int i = 0; i < 3; i++)
        {
            upgradeIndex[i] = selectedNumbers_[i];
            Upgrade upgrade = upgradeChooseList.uLObject.upgradeList[upgradeIndex[i]];

            gameManager.playerData.skillsSpotted[upgradeIndex[i]] = true;

            // set text descriptions
            iconPanel[i] = upgrade.iconPanel;
            panelList[i].EnablePanel();
        }
    }
    #endregion




    /* **************************************************************************** */
    /* Input Control * ------------------------------------------------------------ */
    /* **************************************************************************** */
    #region input control

    // Part of the Mouse HoverEvent
    public void UpdateValuePanelOnMouseEnter(int index)
    {
        if (isTweening == false)
        {
            int number = selectedNumbers_[index];

            panelList[0].DeselectPanel();
            panelList[1].DeselectPanel();
            panelList[2].DeselectPanel();

            // on mouse exit
            if (selectetPanel != -1)
                panelList[index].SelectPanel();

            // weapon Upgrades
            if ((number > 5 && number <= 17) || number == 69)
            {
                //                selectedUpgradePanelList[weaponCount].sprite = iconPanel[index];
                selectedUpgradePanelList[weaponCount].gameObject.GetComponent<ClassTooltipTrigger>().contentType = number;
            }

            Upgrade uC = upgradeChooseList.uLObject.upgradeList[number];

            // update class colors
            int index_ = (int)uC.mainClass;
        }
    }


    // Click Event - Choose an Ability
    public void ChooseAValue(int index)
    {
        int number = selectedNumbers_[index];
        isTweening = true;
        upgradeChooseList.upgrades[number].upgradeIndexInstalled += 1;
        isButtonPressed = true;

        switch (number)
        {
            case 0: //upgrade: health
                playerController.playerMaxHealth = playerController.playerMaxHealth + 2;
                playerController.playerCurrentHealth = Mathf.Min(playerController.playerCurrentHealth + 2, playerController.playerMaxHealth);
                gameManager.UpdateUIPlayerHealth(playerController.playerCurrentHealth, playerController.playerMaxHealth);
                break;
            case 1: //upgrade: main Weapon damage
                playerWeaponController.UpdateMWDamage(1);
                break;
            case 2: //upgrade: Protection

                float normalizedLvl = Mathf.InverseLerp(0, 10, playerController.protectionLvl + 1);
                float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);
                playerController.protectionPerc = targetPercentage;
                playerController.protectionLvl++;
                break;
            case 3: //upgrade: boost

                playerController.energieMax = playerController.energieMax * (1.15f);
                gameManager.energieSlider.maxValue = playerController.energieMax;
                gameManager.energieSlider.value = playerController.energieMax;
                break;
            case 4: //upgrade: rotate speed
                playerController.UpdateAgility(10);
                break;
            case 5: //upgrade: pickup Range
                playerController.UpdatePickUpRange(0.8f);

                break;
            case 6: //weapon: headgun
                playerWeaponController.isHeadCannon = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();
                weaponCount++;

                upgradeChooseList.upgrades[64].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[64].UpgradeCount;
                upgradeChooseList.upgrades[65].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[65].UpgradeCount;

                //upgradeChooseList.weaponUpgrades[64].reqBullet = 2; // Fast Head Cannon
                //upgradeChooseList.weaponUpgrades[65].reqBullet = 2; // Big Head Cannon
                break;
            case 7: //weapon: rocket launcher
                playerWeaponController.isRocketLauncher = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();
                upgradeChooseList.upgrades[57].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[57].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[57].reqRocket = 3; // Explosive Impact

                weaponCount++;
                break;
            case 8: //weapon: fire flys
                playerWeaponController.isFireFlies = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();
                upgradeChooseList.upgrades[58].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[58].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[58].reqBullet = 4; // Explosive Impact

                weaponCount++;
                break;
            case 9: //weapon: bullet wings
                playerWeaponController.isBulletWings = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();
                weaponCount++;
                break;
            case 10: //weapon: -
                /*playerWeaponController.shipData.chanceToGetTwoExp += 15;
                playerWeaponController.shipData.chanceToGetFullEnergy += 2;
                playerWeaponController.isLifeModul = true;
                playerWeaponController.WeaponChoose();
                
                UpdateClass(number,1);
                GoBackToDimension();
                weaponCount++;

                upgradeChooseList.upgrades[64].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[64].UpgradeCount;
                upgradeChooseList.upgrades[32].upgradeStartCount += 3;
                upgradeChooseList.upgrades[33].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[33].UpgradeCount;

                //upgradeChooseList.weaponUpgrades[63].reqSupport = 2; // Natural Energy
                //upgradeChooseList.weaponUpgrades[32].reqSupport = 2; // Expansion Fortuity
                //upgradeChooseList.weaponUpgrades[33].reqSupport = 3; // Vitality Infusion
                */
                break;
            case 11: //weapon: spread gun
                playerWeaponController.isSpreadGun = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();

                upgradeChooseList.upgrades[50].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[50].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[50].reqBullet = 2;  // Wide Spray Expansion


                weaponCount++;
                break;
            case 12: //-
                /*playerWeaponController.isFrontShield = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);

                upgradeChooseList.upgrades[40].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[40].UpgradeCount;
                upgradeChooseList.upgrades[41].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[41].UpgradeCount;
                upgradeChooseList.upgrades[13].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[13].UpgradeCount;

                //upgradeChooseList.weaponUpgrades[40].reqSupport = 1; // Fortified Defense
                //upgradeChooseList.weaponUpgrades[41].reqSupport = 1; // Shielded Strike
                //upgradeChooseList.weaponUpgrades[13].reqSupport = 2; // Back Shield
                GoBackToDimension();
                weaponCount++;*/
                break;
            case 13: //-
                /*playerWeaponController.isBackShield = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);

                GoBackToDimension();
                weaponCount++;*/
                break;
            case 14: //weapon: schock nova
                playerWeaponController.isNovaExplosion = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();

                upgradeChooseList.upgrades[77].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[77].UpgradeCount;
                upgradeChooseList.upgrades[78].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[78].UpgradeCount;
                //upgradeChooseList.upgrades[36].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[36].UpgradeCount;
                upgradeChooseList.upgrades[79].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[79].UpgradeCount;

                //upgradeChooseList.weaponUpgrades[77].reqRocket = 2; // Fury Nova
                //upgradeChooseList.weaponUpgrades[78].reqRocket = 2; // Nuclear Nova
                //upgradeChooseList.weaponUpgrades[36].reqRocket = 2; // Boosted Nova Impact
                //upgradeChooseList.weaponUpgrades[79].reqRocket = 3; // Power Nova
                weaponCount++;
                break;
            case 15: //weapon: rocket wings
                playerWeaponController.isRockedWings = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();
                weaponCount++;
                break;
            case 16: //weapon: front laser
                playerWeaponController.isFrontLaser = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();
                weaponCount++;
                break;
            case 17: //weapon: orbital laser
                playerWeaponController.isOrbitalLaser = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();

                upgradeChooseList.upgrades[56].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[56].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[56].reqLaser = 2; // Laser Orbit Accelerator

                weaponCount++;
                break;
            case 18: //bullet class
                UpdateClass(number, 1);
                break;
            case 19: //rocked class
                UpdateClass(number, 1);
                break;
            case 20: //laser class
                UpdateClass(number, 1);
                break;
            case 21: //support class
                UpdateClass(number, 1);
                break;
            case 22: //weapon: Backfire Beam
                playerWeaponController.isBackfireBeam = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();
                weaponCount++;

                upgradeChooseList.upgrades[83].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[83].UpgradeCount;
                upgradeChooseList.upgrades[84].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[84].UpgradeCount;
                upgradeChooseList.upgrades[85].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[85].UpgradeCount;
                break;
            case 23: // Guardian Drive
                playerWeaponController.shipData.boostSize = playerWeaponController.shipData.boostSize * 1.6f;
                break;
            case 24: // Force Multiplier
                playerWeaponController.shipData.boostDamage = playerWeaponController.shipData.boostDamage * 2;

                if (upgradeChooseList.upgrades[24].upgradeIndexInstalled == 0)
                {
                    upgradeChooseList.upgrades[25].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[25].UpgradeCount;
                    upgradeChooseList.upgrades[42].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[42].UpgradeCount;
                    upgradeChooseList.upgrades[61].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[61].UpgradeCount;
                }
                break;
            case 25: // Nova Overdrive
                upgradeChooseList.upgrades[42].upgradeStartCount = 0;
                upgradeChooseList.upgrades[61].upgradeStartCount = 0;
                break;

            case 26: // crit damage
                playerWeaponController.shipData.bulletCritDamage += 7;
                playerWeaponController.WeaponChoose();
                break;
            case 27: // crit chance
                playerWeaponController.shipData.bulletCritChance += 3;
                playerWeaponController.WeaponChoose();
                break;
            case 28: // explosion range
                playerWeaponController.shipData.rocketAOERadius += 8;
                playerWeaponController.WeaponChoose();
                break;
            case 29: // burning tick damage
                playerWeaponController.shipData.laserBurningTickDamangePercent += 20;
                playerWeaponController.WeaponChoose();
                break;
            case 30: // burning chance
                playerWeaponController.shipData.burnDamageChance += 1;
                playerWeaponController.WeaponChoose();
                break;
            case 31: // invulnerability
                playerWeaponController.shipData.boostInvulnerability += 0.2f;
                break;
            case 32: // chance to get douple exp
                playerWeaponController.shipData.chanceToGetTwoExp += 10;
                break;
            case 33: // 1 life to collect exp
                playerWeaponController.shipData.chanceToGetHealth += 2;
                break;
            case 34: // rocket life time
                playerWeaponController.shipData.rocketLifeTime += 0.2f;
                playerWeaponController.WeaponChoose();
                break;
            case 35: // Chance to trigger a Nova if u get hit
                     // upgradeChooseList.weaponIndexInstalled[number] += 1;
                break;
            case 36: // Rocket Overdrive
                playerWeaponController.shipData.extraRockets += 4;
                break;
            case 37: // Bullet crit chance +10%
                playerWeaponController.shipData.bulletCritChance += 10;
                playerWeaponController.WeaponChoose();
                break;
            case 38: // Extended Blast Expansion
                playerWeaponController.shipData.rocketAOERadius += 15;
                playerWeaponController.WeaponChoose();
                break;
            case 39: // Ignition Augment
                playerWeaponController.shipData.baseLaserTickDamage += 3;
                playerWeaponController.WeaponChoose();
                break;
            case 40: // Fortified Defense
                playerWeaponController.shipData.shieldHealth += 1;
                //playerWeaponController.UpdateWeaponValues();

                //upgradeChooseList.upgrades[41].upgradeStartCount = 0;
                //upgradeChooseList.upgrades[42].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[42].UpgradeCount; 
                //upgradeChooseList.weaponUpgrades[41].reqSupport = 99; // Shielded Strike
                //upgradeChooseList.weaponUpgrades[42].reqSupport = 2; // Lifeflow Shields
                break;
            case 41: // Shielded Strike
                playerWeaponController.shipData.shieldDamage += 5;
                //playerWeaponController.UpdateWeaponValues();

                //upgradeChooseList.upgrades[40].upgradeStartCount = 0;
                //upgradeChooseList.upgrades[61].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[61].UpgradeCount; 
                //upgradeChooseList.weaponUpgrades[40].reqSupport = 99; // Fortified Defense
                //upgradeChooseList.weaponUpgrades[61].reqSupport = 2; // Shieldbreaker's Might
                break;
            case 42: // Critical Impact Drive
                upgradeChooseList.upgrades[25].upgradeStartCount = 0;
                upgradeChooseList.upgrades[61].upgradeStartCount = 0;
                break;
            case 43: // Ignition Shield
                upgradeChooseList.upgrades[44].upgradeStartCount = 0;
                upgradeChooseList.upgrades[45].upgradeStartCount = 0;
                break;
            case 44: // Shield Nova
                upgradeChooseList.upgrades[43].upgradeStartCount = 0;
                upgradeChooseList.upgrades[45].upgradeStartCount = 0;
                break;
            case 45: // Shieldbreaker's Might
                upgradeChooseList.upgrades[43].upgradeStartCount = 0;
                upgradeChooseList.upgrades[44].upgradeStartCount = 0;
                break;
            case 46: // Ballistic Boost
                playerWeaponController.shipData.percBulletDamage += 12;
                playerWeaponController.UpdateWeaponValues();
                playerWeaponController.UpdateMWDamage(0);
                break;
            case 47: // Boom Boom Boost
                playerWeaponController.shipData.percRocketDamage += 12;
                playerWeaponController.UpdateWeaponValues();
                playerWeaponController.UpdateMWDamage(0);
                break;
            case 48: // Beam Boost
                playerWeaponController.shipData.percLaserDamage += 12;
                playerWeaponController.UpdateWeaponValues();
                playerWeaponController.UpdateMWDamage(0);
                break;
            case 49: // Rapid Laser Reload
                playerWeaponController.shipData.percMainDownTimeLaser += 10;
                playerWeaponController.UpdateMWLaserReloadTime(0.9f);
                break;
            case 50: // Wide Spray Expansion
                upgradeChooseList.upgrades[68].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[68].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[68].reqBullet = 3;  // fast Spread gun
                playerWeaponController.sgBulletCount += 2;
                break;
            case 51: // Lightning Reload
                playerWeaponController.shipData.percMainAttackSpeedBullet += 7;
                playerWeaponController.UpdateMWBulletFireRate(0.93f);
                break;
            case 52: // Kaboomed Targets

                break;
            case 53: // Critical Explosion

                break;
            case 54: // Detonation Crits

                break;
            case 55: // Titan Slayer

                playerWeaponController.shipData.bossBonusDamage = 25;
                break;
            case 56: // Laser Orbit Accelerator
                playerWeaponController.olRotationSpeed = Mathf.Round(playerWeaponController.olRotationSpeed * 1.25f);
                playerWeaponController.olDamage += 2;
                upgradeChooseList.upgrades[80].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[80].UpgradeCount; ;
                //upgradeChooseList.weaponUpgrades[80].reqLaser = 3; // Orbital Laser Array
                playerWeaponController.UpdateWeaponValues();
                break;
            case 57: // Explosive Impact
                playerWeaponController.rlAOERange = Mathf.Round(playerWeaponController.rlAOERange * 1.2f * 10) / 10;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 58: // Luminous Growth
                playerWeaponController.UpdateWeaponValues();
                break;
            case 59: // Engine boost
                playerController.energieProduction = playerController.energieProduction * (1.2f);
                break;
            case 60: // Engine Overload
                playerController.energieProduction = playerController.energieProduction * (1.6f);
                break;
            case 61: // Torchshield Drive
                upgradeChooseList.upgrades[25].upgradeStartCount = 0;
                upgradeChooseList.upgrades[42].upgradeStartCount = 0;
                break;
            case 62: // Explosiv Shield 
                upgradeChooseList.upgrades[43].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[43].UpgradeCount; ;
                upgradeChooseList.upgrades[44].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[44].UpgradeCount;
                upgradeChooseList.upgrades[45].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[45].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[44].reqSupport = 4; // Shield Nova
                //upgradeChooseList.weaponUpgrades[45].reqSupport = 4; // Shieldbreaker's Might
                break;
            case 63: // Natural energie
                playerWeaponController.shipData.chanceToGetFullEnergy += 5;
                break;
            case 64: // Fast Head Cannon
                playerWeaponController.hcBulletDamage = 2;
                playerWeaponController.hcSalveCount = 7;
                playerWeaponController.hcReloadTime = 1f;
                playerWeaponController.UpdateWeaponValues();

                upgradeChooseList.upgrades[65].upgradeStartCount = 0;
                upgradeChooseList.upgrades[67].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[67].UpgradeCount;
                upgradeChooseList.upgrades[76].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[76].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[65].reqBullet = 99; // Big Head Cannon
                //upgradeChooseList.weaponUpgrades[67].reqBullet = 3; // Gatlin Head Cannon
                //upgradeChooseList.weaponUpgrades[76].reqBullet = 4; // Cannon Power
                break;
            case 65: // Big Head Cannon
                playerWeaponController.hcBulletDamage = 12;
                playerWeaponController.hcSalveCount = 2;
                playerWeaponController.hcReloadTime = 3.5f;
                playerWeaponController.UpdateWeaponValues();

                upgradeChooseList.upgrades[64].upgradeStartCount = 0;
                upgradeChooseList.upgrades[66].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[66].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[64].reqBullet = 99; // Fast Head Cannon
                //upgradeChooseList.weaponUpgrades[66].reqBullet = 3; // Sniper Head Cannon
                break;
            case 66: // Sniper Head Cannon
                playerWeaponController.hcBulletDamage = 18;
                playerWeaponController.hcSalveCount = 2;
                playerWeaponController.hcReloadTime = 4f;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 67: // Gatlin Head Cannon
                playerWeaponController.hcReloadTime = 0.5f;
                playerWeaponController.hcSalveCount = 5;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 68: // fast Spread gun
                playerWeaponController.sgReloadTime -= 0.5f;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 69: // therminal Spheres
                upgradeChooseList.upgrades[70].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[70].UpgradeCount;
                upgradeChooseList.upgrades[73].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[73].UpgradeCount;
                upgradeChooseList.upgrades[74].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[74].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[70].reqLaser = 3; // Chaotic Shperes
                //upgradeChooseList.weaponUpgrades[73].reqLaser = 4; // Dynamic Spheres
                //upgradeChooseList.weaponUpgrades[74].reqLaser = 3; // Enhanced Plasma Spheres
                playerWeaponController.isThermalSpheres = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 2);
                GoBackToDimension();
                weaponCount++;
                break;
            case 70: // Chaotic Sunfire Spheres
                upgradeChooseList.upgrades[71].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[71].UpgradeCount;
                upgradeChooseList.upgrades[74].upgradeStartCount = 0;
                upgradeChooseList.upgrades[72].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[72].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[71].reqLaser = 4; // Power Spheres
                //upgradeChooseList.weaponUpgrades[74].reqLaser = 99; // Enhanced Plasma Spheres
                //upgradeChooseList.weaponUpgrades[72].reqLaser = 3; // Creazy Spheres
                playerWeaponController.tsDamage = 2;
                playerWeaponController.tsLifetime = 2f;
                playerWeaponController.tsReloadTime = 0.5f;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 71: // Power Spheres
                playerWeaponController.tsDamage += 2;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 72: // Creazy Spheres
                break;
            case 73: // Dynamic Spheres
                break;
            case 74: // Enhanced Plasma  Spheres
                upgradeChooseList.upgrades[70].upgradeStartCount = 0;
                upgradeChooseList.upgrades[75].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[75].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[70].reqLaser = 99; // Chaotic Shperes
                //upgradeChooseList.weaponUpgrades[75].reqLaser = 4; // Big Photon Sphere
                playerWeaponController.tsLifetime += 1;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 75: // Enhanced Plasma  Spheres
                playerWeaponController.tsLifetime += 3;
                playerWeaponController.tsReloadTime += 0.5f;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 76: // Cannon Power
                playerWeaponController.hcBulletDamage += 1;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 77: // Fury Novas
                upgradeChooseList.upgrades[78].upgradeStartCount = 0;
                //upgradeChooseList.weaponUpgrades[78].reqRocket = 99; // Nuclear Nova
                playerWeaponController.neRadius -= 0.15f;
                playerWeaponController.neReloadTime -= 0.8f;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 78: // Nuclear Novas
                upgradeChooseList.upgrades[77].upgradeStartCount = 0;
                //upgradeChooseList.weaponUpgrades[77].reqRocket = 99; // Fury Nova
                playerWeaponController.neRadius += 0.12f;
                playerWeaponController.neReloadTime += 0.3f;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 79: // Power Nova
                playerWeaponController.neDamage += 2;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 80: // Orbital Laser Array
                playerWeaponController.olCount += 1;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 81: // Tactical Minefield
                playerWeaponController.isMineLayer = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number, 1);
                GoBackToDimension();
                weaponCount++;
                break;
            case 82: // Burning Spheres

                break;
            case 83: // Bloodlust Shurikens
                playerWeaponController.bbKillBeams += 1;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 84: // Dancing Blades
                playerWeaponController.bbMainBeams += 1;
                playerWeaponController.UpdateWeaponValues();
                break;
            case 85: // Blade Nova

                break;
            case 86: // Overheating Rockets
                playerWeaponController.shipData.extraDamage += 1;
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

                upgradeChooseList.upgrades[95].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[95].UpgradeCount;
                upgradeChooseList.upgrades[96].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[96].UpgradeCount;
                upgradeChooseList.upgrades[97].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[97].UpgradeCount;
                break;
            case 95: // Astral Shockwave
                upgradeChooseList.upgrades[96].upgradeStartCount = 0;
                upgradeChooseList.upgrades[97].upgradeStartCount = 0;
                break;
            case 96: // Rising Force
                upgradeChooseList.upgrades[95].upgradeStartCount = 0;
                upgradeChooseList.upgrades[97].upgradeStartCount = 0;
                break;
            case 97: // Enduring Inferno
                upgradeChooseList.upgrades[95].upgradeStartCount = 0;
                upgradeChooseList.upgrades[96].upgradeStartCount = 0;
                break;
            case 98: // Collector
                playerWeaponController.shipData.chanceToGetScrap += 3;
                break;
            case 99: // Lasting Shield
                playerController.timeGetInvulnerability += 0.2f;
                break;

        }
    }

    public void TriggerPanel(int index)
    {
        panelList[0].FadeOut(index);
        panelList[1].FadeOut(index);
        panelList[2].FadeOut(index);
        shipPanalController.FadeOut();
        isTweening = true;

        bkImage.DOFade(0f, 0.8f).SetUpdate(true);
    }

    public void GetUpdate()
    {
        // make the player Invulnerability for few sec
        playerController.GetInvulnerabilityAfterUpdate();

        gameManager.UpgradeGet();
    }

    // update classlevel in player weapon controller
    private void UpdateClass(int number, int factor = 1)
    {
        Upgrade uC = upgradeChooseList.uLObject.upgradeList[number];

        // update mainClass level
        int index = (int)uC.mainClass;

        switch (index)
        {
            case 0:
                if (playerWeaponController.shipData.mcBulletLvl < 8)
                {
                    playerWeaponController.shipData.mcBulletLvl += factor;
                    playerWeaponController.shipData.bulletCritChance += playerWeaponController.shipData.critChance;
                    playerWeaponController.shipData.bulletCritDamage += playerWeaponController.shipData.critDamage;
                }
                break;
            case 1:
                if (playerWeaponController.shipData.mcExplosionLvl < 8)
                {
                    playerWeaponController.shipData.mcExplosionLvl += factor;
                    playerWeaponController.shipData.rocketAOERadius += playerWeaponController.shipData.aoeRange;
                }
                break;
            case 2:
                if (playerWeaponController.shipData.mcLaserLvl < 8)
                {
                    playerWeaponController.shipData.mcLaserLvl += factor;
                    playerWeaponController.shipData.burnDamageChance += playerWeaponController.shipData.burningChance;
                }
                break;
            case 3:
                if (playerWeaponController.shipData.mcSupportLvl < 8)
                {
                    playerWeaponController.shipData.mcSupportLvl += factor;
                    playerWeaponController.shipData.supportReloadTime += playerWeaponController.shipData.realodTime;
                }
                break;
        }
        playerWeaponController.UpdateWeaponValues();
    }

    #endregion

    private void GoBackToDimension()
    {
        gameManager.GoBackDimensionInvoke();
        //gameManager.GoBackDimension();
        AudioManager.Instance.PlaySFX("DimensionSwap");
    }

    private void SetRerollText()
    {
        if (rerolls > 0)
        {
            rerollText.text = $"press R to reroll ({rerolls})";
            rerollImage.SetActive(true);
        }
        else
        {
            rerollText.text = "";
            rerollImage.SetActive(false);
        }
    }
}

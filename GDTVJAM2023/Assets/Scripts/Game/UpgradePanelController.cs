using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using static UpgradeChooseList;


public class UpgradePanelController : MonoBehaviour
{
    [Header("Main Panel")]
    //public UpgradeContainer[] upgradeIndex;
    public List<UpgradPanelIndex> panelList;
    public UpgradeShipPanelController shipPanalController;
    [HideInInspector] public int[] upgradeIndex;
    [HideInInspector] public float[] upgradeValue;
    [HideInInspector] public string[] headerStr;
    [HideInInspector] public string[] descriptionTextStr;
    [HideInInspector] public Color[] headerColor;
    [HideInInspector] public Color[] mainClassColor;
    [HideInInspector] public string[] mainClassStr;
    [HideInInspector] public Sprite[] iconPanel;
    [HideInInspector] public Color[] reqColor;
    [HideInInspector] public string[] reqText;
    [HideInInspector] public Color[] reqColor3;
    [HideInInspector] public string[] reqText3;
    [HideInInspector] public int[] upgradeCount;
    public Image[] classPointsBullets;
    public Image[] classPointsAOE;
    public Image[] classPointsLaser;
    public Image[] classPointsSupport;
    public Image[] classUpImage;
    public Sprite[] classUpSprite;

    [Header("Value Panel")]
    public Image bkImage;
    //public List<Color> classColors;
    public List<Image> classPanels = new List<Image>();

    public List<Image> selectedUpgradePanelList = new List<Image>();
    private int weaponCount = 0;

    // Objects
    private GameManager gameManager;
    private NewPlayerController playerController;
    private PlayerWeaponController playerWeaponController;
    private UpgradeChooseList upgradeChooseList;

    public int selectetPanel;
    private bool isButtonPressed = false;
    public bool isTweening = true;


    void OnEnable()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        //classColors = new List<Color>(gameManager.cCPrefab.classColor);
        playerController = GameObject.FindWithTag("Player").GetComponent<NewPlayerController>();
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();

        upgradeIndex = new int[3];
        upgradeValue = new float[3];
        headerStr = new string[3];
        descriptionTextStr = new string[3];
        mainClassColor = new Color[3];
        mainClassStr = new string[3];
        iconPanel = new Sprite[3];
        headerColor = new Color[3];
        reqColor = new Color[6];
        reqText = new string[6];
        reqColor3 = new Color[3];
        reqText3 = new string[3];
        upgradeCount = new int[3];

        selectetPanel = -1;
        isTweening = true;
        isButtonPressed = true;

        StringLibrary();
        UpdateValuePanel();

        bkImage.DOFade(1f, 0.2f).SetUpdate(true);
    }

    private void Update()
    {
        if (isTweening == false)
        {
            if (Input.GetAxisRaw("Horizontal") <= -0.5 && !isButtonPressed)
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
                UpdateValuePanel();
                UpdateValuePanelOnMouseEnter(selectetPanel);

                isButtonPressed = true;
            }
            else if (Input.GetAxisRaw("Horizontal") >= 0.5 && !isButtonPressed)
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
                UpdateValuePanel();
                UpdateValuePanelOnMouseEnter(selectetPanel);

                isButtonPressed = true;
            }

            if (selectetPanel != -1 && Input.GetAxisRaw("SubmitEnter") >= 0.5 && isButtonPressed == false)
            {
                panelList[selectetPanel].OnMouseDown_();
                isButtonPressed = true;
            }

            if (isButtonPressed == true)
            {
                if (Input.GetAxisRaw("Horizontal") > -0.5 && Input.GetAxisRaw("Horizontal") < 0.5 && Input.GetAxisRaw("SubmitEnter") < 0.5)
                {
                    isButtonPressed = false;
                }
            }
        }
    }



    // String Library
    public void StringLibrary()
    {
        headerColor[0] = new Color(255, 255, 255, 1f);
        headerColor[1] = new Color(255, 255, 255, 1f);
        headerColor[2] = new Color(255, 255, 255, 1f);

        for (int i = 0; i < 3; i++)
        {
            upgradeValue[i] =0f;
            

            int number = gameManager.selectedNumbers_[i];
            Upgrade uC = upgradeChooseList.uLObject.upgradeList[number];

            // calculate values
            switch (number)
            {
                case 0: // life
                    upgradeValue[i] = 2;
                    break;
                case 1: //damage
                    upgradeValue[i] = 1;
                    break;
                case 2: //Protection
                    float normalizedLvl = Mathf.InverseLerp(0, 10, playerController.protectionLvl + 1);
                    float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);

                    upgradeValue[i] = targetPercentage;
                    break;
                case 3: // boost
                    upgradeValue[i] = 15f; // 10%
                    break;
                case 4: // agility
                    upgradeValue[i] = 10f; // 10%
                    break;
                case 5: // pickup range
                    upgradeValue[i] = 0.8f;
                    break;

                default: //weapon select
                    headerColor[i] = gameManager.cCPrefab.classColor[uC.colorIndex];
                    break;
            }
            upgradeIndex[i] = number;
            gameManager.playerData.skillsSpotted[upgradeIndex[i]] = true;

            // set text descriptions
            iconPanel[i] = uC.iconPanel;
            headerStr[i] = uC.headerStr;
            descriptionTextStr[i] = uC.descriptionStr.Replace("XX", upgradeValue[i].ToString());

            // mainClass
            int index = (int)uC.mainClass;
            mainClassStr[i] = System.Enum.GetName(typeof(UpgradeContainer.MainClass), uC.mainClass).ToString();
            mainClassColor[i] = gameManager.cCPrefab.classColor[index];
            Debug.Log(gameManager.cCPrefab.classColor[index]);

            // set requerments
            int count_ = 0;
            reqColor[i] = gameManager.cCPrefab.classColor[8];
            reqText[i] = "";
            reqColor[i+3] = gameManager.cCPrefab.classColor[8];
            reqText[i+3] = "";

            if (uC.reqBullet > 0 )
            {
                reqColor[i] = gameManager.cCPrefab.classColor[0];
                reqText[i] = uC.reqBullet.ToString();
                count_ += 3;
            }
            if (uC.reqRocket > 0)
            {
                reqColor[i+ count_] = gameManager.cCPrefab.classColor[1];
                reqText[i+ count_] = uC.reqRocket.ToString();
                count_ += 3;
            }
            if (uC.reqLaser > 0)
            {
                reqColor[i+ count_] = gameManager.cCPrefab.classColor[2];
                reqText[i+ count_] = uC.reqLaser.ToString();
                count_ += 3;
            }
            if (uC.reqSupport > 0)
            {
                reqColor[i+ count_] = gameManager.cCPrefab.classColor[3];
                reqText[i+ count_] = uC.reqSupport.ToString();
            }

            if (uC.reqAbility != "")
            {
                reqColor3[i] = gameManager.cCPrefab.classColor[uC.colorIndex];
                reqText3[i] = uC.reqAbility;
            }
            else
            {
                reqColor3[i] = gameManager.cCPrefab.classColor[8];
                reqText3[i] = "";
            }

            upgradeCount[i] = uC.UpgradeCount;

            panelList[i].SetDescription();        
        }
    }

    // Update Values in Panel 4
    public void UpdateValuePanel()
    {
        ClassBarFiller();
    }
    
    // Part of the Mouse HoverEvent
    public void UpdateValuePanelOnMouseEnter(int index)
    {
        if (isTweening == false)
        {
            int number = gameManager.selectedNumbers_[index];

            panelList[0].DeselectPanel();
            panelList[1].DeselectPanel();
            panelList[2].DeselectPanel();

            // on mouse exit
            if (selectetPanel != -1)
                panelList[index].SelectPanel();

            // weapon Upgrades
            if ((number > 5 && number <= 17) || number == 69)
            {
                selectedUpgradePanelList[weaponCount].sprite = iconPanel[index];
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
        int number = gameManager.selectedNumbers_[index];
        isTweening = true;
        upgradeChooseList.upgrades[number].upgradeIndexInstalled += 1;

        switch (number)
        {
            case 0: //upgrade: health
                playerController.playerMaxHealth = playerController.playerMaxHealth + Mathf.RoundToInt(upgradeValue[index]);
                playerController.playerCurrentHealth = Mathf.Min(playerController.playerCurrentHealth + Mathf.RoundToInt(upgradeValue[index]), playerController.playerMaxHealth);
                gameManager.UpdateUIPlayerHealth(playerController.playerCurrentHealth, playerController.playerMaxHealth);
                break;
            case 1: //upgrade: main Weapon damage
                playerWeaponController.UpdateMWDamage(Mathf.RoundToInt(upgradeValue[index]));
                break;
            case 2: //upgrade: Protection
                float normalizedLvl = Mathf.InverseLerp(0, 10, playerController.protectionLvl + 1);
                float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);
                playerController.protectionPerc = targetPercentage;
                playerController.protectionLvl ++;
                break;
            case 3: //upgrade: boost
                
                playerController.energieMax = playerController.energieMax * (1 + upgradeValue[index] / 100);
                gameManager.energieSlider.maxValue = playerController.energieMax;
                gameManager.energieSlider.value = playerController.energieMax;
                break;
            case 4: //upgrade: rotate speed
                playerController.UpdateAgility(upgradeValue[index]);
                break;
            case 5: //upgrade: pickup Range
                playerController.pickupRange = playerController.pickupRange + upgradeValue[index];
                break;
            case 6: //weapon: headgun
                playerWeaponController.isHeadCannon = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);
                GoBockToDimension();
                weaponCount++;

                upgradeChooseList.upgrades[64].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[64].UpgradeCount;
                upgradeChooseList.upgrades[65].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[65].UpgradeCount;

                //upgradeChooseList.weaponUpgrades[64].reqBullet = 2; // Fast Head Cannon
                //upgradeChooseList.weaponUpgrades[65].reqBullet = 2; // Big Head Cannon
                break;
            case 7: //weapon: rocket launcher
                playerWeaponController.isRocketLauncher = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);
                GoBockToDimension();
                upgradeChooseList.upgrades[57].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[57].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[57].reqRocket = 3; // Explosive Impact

                weaponCount++;
                break;
            case 8: //weapon: fire flys
                playerWeaponController.isFireFlies = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);
                GoBockToDimension();
                upgradeChooseList.upgrades[58].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[58].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[58].reqBullet = 4; // Explosive Impact

                weaponCount++;
                break;
            case 9: //weapon: bullet wings
                playerWeaponController.isBulletWings = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);
                GoBockToDimension();
                weaponCount++;
                break;
            case 10: //weapon: supoort Modul
                upgradeChooseList.chanceToGetTwoExp += 15;
                playerWeaponController.isLifeModul = true;
                playerWeaponController.WeaponChoose();
                
                UpdateClass(number,1);
                GoBockToDimension();
                weaponCount++;

                upgradeChooseList.upgrades[64].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[64].UpgradeCount;
                upgradeChooseList.upgrades[32].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[32].UpgradeCount;
                upgradeChooseList.upgrades[33].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[33].UpgradeCount;

                //upgradeChooseList.weaponUpgrades[63].reqSupport = 2; // Natural Energy
                //upgradeChooseList.weaponUpgrades[32].reqSupport = 2; // Expansion Fortuity
                //upgradeChooseList.weaponUpgrades[33].reqSupport = 3; // Vitality Infusion

                break;
            case 11: //weapon: spread gun
                playerWeaponController.isSpreadGun = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);
                GoBockToDimension();

                upgradeChooseList.upgrades[50].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[50].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[50].reqBullet = 2;  // Wide Spray Expansion


                weaponCount++;
                break;
            case 12: //weapon: front shield
                playerWeaponController.isFrontShield = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);

                upgradeChooseList.upgrades[40].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[40].UpgradeCount;
                upgradeChooseList.upgrades[41].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[41].UpgradeCount;
                upgradeChooseList.upgrades[13].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[13].UpgradeCount;

                //upgradeChooseList.weaponUpgrades[40].reqSupport = 1; // Fortified Defense
                //upgradeChooseList.weaponUpgrades[41].reqSupport = 1; // Shielded Strike
                //upgradeChooseList.weaponUpgrades[13].reqSupport = 2; // Back Shield
                GoBockToDimension();
                weaponCount++;
                break;
            case 13: //weapon: back shield
                playerWeaponController.isBackShield = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);

                GoBockToDimension();
                weaponCount++;
                break;
            case 14: //weapon: schock nova
                playerWeaponController.isNovaExplosion = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);
                GoBockToDimension();

                upgradeChooseList.upgrades[77].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[77].UpgradeCount;
                upgradeChooseList.upgrades[78].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[78].UpgradeCount;
                upgradeChooseList.upgrades[36].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[36].UpgradeCount;
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
                UpdateClass(number,1);
                GoBockToDimension();
                weaponCount++;
                break;
            case 16: //weapon: front laser
                playerWeaponController.isFrontLaser = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);
                GoBockToDimension();
                weaponCount++;
                break;
            case 17: //weapon: orbital laser
                playerWeaponController.isOrbitalLaser = true;
                playerWeaponController.WeaponChoose();
                UpdateClass(number,1);
                GoBockToDimension();

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
            case 22: //spwarm class - useless
                UpdateClass(number, 1);
                break;
            case 23: //Defense class - useless
                UpdateClass(number, 1);
                break;
            case 24: //Targeting class - useless
                UpdateClass(number, 1);
                break;
            case 25: //Backwards class - useless
                UpdateClass(number, 1);
                break;

            case 26: // crit damage
                upgradeChooseList.baseBulletCritDamage += 7;
                playerWeaponController.WeaponChoose();
                break;
            case 27: // crit chance
                upgradeChooseList.baseBulletCritChance += 3;
                playerWeaponController.WeaponChoose();
                break;
            case 28: // explosion range
                upgradeChooseList.baseRocketAOERadius += 8;
                playerWeaponController.WeaponChoose();
                break;
            case 29: // burning tick damage
                upgradeChooseList.laserBurningTickDamangePercent += 20;
                playerWeaponController.WeaponChoose();
                break;
            case 30: // burning chance
                upgradeChooseList.baseLaserBurnDamageChance += 1;
                playerWeaponController.WeaponChoose();
                break;
            case 31: // invulnerability
                upgradeChooseList.baseBoostInvulnerability += 0.2f;
                break;
            case 32: // chance to get douple exp
                upgradeChooseList.chanceToGetTwoExp += 10;
                break;
            case 33: // 1 life to collect exp
                upgradeChooseList.chanceToGet1Health += 2;
                break;
            case 34: // rocket life time
                upgradeChooseList.rocketLifeTime += 0.2f;
                playerWeaponController.WeaponChoose();
                break;
            case 35: // Chance to trigger a Nova if u get hit
                     // upgradeChooseList.weaponIndexInstalled[number] += 1;
                break;
            case 36: // Chance to trigger a Nova if u dash Enemys

                break;
            case 37: // Bullet crit chance +10%
               
                upgradeChooseList.baseBulletCritChance += 10;
                playerWeaponController.WeaponChoose();
                break;
            case 38: // Extended Blast Expansion
                upgradeChooseList.baseRocketAOERadius += 15;
                playerWeaponController.WeaponChoose();
                break;
            case 39: // Ignition Augment
                upgradeChooseList.baseLaserTickDamage += 3;
                playerWeaponController.WeaponChoose();
                break;
            case 40: // Fortified Defense
                upgradeChooseList.shieldHealth += 1;

                upgradeChooseList.upgrades[41].upgradeStartCount = 0;
                upgradeChooseList.upgrades[42].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[42].UpgradeCount; 
                //upgradeChooseList.weaponUpgrades[41].reqSupport = 99; // Shielded Strike
                //upgradeChooseList.weaponUpgrades[42].reqSupport = 2; // Lifeflow Shields
                break;
            case 41: // Shielded Strike
                upgradeChooseList.shieldDamage += 5;
                upgradeChooseList.upgrades[40].upgradeStartCount = 0;
                upgradeChooseList.upgrades[61].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[61].UpgradeCount; 
                //upgradeChooseList.weaponUpgrades[40].reqSupport = 99; // Fortified Defense
                //upgradeChooseList.weaponUpgrades[61].reqSupport = 2; // Shieldbreaker's Might
                break;
            case 42: // Lifeflow Shields
                upgradeChooseList.upgrades[43].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[43].UpgradeCount;
                //upgradeChooseList.weaponUpgrades[43].reqSupport = 3; // Ignition Shield
                break;
            case 43: // Ignition Shield

                break;
            case 44: // Shield Nova

                break;
            case 45: // Shieldbreaker's Might

                break;
            case 46: // Ballistic Boost
                upgradeChooseList.percBulletDamage += 12;
                playerWeaponController.UpdateWeaponValues();
                playerWeaponController.UpdateMWDamage(0);
                break;
            case 47: // Boom Boom Boost
                upgradeChooseList.percRocketDamage += 15;
                playerWeaponController.UpdateWeaponValues();
                playerWeaponController.UpdateMWDamage(0);
                break;
            case 48: // Beam Boost
                upgradeChooseList.percLaserDamage += 15;
                playerWeaponController.UpdateWeaponValues();
                playerWeaponController.UpdateMWDamage(0);
                break;
            case 49: // Rapid Laser Reload
                playerWeaponController.UpdateMWLaserReloadTime(0.9f);

                break;
            case 50: // Wide Spray Expansion
                upgradeChooseList.upgrades[68].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[68].UpgradeCount; ;
                //upgradeChooseList.weaponUpgrades[68].reqBullet = 3;  // fast Spread gun
                playerWeaponController.sgBulletCount += 2;
                break;
            case 51: // Lightning Reload
                playerWeaponController.UpdateMWBulletFireRate(0.93f);
                break;
            case 52: // Kaboomed Targets
               
                break;
            case 53: // Critical Explosion
          
                break;
            case 54: // Detonation Crits
                
                break;
            case 55: // Titan Slayer
            
                upgradeChooseList.bossBonusDamage = 35;
                break;
            case 56: // Laser Orbit Accelerator
                playerWeaponController.olRotationSpeed = Mathf.Round(playerWeaponController.olRotationSpeed * 1.25f);
                playerWeaponController.olDamage += 1;
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
                playerController.energieProduction = playerController.energieProduction * (1 + (0.2f));
                break;
            case 60: // Engine Overload
                playerController.energieProduction = playerController.energieProduction * (1 + (0.6f));
                break;
            case 61: // Continuous shield 
                upgradeChooseList.upgrades[62].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[62].UpgradeCount; ;
                //upgradeChooseList.weaponUpgrades[62].reqSupport = 3; // Explosiv Shild
                break;
            case 62: // Explosiv Shield 
                upgradeChooseList.upgrades[44].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[44].UpgradeCount; ;
                upgradeChooseList.upgrades[45].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[45].UpgradeCount; ;
                //upgradeChooseList.weaponUpgrades[44].reqSupport = 4; // Shield Nova
                //upgradeChooseList.weaponUpgrades[45].reqSupport = 4; // Shieldbreaker's Might
                break;
            case 63: // Natural energie
                upgradeChooseList.chanceToGetfullEnergy += 5;
                break;
            case 64: // Fast Head Cannon
                playerWeaponController.hcBulletDamage = 2;
                playerWeaponController.hcSalveCount = 7;
                playerWeaponController.hcReloadTime = 1f;
                playerWeaponController.UpdateWeaponValues();

                upgradeChooseList.upgrades[65].upgradeStartCount = 0 ;
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
                GoBockToDimension();
                weaponCount++;
                break;
            case 70: // therminal Spheres
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
                upgradeChooseList.upgrades[77].upgradeStartCount = upgradeChooseList.uLObject.upgradeList[77].UpgradeCount; 
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
                GoBockToDimension();
                weaponCount++;
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
                if (upgradeChooseList.mcBulletLvl < 8)
                {
                    upgradeChooseList.mcBulletLvl += factor;
                    upgradeChooseList.baseBulletCritChance += upgradeChooseList.critChance;
                    upgradeChooseList.baseBulletCritDamage += upgradeChooseList.critDamage;
                }
                break;
            case 1:
                if (upgradeChooseList.mcExplosionLvl < 8)
                {
                    upgradeChooseList.mcExplosionLvl += factor;
                    upgradeChooseList.baseRocketAOERadius += upgradeChooseList.aoeRange;
                }
                break;
            case 2:
                if (upgradeChooseList.mcLaserLvl < 8)
                {
                    upgradeChooseList.mcLaserLvl += factor;
                    upgradeChooseList.baseLaserBurnDamageChance += upgradeChooseList.burningChance;
                }
                break;
            case 3:
                if (upgradeChooseList.mcSupportLvl < 8)
                {
                    upgradeChooseList.mcSupportLvl += factor;
                    upgradeChooseList.baseSupportRealoadTime += upgradeChooseList.supportRealodTime;
                }
                break;
        }
        playerWeaponController.UpdateWeaponValues();
    }

    void ClassBarFiller()
    {
        for(int i=0; i<8 ; i++) //classPointsBullets.Length
        {
            if (!DisplayClassPoints(upgradeChooseList.mcBulletLvl, i))
            { classPointsBullets[i].color = Color.white; /*Debug.Log("Bullets " + i);*/ }

            if (!DisplayClassPoints(upgradeChooseList.mcExplosionLvl, i))
            { classPointsAOE[i].color = Color.white; }

            if (!DisplayClassPoints(upgradeChooseList.mcLaserLvl, i))
            { classPointsLaser[i].color = Color.white; }

            if (!DisplayClassPoints(upgradeChooseList.mcSupportLvl, i))
            { classPointsSupport[i].color = Color.white; }
        }

        if (upgradeChooseList.mcBulletLvl > 0)
        { classUpImage[0].sprite = classUpSprite[0]; }

        if (upgradeChooseList.mcExplosionLvl > 0)
        { classUpImage[1].sprite = classUpSprite[1]; }

        if (upgradeChooseList.mcLaserLvl > 0)
        { classUpImage[2].sprite = classUpSprite[2]; }

        if (upgradeChooseList.mcSupportLvl > 0)
        { classUpImage[3].sprite = classUpSprite[3]; }
    }

    bool DisplayClassPoints(int _class, int pointNumber)
    {
        return ((pointNumber) >= _class);
    }

    private void GoBockToDimension()
    {
        gameManager.GoBackDimension();
        AudioManager.Instance.PlaySFX("DimensionSwap");
    }
}

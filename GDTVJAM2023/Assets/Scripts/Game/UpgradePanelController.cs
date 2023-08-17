using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class UpgradePanelController : MonoBehaviour
{
    [Header("Main Panel")]
    //public UpgradeContainer[] upgradeIndex;
    public List<UpgradPanelIndex> panelList;
    public UpgradeShipPanelController shipPanalController;
    public UpgradeChooseList upgradeList;
    [HideInInspector] public float[] upgradeValue;
    [HideInInspector] public string[] headerStr;
    [HideInInspector] public string[] descriptionTextStr;
    [HideInInspector] public Color[] headerColor;
    [HideInInspector] public Color[] mainClassColor;
    [HideInInspector] public string[] mainClassStr;
    [HideInInspector] public Color[] subClassColor;
    [HideInInspector] public string[] subClassStr;
    [HideInInspector] public Sprite[] iconPanel;
    public Color weaponColor;

    [Header("Value Panel")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI protectionText;
    public TextMeshProUGUI boostText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI pickupText;
    public Image bkImage;
    [HideInInspector] public List<Color> classColors;
    public List<Image> classPanels = new List<Image>();

    public List<Image> selectedUpgradePanelList = new List<Image>();
    private int weaponCount = 0;

    //Objects
    private GameManager gameManager;
    private PlayerController playerController;
    private PlayerWeaponController playerWeaponController;
    private UpgradeChooseList upgradeChooseList;

    public int selectetPanel;
    private bool isButtonPressed = false;
    public bool isTweening = true;

    void OnEnable()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        classColors = new List<Color>(gameManager.globalClassColor);
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();

        upgradeValue = new float[3];
        headerStr = new string[3];
        descriptionTextStr = new string[3];
        mainClassColor = new Color[3];
        mainClassStr = new string[3];
        subClassColor = new Color[3];
        subClassStr = new string[3];
        iconPanel = new Sprite[3];
        headerColor = new Color[3];

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
            if (Input.GetAxisRaw("Horizontal") >= 0.5 && !isButtonPressed)
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
                Debug.Log(selectetPanel);
            }
            else if (Input.GetAxisRaw("Horizontal") <= -0.5 && !isButtonPressed)
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
                Debug.Log(selectetPanel);
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
                    upgradeValue[i] = 1;
                    break;
                case 3: // boost
                    upgradeValue[i] = 0.4f;
                    break;
                case 4: // agility
                    upgradeValue[i] = 0.3f;
                    break;
                case 5: // pickup range
                    upgradeValue[i] = 0.8f;
                    break;

                //----
                case 18: //bullet class  
                    headerColor[i] = gameManager.globalClassColor[0];
                    break;
                case 19: //explosion class
                    headerColor[i] = gameManager.globalClassColor[1];
                    break;
                case 20: //laser class
                    headerColor[i] = gameManager.globalClassColor[2];
                    break;
                case 21: //support class
                    headerColor[i] = gameManager.globalClassColor[3];
                    break;
                case 22: //swarm class
                    headerColor[i] = gameManager.globalClassColor[4];
                    break;
                case 23: //defence class
                    headerColor[i] = gameManager.globalClassColor[5];
                    break;
                case 24: //targeting class
                    headerColor[i] = gameManager.globalClassColor[6];
                    break;
                case 25: //direction class
                    headerColor[i] = gameManager.globalClassColor[7];
                    break;

                //----
                case 26: //crit damage
                    headerColor[i] = gameManager.globalClassColor[0];
                    break;
                case 27: //crit chance
                    headerColor[i] = gameManager.globalClassColor[0];
                    break;
                case 28: //explosion range
                    headerColor[i] = gameManager.globalClassColor[1];
                    break;
                case 29: //burning tick damage
                    headerColor[i] = gameManager.globalClassColor[2];
                    break;
                case 30: //burning chance
                    headerColor[i] = gameManager.globalClassColor[2];
                    break;
                case 31: //invulnerability
                    headerColor[i] = gameManager.globalClassColor[3];
                    break;
                case 32: //chance to get double exp
                    headerColor[i] = gameManager.globalClassColor[3];
                    break;
                case 33: //chance to collect exp
                    headerColor[i] = gameManager.globalClassColor[3];
                    break;
                case 34: // rocket life time
                    headerColor[i] = gameManager.globalClassColor[1];
                    break;


                //----
                default: //weapon select
                    headerColor[i] = weaponColor;
                    break;
            }

            UpgradeContainer uC = upgradeList.weaponUpgrades[number];

            // set text descriptions
            iconPanel[i] = uC.iconPanel;
            headerStr[i] = uC.headerStr;
            descriptionTextStr[i] = uC.descriptionStr.Replace("XX", upgradeValue[i].ToString());

            // mainClass
            int index = (int)uC.mainClass;
            mainClassStr[i] = System.Enum.GetName(typeof(UpgradeContainer.MainClass), uC.mainClass).ToString();
            mainClassColor[i] = classColors[index];

            // subClass
            index = (int)uC.subClass;
            subClassStr[i] = uC.subClass.ToString();
            subClassColor[i] = classColors[index];

            panelList[i].SetDescription();        
        }
    }


    // Update Values in Panel 4
    public void UpdateValuePanel()
    {
  
        // update main weapon valuesText
        lifeText.text = playerController.playerMaxHealth.ToString();
        damageText.text = playerController.playerBulletBaseDamage.ToString();
        protectionText.text = playerController.protectionPerc.ToString() + "%";
        boostText.text = (gameManager.boostSlider.maxValue).ToString() + "s";
        agilityText.text = playerController.rotateSpeed.ToString();
        pickupText.text =playerController.pickupRange.ToString();

        Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        // update class colors
        if (upgradeChooseList.mcBulletLvl > 0) { classPanels[0].color = classColors[0]; } else { classPanels[0].color = defaultColor; }
        if (upgradeChooseList.mcExplosionLvl > 0) { classPanels[1].color = classColors[1]; } else { classPanels[1].color = defaultColor; }
        if (upgradeChooseList.mcLaserLvl > 0) { classPanels[2].color = classColors[2]; } else { classPanels[2].color = defaultColor; }
        if (upgradeChooseList.mcSupportLvl > 0) { classPanels[3].color = classColors[3]; } else { classPanels[3].color = defaultColor; }

        if (upgradeChooseList.scSwarmLvl > 0) { classPanels[4].color = classColors[4]; } else { classPanels[4].color = defaultColor; }
        if (upgradeChooseList.scDefenceLvl > 0) { classPanels[5].color = classColors[5]; } else { classPanels[5].color = defaultColor; }
        if (upgradeChooseList.scTargetingLvl > 0) { classPanels[6].color = classColors[6]; } else { classPanels[6].color = defaultColor; }
        if (upgradeChooseList.scDirectionLvl > 0) { classPanels[7].color = classColors[7]; } else { classPanels[7].color = defaultColor; }

    }
    
    
    // Part of the Mouse HoverEvent
    public void UpdateValuePanelOnMouseEnter(int index)
    {
        int number = gameManager.selectedNumbers_[index];

        panelList[0].DeselectPanel();
        panelList[1].DeselectPanel();
        panelList[2].DeselectPanel();

        // on mouse exit
        if (selectetPanel != -1)
            panelList[index].SelectPanel();

        switch (number)
        {
            case 0:
                lifeText.text = (playerController.playerMaxHealth + upgradeValue[index]).ToString();
                break;
            case 1:
                damageText.text = (playerController.playerBulletBaseDamage + upgradeValue[index]).ToString();
                break;
            case 2:
                float normalizedLvl = Mathf.InverseLerp(0, 10, playerController.protectionLvl + upgradeValue[index]);
                float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);
                protectionText.text = targetPercentage.ToString() + "%";
                break;
            case 3:
                boostText.text = (gameManager.boostSlider.maxValue + upgradeValue[index]).ToString() + "s";
                break;
            case 4:
                agilityText.text = (playerController.rotateSpeed + upgradeValue[index]).ToString();
                break;
            case 5:
                pickupText.text = (playerController.pickupRange + upgradeValue[index]).ToString();
                break;

                //----
            case 18: //bullet class  
                break;
            case 19: //explosion class
                break;
            case 20: //laser class
                break;
            case 21: //support class
                break;
            case 22: //swarm class
                break;
            case 23: //defence class
                break;
            case 24: //targeting class
                break;
            case 25: //direction class
                break;

                //----
            case 26: //crit damage
                break;
            case 27: //crit chance
                break;
            case 28: //explosion range
                break;
            case 29: //burning tick damage
                break;
            case 30: //burning chance
                break;
            case 31: //invulnerability
                break;
            case 32: //chance to get double exp
                break;
            case 33: //chance to collect exp
                break;
            case 34: //chance to collect exp
                break;


            //----
            default: //weapon select
                selectedUpgradePanelList[weaponCount].sprite = iconPanel[index];
                break;
        }

        UpgradeContainer uC = upgradeList.weaponUpgrades[number];


        // update class colors
        int index_ = (int)uC.mainClass;
        if (index_ == 0) { classPanels[0].color = classColors[0]; }
        if (index_ == 1) { classPanels[1].color = classColors[1]; }
        if (index_ == 2) { classPanels[2].color = classColors[2]; }
        if (index_ == 3) { classPanels[3].color = classColors[3]; }

        index_ = (int)uC.subClass;
        if (index_ == 4) { classPanels[4].color = classColors[4]; }
        if (index_ == 5) { classPanels[5].color = classColors[5]; }
        if (index_ == 6) { classPanels[6].color = classColors[6]; }
        if (index_ == 7) { classPanels[7].color = classColors[7]; }
    }


    // Click Event - Choose an Ability
    public void ChooseAValue(int index)
    {
        int number = gameManager.selectedNumbers_[index];
        isTweening = true;
        

        switch (number)
        {
            case 0: //upgrade: health
                playerController.playerMaxHealth = playerController.playerMaxHealth + Mathf.RoundToInt(upgradeValue[index]);
                playerController.playerCurrentHealth = Mathf.Min(playerController.playerCurrentHealth + Mathf.RoundToInt(upgradeValue[index]), playerController.playerMaxHealth);
                gameManager.UpdateUIPlayerHealth(playerController.playerCurrentHealth, playerController.playerMaxHealth);
                break;
            case 1: //upgrade: main Weapon damage
                playerController.playerBulletBaseDamage = playerController.playerBulletBaseDamage + Mathf.RoundToInt(upgradeValue[index]);
                playerController.SetBulletDamage();
                break;
            case 2: //upgrade: Protection
                float normalizedLvl = Mathf.InverseLerp(0, 10, playerController.protectionLvl + upgradeValue[index]);
                float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);

                playerController.protectionPerc = targetPercentage;
                playerController.protectionLvl ++;

                break;
            case 3: //upgrade: boost
                gameManager.boostSlider.maxValue = gameManager.boostSlider.maxValue + upgradeValue[index];
                playerController.boostValue = gameManager.boostSlider.maxValue + upgradeValue[index];
                gameManager.boostSlider.value = gameManager.boostSlider.maxValue;
                break;
            case 4: //upgrade: rotate speed
                playerController.rotateSpeed = playerController.rotateSpeed + upgradeValue[index];
                break;
            case 5: //upgrade: pickup Range
                playerController.pickupRange = playerController.pickupRange + upgradeValue[index];
                break;
            case 6: //weapon: headgun
                playerWeaponController.isHeadCannon = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 7: //weapon: rocket launcher
                playerWeaponController.isRocketLauncher = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 8: //weapon: fire flys
                playerWeaponController.isFireFlies = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 9: //weapon: bullet wings
                playerWeaponController.isBulletWings = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 10: //weapon: life modul
                playerWeaponController.isLifeModul = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 11: //weapon: spread gun
                playerWeaponController.isSpreadGun = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 12: //weapon: front shield
                playerWeaponController.isFrontShield = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 13: //weapon: back shield
                playerWeaponController.isBackShield = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 14: //weapon: schock nova
                playerWeaponController.isNovaExplosion = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 15: //weapon: rocket wings
                playerWeaponController.isRockedWings = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 16: //weapon: front laser
                playerWeaponController.isFrontLaser = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
                weaponCount++;
                break;
            case 17: //weapon: orbital laser
                playerWeaponController.isOrbitalLaser = true;
                upgradeChooseList.weaponIndexInstalled[number] = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number,1);
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
            case 22: //spwarm class
                UpdateClass(number, 1);
                break;
            case 23: //Defense class
                UpdateClass(number, 1);
                break;
            case 24: //Targeting class
                UpdateClass(number, 1);
                break;
            case 25: //Backwards class
                UpdateClass(number, 1);
                break;

            case 26: // crit damage
                upgradeChooseList.baseBulletCritDamage += 12;
                break;
            case 27: // crit chance
                upgradeChooseList.baseBulletCritChance += 3;
                break;
            case 28: // explosion range
                upgradeChooseList.baseRocketAOERadius += 10;
                break;
            case 29: // burning tick damage
                upgradeChooseList.buringChance += 20;
                break;
            case 30: // burning chance
                upgradeChooseList.baseLaserBurnDamageChance += 1;
                break;
            case 31: // invulnerability
                upgradeChooseList.baseBoostInvulnerability += 0.2f;
                break;
            case 32: // chance to get douple exp
                upgradeChooseList.chanceToGetTwoExp += 5;
                break;
            case 33: // 1 life to collect exp
                upgradeChooseList.chanceToGet1Health += 2;
                break;
            case 34: // rocket life time
                upgradeChooseList.rocketLifeTime += 0.2f;
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
        UpgradeContainer uC = upgradeList.weaponUpgrades[number];

        // update mainClass level
        int index = (int)uC.mainClass;
        switch (index)
        {
            case 0:
                upgradeChooseList.mcBulletLvl += factor;
                upgradeChooseList.baseBulletCritChance += upgradeChooseList.critChance;
                upgradeChooseList.baseBulletCritDamage += upgradeChooseList.critDamage;
                break;
            case 1:
                upgradeChooseList.mcExplosionLvl += factor;
                upgradeChooseList.baseRocketAOERadius += upgradeChooseList.aoeRange;
                break;
            case 2:
                upgradeChooseList.mcLaserLvl += factor;
                upgradeChooseList.baseLaserBurnDamageChance += upgradeChooseList.buringChance;
                break;
            case 3:
                upgradeChooseList.mcSupportLvl += factor;
                upgradeChooseList.baseSupportRealoadTime += upgradeChooseList.supportRealodTime;
                break;
        }

        // update subClass level
        index = (int)uC.subClass;
        switch (index)
        {
            case 4:
                upgradeChooseList.scSwarmLvl += factor;
                break;
            case 5:
                upgradeChooseList.scDefenceLvl += factor;
                break;
            case 6:
                upgradeChooseList.scTargetingLvl += factor;
                break;
            case 7:
                upgradeChooseList.scDirectionLvl += factor;
                break;
        }

        playerWeaponController.UpdateWeaponValues();
    }
}

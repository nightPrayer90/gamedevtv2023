using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradePanelController : MonoBehaviour
{
    [Header("Main Panel")]
    public UpgradeContainer[] upgradeIndex;
    public List<UpgradPanelIndex> panelList;
    [HideInInspector] public float[] upgradeValue;
    [HideInInspector] public string[] headerStr;
    [HideInInspector] public string[] descriptionTextStr;
    [HideInInspector] public Color[] mainClassColor;
    [HideInInspector] public string[] mainClassStr;
    [HideInInspector] public Color[] subClassColor;
    [HideInInspector] public string[] subClassStr;
    [HideInInspector] public Sprite[] iconPanel;


    [Header("Value Panel")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rateText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI pickupText;
    public List<Color> classColors = new List<Color>();
    public List<Image> classPanels = new List<Image>();

    public List<Image> selectedUpgradePanelList = new List<Image>();
    private int weaponCount = 0;

    //Objects
    private GameManager gameManager;
    private PlayerController playerController;
    private PlayerWeaponController playerWeaponController;
  


    void OnEnable()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
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
        

        StringLibrary();
        UpdateValuePanel();
    }


  

    // String Library
    public void StringLibrary()
    {
        for (int i = 0; i < 3; i++)
        {
            upgradeValue[i] =0f;
            int number = gameManager.selectedNumbers_[i];

            // calculate values
            switch (number)
            {
                case 0: // life
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(1, 3));
                    break;
                case 1: //damage
                    upgradeValue[i] = 1;
                    break;
                case 2: //fire rate
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(2, 3)) / 100f;
                    break;
                case 3: // speed
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(2, 4) * 5)/100;
                    break;
                case 4: // agility
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(1, 3)) / 10f;
                    break;
                case 5: // pickup range
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(5, 10)) / 10f;
                    break;
            }


            UpgradeContainer uC = upgradeIndex[number];

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
        rateText.text = playerController.playerFireRate.ToString();
        speedText.text = (playerController.speed/100).ToString();
        agilityText.text = playerController.rotateSpeed.ToString();
        pickupText.text =playerController.pickupRange.ToString();

        Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        // update class colors
        if (playerWeaponController.mcBulletLvl > 0) { classPanels[0].color = classColors[0]; } else { classPanels[0].color = defaultColor; }
        if (playerWeaponController.mcExplosionLvl > 0) { classPanels[1].color = classColors[1]; } else { classPanels[1].color = defaultColor; }
        if (playerWeaponController.mcLaserLvl > 0) { classPanels[2].color = classColors[2]; } else { classPanels[2].color = defaultColor; }
        if (playerWeaponController.mcSupportLvl > 0) { classPanels[3].color = classColors[3]; } else { classPanels[3].color = defaultColor; }

        if (playerWeaponController.scSwarmLvl > 0) { classPanels[4].color = classColors[4]; } else { classPanels[4].color = defaultColor; }
        if (playerWeaponController.scDefenceLvl > 0) { classPanels[5].color = classColors[5]; } else { classPanels[5].color = defaultColor; }
        if (playerWeaponController.scTargetingLvl > 0) { classPanels[6].color = classColors[6]; } else { classPanels[6].color = defaultColor; }
        if (playerWeaponController.scBackwardsLvl > 0) { classPanels[7].color = classColors[7]; } else { classPanels[7].color = defaultColor; }

    }
    
    
    // Part of the Mouse HoverEvent
    public void UpdateValuePanelOnMouseEnter(int index)
    {
        int number = gameManager.selectedNumbers_[index];

        switch (number)
        {
            case 0:
                lifeText.text = (playerController.playerMaxHealth + upgradeValue[index]).ToString();
                break;
            case 1:
                damageText.text = (playerController.playerBulletBaseDamage + upgradeValue[index]).ToString();
                break;
            case 2:
                rateText.text = (playerController.playerFireRate - upgradeValue[index]).ToString();
                break;
            case 3:
                speedText.text = ((playerController.speed + upgradeValue[index]) / 100).ToString();
                break;
            case 4:
                agilityText.text = (playerController.rotateSpeed + upgradeValue[index]).ToString();
                break;
            case 5:
                pickupText.text = (playerController.pickupRange + upgradeValue[index]).ToString();
                break;
            default: //weapon select
                selectedUpgradePanelList[weaponCount].sprite = iconPanel[index];
                break;
        }

        UpgradeContainer uC = upgradeIndex[number];


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
            case 2: //upgrade: fire Rate
                playerController.playerFireRate = Mathf.Max(0.05f, playerController.playerFireRate - upgradeValue[index]);
                playerController.SetBulletDamage();
                break;
            case 3: //upgrade: speed
                playerController.speed = playerController.speed + upgradeValue[index];
                break;
            case 4: //upgrade: rotate speed
                playerController.rotateSpeed = playerController.rotateSpeed + upgradeValue[index];
                break;
            case 5: //upgrade: pickup Range
                playerController.pickupRange = playerController.pickupRange + upgradeValue[index];
                break;
            case 6: //weapon: headgun
                playerWeaponController.isHeadCannon = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 7: //weapon: rocket launcher
                playerWeaponController.isRocketLauncher = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 8: //weapon: fire flys
                playerWeaponController.isFireFlies = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 9: //weapon: bullet wings
                playerWeaponController.isBulletWings = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 10: //weapon: life modul
                playerWeaponController.isLifeModul = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 11: //weapon: spread gun
                playerWeaponController.isSpreadGun = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 12: //weapon: front shield
                playerWeaponController.isFrontShield = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 13: //weapon: back shield
                playerWeaponController.isBackShield = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 14: //weapon: schock nova
                playerWeaponController.isNovaExplosion = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 15: //weapon: rocket wings
                playerWeaponController.isRockedWings = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 16: //weapon: front laser
                playerWeaponController.isFrontLaser = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;
            case 17: //weapon: orbital laser
                playerWeaponController.isOrbitalLaser = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                UpdateClass(number);
                weaponCount++;
                break;

        }

        gameManager.UpgradeGet();
    }

    // update classlevel in player weapon controller
    private void UpdateClass(int number)
    {
        UpgradeContainer uC = upgradeIndex[number];

        // update mainClass level
        int index = (int)uC.mainClass;
        switch (index)
        {
            case 0:
                playerWeaponController.mcBulletLvl ++;
                break;
            case 1:
                playerWeaponController.mcExplosionLvl ++;
                break;
            case 2:
                playerWeaponController.mcLaserLvl ++;
                break;
            case 3:
                playerWeaponController.mcSupportLvl ++;
                break;
        }

        // update subClass level
        index = (int)uC.subClass;
        switch (index)
        {
            case 4:
                playerWeaponController.scSwarmLvl ++;
                break;
            case 5:
                playerWeaponController.scDefenceLvl ++;
                break;
            case 6:
                playerWeaponController.scTargetingLvl ++;
                break;
            case 7:
                playerWeaponController.scBackwardsLvl ++;
                break;
        }
    }
}

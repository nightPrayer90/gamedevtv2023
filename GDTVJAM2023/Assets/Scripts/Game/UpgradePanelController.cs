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
    [HideInInspector] public string[] mainClass;
    [HideInInspector] public string[] subClass;
    [HideInInspector] public Sprite[] iconPanel;


    [Header("Value Panel")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rateText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI pickupText;
    /*public Image imFrontShield;
    public Image imBackShield;
    public Image imLifeModul;
    public Image imSpreadGun;
    public Image imHeadCannon;
    public Image imBulletWings;
    public Image imRocketLauncher;
    public Image imFireflies;*/
    //public Color imPanelBaseColor;
    //public Color imPanelUpgradeColor;
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
        mainClass = new string[3];
        subClass = new string[3];
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

            UpgradeContainer uC = upgradeIndex[number];
            
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

            // set text descriptions
            iconPanel[i] = uC.iconPanel;
            headerStr[i] = uC.headerStr;
            descriptionTextStr[i] = uC.descriptionStr.Replace("XX", upgradeValue[i].ToString());
            if (uC.mainClass == 0) mainClass[i] = ""; else mainClass[i] = uC.mainClass.ToString();
            if (uC.subClass == 0) subClass[i] = ""; else subClass[i] = uC.subClass.ToString();

            panelList[i].SetDescription();
        }
    }



    // Update Values in Panel 4
    public void UpdateValuePanel()
    {
        // Update Main Upgrade Text
        lifeText.text = playerController.playerMaxHealth.ToString();
        damageText.text = playerController.playerBulletBaseDamage.ToString();
        rateText.text = playerController.playerFireRate.ToString();
        speedText.text = (playerController.speed/100).ToString();
        agilityText.text = playerController.rotateSpeed.ToString();
        pickupText.text =playerController.pickupRange.ToString();
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
                weaponCount++;
                break;
            case 7: //weapon: rocket launcher
                playerWeaponController.isRocketLauncher = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 8: //weapon: fire flys
                playerWeaponController.isFireFlies = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 9: //weapon: bullet wings
                playerWeaponController.isBulletWings = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 10: //weapon: life modul
                playerWeaponController.isLifeModul = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 11: //weapon: spread gun
                playerWeaponController.isSpreadGun = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 12: //weapon: front shield
                playerWeaponController.isFrontShield = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 13: //weapon: back shield
                playerWeaponController.isBackShield = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 14: //weapon: schock nova
                playerWeaponController.isNovaExplosion = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 15: //weapon: rocket wings
                playerWeaponController.isRockedWings = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 16: //weapon: front laser
                playerWeaponController.isFrontLaser = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;
            case 17: //weapon: orbital laser
                playerWeaponController.isOrbitalLaser = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                weaponCount++;
                break;

        }

        gameManager.UpgradeGet();
     
    }
}

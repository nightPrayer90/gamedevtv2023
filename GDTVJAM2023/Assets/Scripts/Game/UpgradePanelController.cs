using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradePanelController : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController playerController;
    private PlayerWeaponController playerWeaponController;


    [Header("Main Panel")]
    public List<Sprite> abilityPanelList;
    public List<UpgradPanelIndex> panelList;
    [HideInInspector] public float[] upgradeValue;
    [HideInInspector] public string[] headerStr;
    [HideInInspector] public string[] descriptionTextStr;
    [HideInInspector] public Sprite[] iconPanel;


    [Header("Value Panel")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rateText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI pickupText;
    public Image imFrontShield;
    public Image imBackShield;
    public Image imLifeModul;
    public Image imSpreadGun;
    public Image imHeadCannon;
    public Image imBulletWings;
    public Image imRocketLauncher;
    public Image imFireflies;
    public Color imPanelBaseColor;
    public Color imPanelUpgradeColor;


    void OnEnable()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerWeaponController = GameObject.Find("Player").GetComponent<PlayerWeaponController>();

        upgradeValue = new float[3];
        headerStr = new string[3];
        descriptionTextStr = new string[3];
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
            iconPanel[i] = abilityPanelList[number];

            switch (number)
            {
                case 0:
                    headerStr[i] = "Life";
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(1, 3));
                    descriptionTextStr[i] = "Increase your health by " + upgradeValue[i];
                    break;
                case 1:
                    headerStr[i] = "Damage ";
                    upgradeValue[i] = 1;
                    descriptionTextStr[i] = "Increase your main weapon damage by " + upgradeValue[i];
                    break;
                case 2:
                    headerStr[i] = "Fire rate";
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(2, 3)) / 100f;
                    descriptionTextStr[i] = "Increase your main weapon fire rate by  " + upgradeValue[i];
                    break;
                case 3:
                    headerStr[i] = "Speed";
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(2, 4) * 5);
                    descriptionTextStr[i] = "Increase your movement speed by " + (upgradeValue[i] / 100);
                    break;
                case 4:
                    headerStr[i] = "Agility";
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(1, 3)) / 10f;
                    descriptionTextStr[i] = "Increase your agility by " + upgradeValue[i];
                    break;
                case 5:
                    headerStr[i] = "Pickup range";
                    upgradeValue[i] = Mathf.RoundToInt(Random.Range(5, 10)) / 10f;
                    descriptionTextStr[i] = "Increase your pickup range by " + upgradeValue[i];
                    break;
                case 6:
                    headerStr[i] = "Head Cannon";
                    descriptionTextStr[i] = "Fires a targeted volley of shots at nearby enemies.";
                    break;
                case 7:
                    headerStr[i] = "Rocket Launcher";
                    descriptionTextStr[i] = "Fires homing missiles at nearby enemies.";
                    break;
                case 8:
                    headerStr[i] = "Fireflies";
                    descriptionTextStr[i] = "Releases a swarm of slowly spreading projectiles.";
                    break;
                case 9:
                    headerStr[i] = "Bullet Wings";
                    descriptionTextStr[i] = "Fires a volley of spreading projectiles from the wings.";
                    break;
                case 10:
                    headerStr[i] = "Life Modul";
                    descriptionTextStr[i] = "The life module regenerates life over time.";
                    break;
                case 11:
                    headerStr[i] = "Spread gun";
                    descriptionTextStr[i] = "Fires periodic bursts of short-range bullets from the center.";
                    break;
                case 12:
                    headerStr[i] = "Front Shield";
                    descriptionTextStr[i] = "Regenerating shield on the front of the ship.";
                    break;
                case 13:
                    headerStr[i] = "Back Shield";
                    descriptionTextStr[i] = "Regenerating shield on the back of the ship.";
                    break;
            }

            panelList[i].SetDescription();
        }
    }



    // Update Values in Panel 4
    public void UpdateValuePanel()
    {
        lifeText.text = playerController.playerMaxHealth.ToString();
        damageText.text = playerController.playerBulletBaseDamage.ToString();
        rateText.text = playerController.playerFireRate.ToString();
        speedText.text = (playerController.speed/100).ToString();
        agilityText.text = playerController.rotateSpeed.ToString();
        pickupText.text =playerController.pickupRange.ToString();

        if (!playerWeaponController.isHeadCannon) imHeadCannon.color = imPanelBaseColor; 
        else imHeadCannon.color = imPanelUpgradeColor;

        if (!playerWeaponController.isRocketLauncher) imRocketLauncher.color = imPanelBaseColor;
        else imRocketLauncher.color = imPanelUpgradeColor;

        if (!playerWeaponController.isFireFlies) imFireflies.color = imPanelBaseColor;
        else imFireflies.color = imPanelUpgradeColor;

        if (!playerWeaponController.isBulletWings) imBulletWings.color = imPanelBaseColor;
        else imBulletWings.color = imPanelUpgradeColor;

        if (!playerWeaponController.isLifeModul) imLifeModul.color = imPanelBaseColor;
        else imLifeModul.color = imPanelUpgradeColor;

        if (!playerWeaponController.isSpreadGun) imSpreadGun.color = imPanelBaseColor;
        else imSpreadGun.color = imPanelUpgradeColor;

        if (!playerWeaponController.isFrontShield) imFrontShield.color = imPanelBaseColor;
        else imFrontShield.color = imPanelUpgradeColor;

        if (!playerWeaponController.isBackShield) imBackShield.color = imPanelBaseColor;
        else imBackShield.color = imPanelUpgradeColor;
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
            case 6: //weapon: headgun
                if (playerWeaponController.isHeadCannon == false) {  imHeadCannon.color = imPanelUpgradeColor; }
                break;
            case 7: //weapon: Rocket Launcher
                if (playerWeaponController.isRocketLauncher == false) {  imRocketLauncher.color = imPanelUpgradeColor; }
                break;
            case 8: //weapon: Fire Flys
                if (playerWeaponController.isFireFlies == false) {  imFireflies.color = imPanelUpgradeColor; }
                break;
            case 9: //weapon: Bullet Wings
                if (playerWeaponController.isBulletWings == false) {  imBulletWings.color = imPanelUpgradeColor; }
                break;
            case 10: //weapon: Life Modul
                if (playerWeaponController.isLifeModul == false) {  imLifeModul.color = imPanelUpgradeColor; }
                break;
            case 11: //weapon: Spread Gun
                if (playerWeaponController.isSpreadGun == false) {  imSpreadGun.color = imPanelUpgradeColor; }
                break;
            case 12: //weapon: front Shield
                if (playerWeaponController.isFrontShield == false) {  imFrontShield.color = imPanelUpgradeColor; }
                break;
            case 13: //weapon: back shield
                if (playerWeaponController.isBackShield == false) {  imBackShield.color = imPanelUpgradeColor; }
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
            case 1: //upgrade: main Weapon
                playerController.playerBulletBaseDamage = playerController.playerBulletBaseDamage + Mathf.RoundToInt(upgradeValue[index]);
                break;
            case 2: //upgrade: fire Rate
                playerController.playerFireRate = Mathf.Max(0.05f, playerController.playerFireRate - upgradeValue[index]);
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
                break;
            case 7: //weapon: Rocket Launcher
                playerWeaponController.isRocketLauncher = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                break;
            case 8: //weapon: Fire Flys
                playerWeaponController.isFireFlies = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                break;
            case 9: //weapon: Bullet Wings
                playerWeaponController.isBulletWings = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                break;
            case 10: //weapon: Life Modul
                playerWeaponController.isLifeModul = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                break;
            case 11: //weapon: Spread Gun
                playerWeaponController.isSpreadGun = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                break;
            case 12: //weapon: front Shield
                playerWeaponController.isFrontShield = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                break;
            case 13: //weapon: back shield
                playerWeaponController.isBackShield = true;
                playerWeaponController.WeaponChoose();
                gameManager.RemoveValueWeaponList(number);
                break;
        }

        gameManager.UpgradeGet();
    }
}

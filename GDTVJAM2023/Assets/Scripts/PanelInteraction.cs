using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PanelInteraction : MonoBehaviour
{
    private Image panelImage;
    private GameManager gameManager;
    private PlayerController playerController;
    private PlayerWeaponController playerWeaponController;

    [Header("Main Panel")]
    public Color defaultColor;
    public Color hoverColor;
    public int panelIndex;
    public GameObject panelUI;
    public GameObject playerUI;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;
    private float upgradeValue;
    private string headerStr;
    private string descriptionTextStr;

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
    public Sprite spPanelDeselcet;
    public Sprite spPanelSelect;


    void Start()
    {
        panelImage = GetComponent<Image>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerWeaponController = GameObject.Find("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void OnMouseEnter()
    {
        // Farbe des Panels ändern, wenn die Maus über das Panel fährt
        UpdateValuePanelOnMouseEnter(gameManager.selectedNumbers_[panelIndex]);
        //panelImage.color = hoverColor;
        panelImage.sprite = spPanelSelect;

}

    public void OnMouseExit()
    {
        // Zurück zur Standardfarbe wechseln, wenn die Maus das Panel verlässt
        panelImage.sprite = spPanelDeselcet;
        UpdateValuePanel();
    }

    public void OnMouseDown()
    {
        // Aktion ausführen, wenn das Panel angeklickt wird
        Debug.Log("Panel angeklickt!" + panelIndex);

        ChooseAValue(gameManager.selectedNumbers_[panelIndex]);

        gameManager.gameIsPlayed = true;
        panelUI.SetActive(false);
        playerUI.SetActive(true);
        Time.timeScale = 1;
    }

    void OnEnable()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerWeaponController = GameObject.Find("Player").GetComponent<PlayerWeaponController>();

        StringLibrary(gameManager.selectedNumbers_[panelIndex]);

        headerText.text = headerStr;
        descriptionText.text = descriptionTextStr;

        UpdateValuePanel();

    }

    void UpdateValuePanel()
    {
        //if (panelIndex == 0)
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
    }
    
    void ChooseAValue(int number)
    {
        AudioManager.Instance.PlaySFX("WindowOpen");

        switch (number)
        {
            case 0: //upgrade: health
                playerController.playerMaxHealth = playerController.playerMaxHealth + Mathf.RoundToInt(upgradeValue);
                playerController.playerCurrentHealth = Mathf.Min(playerController.playerCurrentHealth + Mathf.RoundToInt(upgradeValue), playerController.playerMaxHealth);
                gameManager.UpdatePlayerHealth();
                break;
            case 1: //upgrade: main Weapon
                playerController.playerBulletBaseDamage = playerController.playerBulletBaseDamage + Mathf.RoundToInt(upgradeValue);
                break;
            case 2: //upgrade: fire Rate
                playerController.playerFireRate = Mathf.Max(0.05f ,playerController.playerFireRate - upgradeValue);
                break;
            case 3: //upgrade: speed
                playerController.speed = playerController.speed + upgradeValue;
                break;
            case 4: //upgrade: rotate speed
                playerController.rotateSpeed = playerController.rotateSpeed + upgradeValue;
                break;
            case 5: //upgrade: pickup Range
                playerController.pickupRange = playerController.pickupRange + upgradeValue;
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
    }

    void UpdateValuePanelOnMouseEnter(int number)
    {
        AudioManager.Instance.PlaySFX("MouseHover");

        switch (number)
        {
            case 0:
                lifeText.text = (playerController.playerMaxHealth + upgradeValue).ToString();
                break;
            case 1:
                damageText.text = (playerController.playerBulletBaseDamage + upgradeValue).ToString();
                break;
            case 2:
                rateText.text = (playerController.playerFireRate - upgradeValue).ToString();
                break;
            case 3:
                speedText.text = ((playerController.speed + upgradeValue) / 100).ToString();
                break;
            case 4:
                agilityText.text = (playerController.rotateSpeed + upgradeValue).ToString();
                break;
            case 5:
                pickupText.text = (playerController.pickupRange + upgradeValue).ToString();
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

    void StringLibrary(int number)
    {
        upgradeValue = 0;
        switch (number)
        {
            case 0:
                headerStr = "Life";
                upgradeValue = Mathf.RoundToInt(Random.Range(1, 3));
                descriptionTextStr = "Increase your health by " + upgradeValue;
                break;
            case 1:
                headerStr = "Damage ";
                upgradeValue = 1;
                descriptionTextStr = "Increase your main weapon damage by " + upgradeValue;
                break;
            case 2:
                headerStr = "Fire rate";
                upgradeValue = Mathf.RoundToInt(Random.Range(1, 2))/100f;
                Debug.Log(upgradeValue);
                descriptionTextStr = "Increase your main weapon fire rate by  " + upgradeValue;
                break;
            case 3:
                headerStr = "Speed";
                upgradeValue = Mathf.RoundToInt(Random.Range(2, 4)*5);
                descriptionTextStr = "Increase your movement speed by " + (upgradeValue/100);
                break;
            case 4:
                headerStr = "Agility";
                upgradeValue = Mathf.RoundToInt(Random.Range(1, 3))/10f;
                descriptionTextStr = "Increase your agility by " + upgradeValue;
                break;
            case 5:
                headerStr = "Pickup range";
                upgradeValue = Mathf.RoundToInt(Random.Range(5, 10))/10f;
                descriptionTextStr = "Increase your pickup range by " + upgradeValue;
                break;
            case 6:
                headerStr = "Head Cannon";
                descriptionTextStr = "Fires a targeted volley of shots at nearby enemies.";
                break;
            case 7:
                headerStr = "Rocket Launcher";
                descriptionTextStr = "Fires homing missiles at nearby enemies.";
                break;
            case 8:
                headerStr = "Fireflies";
                descriptionTextStr = "Releases a swarm of slowly spreading projectiles.";
                break;
            case 9:
                headerStr = "Bullet Wings";
                descriptionTextStr = "Fires a volley of spreading projectiles from the wings.";
                break;
            case 10:
                headerStr = "Life Modul";
                descriptionTextStr = "The life module regenerates life over time.";
                break;
            case 11:
                headerStr = "Spread gun";
                descriptionTextStr = "Fires periodic bursts of short-range bullets from the center.";
                break;
            case 12:
                headerStr = "Front Shield";
                descriptionTextStr = "Regenerating shield on the front of the ship.";
                break;
            case 13:
                headerStr = "Back Shield";
                descriptionTextStr = "Regenerating shield on the back of the ship.";
                break;
        }

    }
}

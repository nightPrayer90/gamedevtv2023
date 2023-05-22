using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PanelInteraction : MonoBehaviour
{
    private Image panelImage;

    [Header("Main Panel")]
    public Color defaultColor;
    public Color hoverColor;
    public int panelIndex;
    public GameObject panelUI;
    public GameObject playerUI;
    private string headerStr;
    private string descriptionTextStr;
    private GameManager gameManager;
    private PlayerController playerController;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;
    private float upgradeValue;

    [Header("Value Panel")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rateText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI pickupText;


    void Start()
    {
        panelImage = GetComponent<Image>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        
    }

    public void OnMouseEnter()
    {
        // Farbe des Panels ändern, wenn die Maus über das Panel fährt
        UpdateValuePanelOnMouseEnter(gameManager.selectedNumbers_[panelIndex]);
        panelImage.color = hoverColor;
        
    }

    public void OnMouseExit()
    {
        // Zurück zur Standardfarbe wechseln, wenn die Maus das Panel verlässt
        panelImage.color = defaultColor;
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
        }       
    }
    
    void ChooseAValue(int number)
    {
        switch (number)
        {
            case 0:
                playerController.playerMaxHealth = playerController.playerMaxHealth + Mathf.RoundToInt(upgradeValue);
                playerController.playerCurrentHealth = Mathf.Min(playerController.playerCurrentHealth + Mathf.RoundToInt(upgradeValue), playerController.playerMaxHealth);
                gameManager.UpdatePlayerHealth();
                break;
            case 1:
                playerController.playerBulletBaseDamage = playerController.playerBulletBaseDamage + Mathf.RoundToInt(upgradeValue);
                break;
            case 2:
                playerController.playerFireRate = Mathf.Max(0.05f ,playerController.playerFireRate - upgradeValue);
                break;
            case 3:
                playerController.speed = playerController.speed + upgradeValue;
                break;
            case 4:
                playerController.rotateSpeed = playerController.rotateSpeed + upgradeValue;
                break;
            case 5:
                playerController.pickupRange = playerController.pickupRange + upgradeValue;
                break;
        }
    }

    void UpdateValuePanelOnMouseEnter(int number)
    {
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

        }

    }
}

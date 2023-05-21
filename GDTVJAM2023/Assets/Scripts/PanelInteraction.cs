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
        panelImage.color = hoverColor;
        
    }

    public void OnMouseExit()
    {
        // Zurück zur Standardfarbe wechseln, wenn die Maus das Panel verlässt
        panelImage.color = defaultColor;
    }

    public void OnMouseDown()
    {
        // Aktion ausführen, wenn das Panel angeklickt wird
        Debug.Log("Panel angeklickt!" + panelIndex);
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
        if (panelIndex == 0)
        {
            lifeText.text = playerController.playerMaxHealth.ToString();
            damageText.text = playerController.playerBulletBaseDamage.ToString();
            rateText.text = playerController.playerFireRate.ToString();
            speedText.text = (playerController.speed/100).ToString();
            agilityText.text = playerController.rotateSpeed.ToString();
            pickupText.text =playerController.pickupRange.ToString();
        }       
    }

    void StringLibrary(int number)
    {
        switch (number)
        {
            case 0:
                headerStr = "Life";
                descriptionTextStr = "0";
                break;
            case 1:
                headerStr = "Damage";
                descriptionTextStr = "1";
                break;
            case 2:
                headerStr = "Fire rate";
                descriptionTextStr = "2";
                break;
            case 3:
                headerStr = "Speed";
                descriptionTextStr = "3";
                break;
            case 4:
                headerStr = "Agility";
                descriptionTextStr = "4";
                break;
            case 5:
                headerStr = "Pickup range";
                descriptionTextStr = "5";
                break;

        }

    }
}

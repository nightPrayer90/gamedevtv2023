using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PanelInteraction : MonoBehaviour
{
    private Image panelImage;
    public Color defaultColor;
    public Color hoverColor;
    public int panelIndex;

    private string headerStr;
    private string descriptionTextStr;

    public GameObject panelUI;
    public GameObject playerUI;
    private GameManager gameManager;

    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;

    

    private void Start()
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

        StringLibrary(gameManager.selectedNumbers_[panelIndex]);

        headerText.text = headerStr;
        descriptionText.text = descriptionTextStr;
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
                headerStr = "Collection range";
                descriptionTextStr = "3";
                break;
            case 4:
                headerStr = "Speed";
                descriptionTextStr = "4";
                break;

        }

    }
}

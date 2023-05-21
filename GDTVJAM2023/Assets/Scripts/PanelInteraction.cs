using UnityEngine;
using UnityEngine.UI;

public class PanelInteraction : MonoBehaviour
{
    private Image panelImage;
    public Color defaultColor;
    public Color hoverColor;
    public int panelIndex;

    public GameObject panelUI;
    public GameObject playerUI;
    private GameManager gameManager;


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


}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;


public class NavigationPanelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI Controls")]
    public TextMeshProUGUI nameText;
    public Image moduleImagePanel;
    public Image modulContentPanel;
    public string btnName;
    public Sprite btnSprite;
    public int btnIndex = 0;

    [Header("Selection Controls")]
    public Color32 baseColor = new Color32(8,57,156,255);
    public Color32 selectionColor = new Color32(124,124,255,255);
    private Selection selectionManager;
    private ModuleStorage moduleStorage;
    private HangarUIController hangarUIController;


    /* **************************************************************************** */
    /* LIFECYCLE------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Lifecycle
    private void Start()
    {
        // find GameObjects
        selectionManager = GameObject.Find("SelectionController").GetComponent<Selection>();
        moduleStorage = GameObject.Find("Ship").GetComponent<ModuleStorage>();
        hangarUIController = selectionManager.gameObject.GetComponent<HangarUIController>();

        // Update the Panel view
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Header
        nameText.text = btnName;

        // Sprite
        moduleImagePanel.sprite = btnSprite;
    }
    #endregion



    /* **************************************************************************** */
    /* INTERFACES------------------------------------------------------------------ */
    /* **************************************************************************** */
    #region Interfaces
    // Handle Mouse over UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.01f, 1.01f, 1.01f), 0.1f);

        modulContentPanel.color = selectionColor;
    }

    // Handle Mouse exit UI
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.05f);

        modulContentPanel.color = baseColor;
    }

    // Handle Mouse klick on UI
    public void OnPointerClick(PointerEventData eventData)
    {
        selectionManager.DeselectAll();
        hangarUIController.HandleDeselect();

        switch (btnIndex)
        {
            case 0: // shop btn
                hangarUIController.GoToShop();
                break;

            case 1: //ship preset 1
                moduleStorage.RemoveAllModule();
                moduleStorage.NewShip();
                moduleStorage.BuildShipFromModuleData();
                moduleStorage.RefreshModulSpheres();
                break;

            case 2: // ship preset 2
                moduleStorage.RemoveAllModule();
                moduleStorage.LoadPreset();
                moduleStorage.BuildShipFromModuleData();
                moduleStorage.RefreshModulSpheres();
                break;
        }

    }
    #endregion
}

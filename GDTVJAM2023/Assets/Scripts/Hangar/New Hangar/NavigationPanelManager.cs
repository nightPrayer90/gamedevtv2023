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
    public ClassColor colorPrefab;

    [Header("Selection Controls")]
    public Color32 baseColor = new Color32(8, 57, 156, 255);
    public Color32 selectionColor = new Color32(124, 124, 255, 255);
    private HangarSelection selectionManager;
    private ModuleStorage moduleStorage;
    private HangarUIController hangarUIController;
    public HangarRotateShips hangarRotateShips;


    /* **************************************************************************** */
    /* LIFECYCLE------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Lifecycle
    private void Start()
    {
        // find GameObjects
        selectionManager = GameObject.Find("SelectionController").GetComponent<HangarSelection>();
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
        if (moduleImagePanel != null)
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
        AudioManager.Instance.PlaySFX("MouseHover");
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
                AudioManager.Instance.PlaySFX("MouseKlick");
                break;

            case 1: //new ship
                moduleStorage.RemoveAllModule();
                moduleStorage.NewShip();
                moduleStorage.BuildShipFromModuleData();
                moduleStorage.BuildModuleGrid();
                moduleStorage.RefreshModulSpheres();
                hangarUIController.SetShipPanel();
                AudioManager.Instance.PlaySFX("HangarNewShip");
                break;

            case 2: // ship preset 2
                moduleStorage.RemoveAllModule();
                moduleStorage.LoadPreset();
                moduleStorage.BuildShipFromModuleData();
                moduleStorage.BuildModuleGrid();
                moduleStorage.RefreshModulSpheres();
                hangarUIController.SetShipPanel();
                AudioManager.Instance.PlaySFX("HangarNewShip");
                break;

            case 3: // skillboard btn
                hangarUIController.GoToSkillBoard();
                AudioManager.Instance.PlaySFX("MouseKlick");
                break;

            case 4: // preset 1
                if (hangarRotateShips.rotationState != 0)
                {
                    hangarRotateShips.RotateShip(1, true);
                    ChangeShip(0);
                    AudioManager.Instance.PlaySFX("HangarBuild1");
                }
                break;
            case 5: // preset 2
                if (hangarRotateShips.rotationState != 90)
                {
                    hangarRotateShips.RotateShip(2, true);
                    ChangeShip(1);
                    AudioManager.Instance.PlaySFX("HangarBuild2");
                }
                break;
            case 6: // preset 3
                if (hangarRotateShips.rotationState != 180)
                {
                    hangarRotateShips.RotateShip(3, true);
                    ChangeShip(2);
                    AudioManager.Instance.PlaySFX("HangarBuild3");
                }
                break;
            case 7: // preset 4
                if (hangarRotateShips.rotationState != 270)
                {
                    hangarRotateShips.RotateShip(4, true);
                    ChangeShip(3);
                    AudioManager.Instance.PlaySFX("HangarBuild4");
                }
                break;
        }

    }

    public void ChangeShip(int activShip)
    {
        moduleStorage.SetShipToPlayerData();
        moduleStorage.RemoveAllModule();
        moduleStorage.playerData.ActiveShip = activShip;
        moduleStorage.LoadShipFromPreset();
    }

    public void ChangeBtnColor()
    {
        nameText.color = colorPrefab.classColor[12];
    }

    #endregion
}

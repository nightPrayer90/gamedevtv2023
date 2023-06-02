using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradPanelIndex : MonoBehaviour
{
    [Header("Panel Settings")]
    public int index;
    public Image panelImage;

    [Header("Panel Values")]
    public UpgradePanelController upgradePanelController;
    public Sprite spPanelSelect;
    public Sprite spPanelDeselcet;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;
    public Image iconPanel;

    public void SetDescription()
    { 
        // Build Panel description
        iconPanel.sprite = upgradePanelController.iconPanel[index];
        headerText.text = upgradePanelController.headerStr[index];
        descriptionText.text = upgradePanelController.descriptionTextStr[index];
    }

    public void OnMouseEnter_()
    {
        // Farbe des Panels ändern, wenn die Maus über das Panel fährt
        upgradePanelController.UpdateValuePanelOnMouseEnter(index);
        panelImage.sprite = spPanelSelect;

        AudioManager.Instance.PlaySFX("MouseHover");
    }

    public void OnMouseExit_()
    {
        // Zurück zur Standardfarbe wechseln, wenn die Maus das Panel verlässt
        panelImage.sprite = spPanelDeselcet;
        upgradePanelController.UpdateValuePanel();
    }

    public void OnMouseDown_()
    {
        upgradePanelController.ChooseAValue(index);
        AudioManager.Instance.PlaySFX("WindowOpen");
    }
}

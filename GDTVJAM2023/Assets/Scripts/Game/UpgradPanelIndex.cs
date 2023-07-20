using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;


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
    public TextMeshProUGUI mainClass;
    public TextMeshProUGUI subClass;
    private bool isTweening = true;
    public bool isSelected = false;


    private void OnEnable()
    {
        isTweening = true;
        transform.position = new Vector3(transform.position.x, transform.position.y + 400f, transform.position.z);
      
        // fade in
        panelImage.sprite = spPanelDeselcet;
        transform.DOLocalMoveY(55f, .22f, true).SetUpdate(UpdateType.Normal, true).OnComplete(() =>
        {
            transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 5, 1).SetUpdate(true).OnComplete(() =>
            {
                upgradePanelController.isTweening = false;
            });

            AudioManager.Instance.PlaySFX("MouseKlick");
            isTweening = false;
            
        });
    }


    public void SetDescription()
    { 
        // Build Panel description
        iconPanel.sprite = upgradePanelController.iconPanel[index];
        headerText.text = upgradePanelController.headerStr[index];
        descriptionText.text = upgradePanelController.descriptionTextStr[index];

        mainClass.text = upgradePanelController.mainClassStr[index];
        mainClass.color = upgradePanelController.mainClassColor[index];
        subClass.text = upgradePanelController.subClassStr[index];
        subClass.color = upgradePanelController.subClassColor[index];
        
    }



    public void OnMouseEnter_()
    {
        // Farbe des Panels ändern, wenn die Maus über das Panel fährt
        upgradePanelController.selectetPanel = index;
        upgradePanelController.UpdateValuePanelOnMouseEnter(index);
        //SelectPanel();
        
    }

    public void SelectPanel()
    {
        isSelected = true;
        panelImage.sprite = spPanelSelect;

        AudioManager.Instance.PlaySFX("MouseHover");

        if (isTweening == false)
        {
            transform.DOComplete();
            transform.DOKill();
            transform.DOPunchScale(new Vector3(0.08f, 0.08f, 0.08f), 0.08f, 5, 1).SetUpdate(true);
        }
    }

    public void OnMouseExit_()
    {
        if (isTweening == false)
        {
            // Zurück zur Standardfarbe wechseln, wenn die Maus das Panel verlässt
            upgradePanelController.selectetPanel = -1;
            upgradePanelController.UpdateValuePanel();
            DeselectPanel();
        }
    }

    public void DeselectPanel()
    {
        isSelected = false;
        panelImage.sprite = spPanelDeselcet;
    }

    public void OnMouseDown_()
    {
        if (isTweening == false)
        {
            if (upgradePanelController.selectetPanel == index)
                panelImage.sprite = spPanelSelect;

            upgradePanelController.ChooseAValue(index);
            AudioManager.Instance.PlaySFX("WindowOpen");

            transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), .2f, 5, 1).SetUpdate(true).OnComplete(() => { upgradePanelController.TriggerPanel(index); });

            isTweening = true;
        }
    }

    public void FadeOut(int index_)
    {
        float duration = (float)index / 15;

        if (upgradePanelController.selectetPanel == index)
            panelImage.sprite = spPanelSelect;

        if (index == index_)
        {
            transform.DOLocalMoveY(855f, .7f, true).SetUpdate(UpdateType.Normal, true).SetEase(Ease.InQuart).OnComplete(() =>
            {
                upgradePanelController.GetUpdate();
            });
        }
        else
        {
            transform.DOLocalMoveY(855f, .5f, true).SetUpdate(UpdateType.Normal, true).SetEase(Ease.InQuart).SetDelay(0.2f + duration);
        }


    }
}


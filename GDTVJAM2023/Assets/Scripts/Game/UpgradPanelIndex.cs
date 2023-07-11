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

    private void OnEnable()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 400f, transform.position.z);
      
        panelImage.sprite = spPanelDeselcet;
        float duration = (float)index / 15;
        transform.DOLocalMoveY(55f, .22f, true).SetUpdate(UpdateType.Normal, true).SetDelay(duration).OnComplete(() =>
        {
            transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 5, 1).SetUpdate(true);
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
        upgradePanelController.UpdateValuePanelOnMouseEnter(index);
        panelImage.sprite = spPanelSelect;

        AudioManager.Instance.PlaySFX("MouseHover");
        if (isTweening == false)
        {
            transform.DOComplete();
            transform.DOPunchScale(new Vector3(0.08f, 0.08f, 0.08f), 0.08f, 5, 1).SetUpdate(true);
       
        } 
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

        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), .2f, 5, 1).SetUpdate(true).OnComplete(() => { upgradePanelController.TriggerPanel(index); } );
    }

    public void FadeOut(int index_)
    {
        float duration = (float)index / 15;

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


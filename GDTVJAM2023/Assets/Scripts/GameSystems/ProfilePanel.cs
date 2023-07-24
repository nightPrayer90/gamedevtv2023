using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ProfilePanel : MonoBehaviour
{
    public int index;
    public Image panel;
    public SaveLoadTest uiController;

    public Sprite hoverSprite;
    public Sprite normalSprite;


    public void OnMouseEnter()
    {
        if (uiController.canChoose == true)
        {
            AudioManager.Instance.PlaySFX("MouseHover");
            panel.sprite = hoverSprite;
            transform.DOPunchScale(new Vector3(0.08f, 0.08f, 0.08f), 0.08f, 5, 1).SetUpdate(true);
        }
    }

    public void OnMouseExit()
    {
        panel.sprite = normalSprite;
        
    }

    public void OnMouseDown()
    {
        if (uiController.canChoose == true)
        {
            uiController.canChoose = false;
            uiController.SetIndex(index);
        }
    }
}


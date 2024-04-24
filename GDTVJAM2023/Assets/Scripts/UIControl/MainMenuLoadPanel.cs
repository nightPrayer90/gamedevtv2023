using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuLoadPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector] public bool canLoad = false;
    public int index;
    public Image borderImage;
    public Sprite mouseOverSprite;
    public Sprite mouseExitSprite;
    public Image districtImage;
    public StartGameController gameController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("MouseHover");
        borderImage.sprite = mouseOverSprite;
        districtImage.color = new Color(districtImage.color.r, districtImage.color.g, districtImage.color.b, 4f/255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderImage.sprite = mouseExitSprite;
        districtImage.color = new Color(districtImage.color.r, districtImage.color.g, districtImage.color.b, 1f/255);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        
        // load game
        if (canLoad == true)
        {
            gameController.LoadGame(index);
        }

        // create a new Game
        else
        {
            gameController.CreateNewGame(index);
        }
    }
}

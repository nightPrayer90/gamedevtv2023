using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{

    public void OnPointerEnter(PointerEventData ped)
    {
        AudioManager.Instance.PlaySFX("MouseHover");
        Debug.Log("test");
    }
    /*void OnMouseEnter()
    {
        AudioManager.Instance.PlaySFX("MouseHover");
        Debug.Log("test");
    }*/
}

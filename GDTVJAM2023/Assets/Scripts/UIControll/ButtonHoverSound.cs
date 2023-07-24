using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{

    public void OnPointerEnter(PointerEventData ped)
    {
        AudioManager.Instance.PlaySFX("MouseHover");
    }
    
}

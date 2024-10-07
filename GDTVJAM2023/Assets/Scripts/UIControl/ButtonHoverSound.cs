using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    [SerializeField] private float scaleMultiplier = 1.1f;
    private bool isSelected = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        rectTransform.localScale = originalScale * scaleMultiplier;
        AudioManager.Instance.PlaySFX("MouseHover");
        
    }

    public void KlickSound()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected == false)
            rectTransform.localScale = originalScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        rectTransform.localScale = originalScale * scaleMultiplier;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        rectTransform.localScale = originalScale;
    }
}

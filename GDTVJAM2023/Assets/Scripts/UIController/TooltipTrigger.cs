using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    private Vector2 triggerPosition;

    public Tooltip tooltip;

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        triggerPosition = transform.position;
        TooltipSystem.Show(tooltip, triggerPosition, content, header);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
}

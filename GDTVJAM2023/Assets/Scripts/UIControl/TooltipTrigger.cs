using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    private Vector2 triggerPosition;

    public Tooltip tooltip;
    private bool needRefresh = false;


    public void OnPointerEnter(PointerEventData eventData)
    {
        triggerPosition = transform.position;
        TooltipSystem.Show(tooltip, triggerPosition, content, header);
        needRefresh = false;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
        needRefresh = false;
    }

    public void Hide()
    {
        TooltipSystem.Hide();
        needRefresh = true;
    }

    public void Refresh()
    {
        if (needRefresh == true)
        {
            triggerPosition = transform.position;
            TooltipSystem.Show(tooltip, triggerPosition, content, header);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    private GameObject go;

    private void Awake()
    {
        go = gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // delay? 
        TooltipSystem.Show(go, content, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
}

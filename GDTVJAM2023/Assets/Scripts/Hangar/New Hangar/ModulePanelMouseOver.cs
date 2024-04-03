using UnityEngine;
using UnityEngine.EventSystems;

public class ModulePanelMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseOverModulePanel = false;
    /* **************************************************************************** */
    /* INTERFACES------------------------------------------------------------------ */
    /* **************************************************************************** */
    #region Interfaces
    // Handle Mouse over UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverModulePanel = true;
    }

    // Handle Mouse exit UI
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverModulePanel = false;
    }
    #endregion
}
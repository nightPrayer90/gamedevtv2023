using UnityEngine;
using UnityEngine.EventSystems;

public class ShopBuyBtnControl : MonoBehaviour, IPointerClickHandler
{
    public ShopModuleMeshHud meshHud;

    public void OnPointerClick(PointerEventData eventData)
    {
        meshHud.BuyModule();
    }
}


using UnityEngine;
using TMPro;

public class ShopModule : MonoBehaviour
{
    private ShopController shopController;
    public int itemIndex = 0;
    public int itemCost = 0;
    [HideInInspector] public int itemSellPrice = 0;
    private ShopModuleMeshHud meshHud;
    private ShopModuleContainer shopModuleContainer;
    private ShopFloatingObject floatingObject;

    private void Awake()
    {
        shopController = GameObject.Find("ShopController").GetComponent<ShopController>();
        //shopController.OnDeselect += HandleSetDeselect;
        itemSellPrice = Mathf.RoundToInt(itemCost * 0.20f);
        meshHud = gameObject.GetComponentInChildren<ShopModuleMeshHud>();
        shopModuleContainer = gameObject.GetComponentInParent<ShopModuleContainer>();
        floatingObject = gameObject.GetComponentInChildren<ShopFloatingObject>();
    }

    private void Start()
    {
        meshHud.SetMeshHuD(shopModuleContainer.isActive);
        if (floatingObject != null)
            floatingObject.SetInActive(shopModuleContainer.isActive);
    }

    /*public void SetActive()
    {
        Debug.Log("isSelected");
    }

    private void HandleSetDeselect()
    {
        Debug.Log("isDeSelected");
    }*/

    public void BuyModule()
    {
        if (meshHud != null)
        {
            meshHud.BuyItem();
        }
    }
}

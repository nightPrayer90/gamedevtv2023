using TMPro;
using UnityEngine;

public class ShopModuleMeshHud : MonoBehaviour
{
    private ShopModule shopModule;
    private ShopController shopController;

    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private TextMeshPro contentText;
    [SerializeField] private TextMeshPro countText;
    [SerializeField] private GameObject buyShpere;
    [SerializeField] private ParticleSystem buyParticle;

    private void Awake()
    {
        shopModule = gameObject.GetComponentInParent<ShopModule>();
        shopController = GameObject.Find("ShopController").GetComponent<ShopController>();
        buyParticle.transform.position = shopModule.transform.position;
    }

    public void BuyItem()
    {
        buyParticle.Play();
        UpdateItemCount();
    }

    private void UpdateItemCount()
    {
        if (shopController.moduleCounts[shopModule.itemIndex] == 0)
            countText.color = Color.grey;
        else
            countText.color = Color.green;
        countText.text = "item count: " + shopController.moduleCounts[shopModule.itemIndex].ToString();
    }

    public void SetMeshHuD(bool canBuy)
    {
        if (canBuy == true)
        {
            buyShpere.SetActive(false);
            headerText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleName + " (" + shopModule.itemCost + " CD)";
            contentText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.modulDescription_multiLineText;
            UpdateItemCount();
        }
        else
        {
            buyShpere.SetActive(true);
            buyShpere.transform.position = shopModule.transform.position;
            headerText.text = "";
            contentText.text = "";
            countText.text = "";
        }
    }
}

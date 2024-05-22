using TMPro;
using UnityEngine;


public class ShopModuleMeshHud : MonoBehaviour
{
    private ShopModule shopModule;
    private ShopController shopController;

    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private TextMeshPro contentText;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private TextMeshPro maxCountText;
    [SerializeField] private TextMeshPro countText;
    [SerializeField] private TextMeshPro creditsText;
    [SerializeField] private GameObject buyShpere;
    [SerializeField] private ParticleSystem buyParticle;

    private void Awake()
    {
        shopModule = gameObject.GetComponentInParent<ShopModule>();
        shopController = GameObject.Find("ShopController").GetComponent<ShopController>();

        ShopFloatingObject meshFO = shopModule.gameObject.GetComponentInChildren<ShopFloatingObject>();
        //buyParticle.transform.position = meshFO.transform.position;

        shopController.onBuyModule += UpdateCreditsColor;
        
    }


    public void BuyItem(int flag)
    {
        if (flag == 0) buyParticle.Play();
        UpdateItemCount();
    }

    private void UpdateItemCount()
    {
        string colorstrStorage = "<color=#FFFFFF>";

        if (shopController.shipModulesPlayerBuyed[shopModule.itemIndex] > 0)
        {
            colorstrStorage = "<color=#2EFEF7>";
        }

        countText.text = colorstrStorage + "<b>in storage: " + shopController.shipModulesPlayerBuyed[shopModule.itemIndex].ToString() + "</b></color>";

        if (shopModule.itemMaxCount == -1)
            maxCountText.text = "Unlimited modules available";
        else
        {
            int inUse = shopController.playerData.moduleCounts[shopModule.itemIndex] + shopController.shipModulesPlayerBuyed[shopModule.itemIndex];
            Color c = new Color(0f, 0f, 0f);

            if (inUse >= shopModule.itemMaxCount)
            {
                c = new Color(maxCountText.color.r, maxCountText.color.g, maxCountText.color.b, 0.1f);
                maxCountText.text = shopModule.itemMaxCount == 1 ? "<color=#8C1B66><b>can only be purchased once</b></color>" : $"<color=#8C1B66><b>can only be purchased {shopModule.itemMaxCount} times</b></color>";
            }
            else
            {
                c = new Color(maxCountText.color.r, maxCountText.color.g, maxCountText.color.b, 1f);
                maxCountText.text = shopModule.itemMaxCount == 1 ? "<b>can only be purchased once</b>" : $"<b>can only be purchased {shopModule.itemMaxCount} times</b>";
            }
            headerText.color = c;
            //maxCountText.color = c;
            countText.color = c;
            contentText.color = c;
            costText.color = c;
            creditsText.color = c;
        }
        UpdateCreditsColor();
    }

    public void UpdateCreditsColor()
    {
        if (shopController.credits < shopModule.itemCost)
        {
            creditsText.color = new Color32(140, 27, 102, 100);
        }
        else
        {
            creditsText.color = Color.white;
        }
    }

    public void SetMeshHuD(bool canBuy)
    {
        if (canBuy == true)
        {
            buyShpere.SetActive(false);
            headerText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleName;
            creditsText.text = $"{shopModule.itemCost} CD";
            contentText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.modulDescription_multiLineText;
            costText.text = $"Mass: {shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.costMass} t   Energy: {shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.costEnergie} TN";
            UpdateItemCount();
        }
        else
        {
            buyShpere.SetActive(true);
            //buyShpere.transform.position = shopModule.transform.position;
            headerText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleName;
            contentText.text = " ";
            creditsText.text = " ";
            maxCountText.text = " ";
            costText.text = " ";
            countText.text = $"<color=\"red\"> Defeat Boss {shopModule.shopModuleContainer.index} to buy.</color>";
        }
    }
}

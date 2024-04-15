using TMPro;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ShopModuleMeshHud : MonoBehaviour
{
    private ShopModule shopModule;
    private ShopController shopController;

    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private TextMeshPro contentText;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private TextMeshPro maxCountText;
    [SerializeField] private TextMeshPro countText;
    [SerializeField] private GameObject buyShpere;
    [SerializeField] private ParticleSystem buyParticle;
    [SerializeField] private ParticleSystem sellParticle;

    private void Awake()
    {
        shopModule = gameObject.GetComponentInParent<ShopModule>();
        shopController = GameObject.Find("ShopController").GetComponent<ShopController>();

        ShopFloatingObject meshFO = shopModule.gameObject.GetComponentInChildren<ShopFloatingObject>();
        buyParticle.transform.position = meshFO.transform.position;
        sellParticle.transform.position = meshFO.transform.position;
    }

    public void BuyItem(int flag)
    {
        if (flag == 0) buyParticle.Play();
        if (flag == 1) sellParticle.Play();
        UpdateItemCount();
    }

    private void UpdateItemCount()
    {
        string colorstrStorage = "<color=#6E6E6E>"; //grey
        string colorstrShip = "<color=#6E6E6E>";
        if (shopController.playerData.moduleCounts[shopModule.itemIndex] > 0)
            colorstrStorage = "<color=#00FF00>"; //green
        if (shopController.shipModulesInUse[shopModule.itemIndex] > 0)
            colorstrShip = "<color=#FF8000>"; // orange

        countText.text = colorstrStorage + "in Storage: " + shopController.playerData.moduleCounts[shopModule.itemIndex].ToString() + "</color>"
            + colorstrShip + "    in ship: " + shopController.shipModulesInUse[shopModule.itemIndex].ToString() + "</color>";

        if (shopModule.itemMaxCount == -1)
            maxCountText.text = "Unlimited modules available";
        else
        {
            int inUse = shopController.playerData.moduleCounts[shopModule.itemIndex] + shopController.shipModulesInUse[shopModule.itemIndex];
            Debug.Log(inUse);
            if (inUse >= shopModule.itemMaxCount)
                maxCountText.text = $"<color=#FF8000>can only be purchased {shopModule.itemMaxCount} times</color>";
            else
                maxCountText.text = $"can only be purchased {shopModule.itemMaxCount} times";
        }
    }

    public void SetMeshHuD(bool canBuy)
    {
        if (canBuy == true)
        {
            buyShpere.SetActive(false);
            headerText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleName + " (" + shopModule.itemCost + " CD)";
            contentText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.modulDescription_multiLineText;
            costText.text = $"Mass: {shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.costMass} t   Energie: {shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.costEnergie} TN";
            UpdateItemCount();
        }
        else
        {
            buyShpere.SetActive(true);
            buyShpere.transform.position = shopModule.transform.position;
            headerText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleName + " (" + shopModule.itemCost + " CD)"; ;
            contentText.text = "";
            maxCountText.text = "";
            costText.text = "";
            countText.text = $"<color=\"red\"> Defeat Boss {shopModule.shopModuleContainer.index} to buy.</color>";
        }
    }
}

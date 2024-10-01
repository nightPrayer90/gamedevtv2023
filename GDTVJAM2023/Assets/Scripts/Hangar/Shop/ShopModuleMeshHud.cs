using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ModuleValues;

public class ShopModuleMeshHud : MonoBehaviour
{
    private ShopModule shopModule;
    private ShopController shopController;

    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private TextMeshPro contentText;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private TextMeshPro maxCountText;
    [SerializeField] private TextMeshPro countText;
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private GameObject buyShpere;
    [SerializeField] private ParticleSystem buyParticle;
    [SerializeField] private CanvasGroup btnGroup;
    [SerializeField] private CanvasGroup upgradeGroup;
    [SerializeField] private Image btnImage;
    [SerializeField] private Transform upgradeIconParent;
    [SerializeField] private GameObject upgradeIconPrefab;

    private void Awake()
    {
        shopModule = gameObject.GetComponentInParent<ShopModule>();
        shopController = GameObject.Find("ShopController").GetComponent<ShopController>();

        ShopFloatingObject meshFO = shopModule.gameObject.GetComponentInChildren<ShopFloatingObject>();
        //buyParticle.transform.position = meshFO.transform.position;

        shopController.onBuyModule += UpdateCreditsColor;
        
    }


    private void UpdateItemCount()
    {
        string colorstrStorage = "<color=#FFFFFF>";

        if (shopController.playerData.moduleCounts[shopModule.itemIndex] > 0)
        {
            colorstrStorage = "<color=#2EFEF7>";
        }

        countText.text = colorstrStorage + "<b>in storage: " + shopController.playerData.moduleCounts[shopModule.itemIndex].ToString() + "</b></color>";


        if (shopModule.itemMaxCount == -1)
            maxCountText.text = "Unlimited modules available";
        else
        {
            int inUse = shopController.playerData.moduleCounts[shopModule.itemIndex] + shopController.shipModulesPlayerBuyed[shopModule.itemIndex];
            Color c = new Color(0f, 0f, 0f);

            if (inUse >= shopModule.itemMaxCount)
            {
                btnImage.enabled = false;
                creditsText.text = "sold out";
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
        if (shopController.credits < shopModule.itemCost || creditsText.text == "sold out")
        {
            btnImage.enabled = false;
            creditsText.color = new Color32(140, 27, 102, 255);
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
            creditsText.text = $"{shopModule.itemCost} Scraps";
            contentText.text = shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.modulDescription_multiLineText;

            if (shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.nion > 0)
            {
                costText.text = $"Nion capacity: {shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.costEnergie}    Mass: {shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.costMass} t";
            }
            else
            {
                costText.text = $"Cost: {shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.costEnergie} Nion    Mass: {shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.costMass} t";
            }
            UpdateItemCount();
            btnGroup.alpha = 1;
            btnGroup.blocksRaycasts = true;


            // upgrades
            if (shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.moduleUpgrades.Count > 0)
            {
                upgradeGroup.alpha = 1;
                foreach (ModuleUpgrade item in shopController.moduleList.moduls[shopModule.itemIndex].moduleValues.moduleUpgrades)
                {
                    GameObject go =  Instantiate(upgradeIconPrefab, upgradeIconParent);
                    IconPrefab iconPrefab = go.GetComponent<IconPrefab>();
                    Color bkColor = shopController.classColor.classColor[shopController.upgradeList.upgradeList[item.moduleUpgradeIndex].colorIndex];
                    bool gray = false;
                    if (creditsText.text == "sold out") gray = true;
                    iconPrefab.SetIcon(item.moduleUpgradeQuantity, shopController.upgradeList.upgradeList[item.moduleUpgradeIndex].iconPanel, bkColor, gray);
                }

            }
            else
            {
                upgradeGroup.alpha = 0;
            }

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
            btnGroup.alpha = 0;
            btnGroup.blocksRaycasts = false;
        }
    }

    public void BuyModule()
    {
        int inUse = shopController.playerData.moduleCounts[shopModule.itemIndex] + shopController.shipModulesPlayerBuyed[shopModule.itemIndex];

        if (inUse < shopModule.itemMaxCount)
        {
            shopController.BuyModuleFromHud(shopModule);
        }
    }

    public void BuyItem(int flag)
    {
        if (flag == 0) buyParticle.Play();
        UpdateItemCount();
    }
}

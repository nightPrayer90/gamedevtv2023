using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ShopController : MonoBehaviour
{
    // Module Management
    public ModuleList moduleList;
    public int credits = 0;
    public int activeLevel = 0;
    public List<ShopModuleContainer> moduleContainers;
    public PlayerData playerData;
    public List<int> shipModulesInUse;

    // Selection
    public Material highligtMaterial;
    public Material selectionMaterial;
    private Material originalMaterial;
    private Material originalMaterialSelectet;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    private Transform lastSelection;
    private ShopModule selectedShopModule;

    // UI Management
    public TextMeshProUGUI creditsText;
    public CanvasGroup buyPanel;
    public TextMeshProUGUI buyBtnText;
    public GameObject buyButton;
    public GameObject sellBtn;
    public TextMeshProUGUI sellBtnText;

    // Events
    public event Action onBuyModule;



    /* **************************************************************************** */
    /* LIFECYCLE------------------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        // create a List with all ModulesCounts that use in the playership
        foreach (int i in playerData.moduleCounts)
        {
            shipModulesInUse.Add(0);
        }
        for (int i = 0; i < playerData.moduleData.Count; i++)
        {
            shipModulesInUse[playerData.moduleData[i].moduleTypeIndex] += 1;
        }

        playerData.shopLevelVisited = playerData.bossLevel;

        // credits
        UpdateCredits(playerData.credits);

        // Level
        activeLevel = playerData.bossLevel;

        // reset
        buyPanel.alpha = 0;
        buyPanel.blocksRaycasts = false;

        // TODO activeLevel LoadFrom SaveData----
        foreach (ShopModuleContainer container in moduleContainers)
        {
            if (activeLevel >= container.index)
                container.isActive = true;
            else
                container.isActive = false;
        }
    }

    private void Start()
    {
        onBuyModule?.Invoke();
    }

    private void Update()
    {
        if (highlight != null && highlight != selection)
        {
            highlight.GetComponent<MeshRenderer>().material = originalMaterial;
            highlight = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // handle mouse over
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;

            if (highlight != selection)
            {
                if (highlight.CompareTag("Selectable"))
                {
                    if (highlight.GetComponent<MeshRenderer>().material != highligtMaterial)
                    {
                        originalMaterial = highlight.GetComponent<MeshRenderer>().material;
                        highlight.GetComponent<MeshRenderer>().material = highligtMaterial;
                    }
                }
                else
                {
                    highlight = null;
                }
            }
        }

        // handle Mouse klick
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Physics.Raycast(ray, out raycastHit))
            {
                selection = raycastHit.transform;

                //ObjectDeselect();
                if (selection != lastSelection || lastSelection == null)
                {
                    // select a part of a ship
                    if (selection.CompareTag("Selectable"))
                    {
                        // deselect the old Object
                        ObjectDeselect();

                        // select the new Object
                        originalMaterialSelectet = originalMaterial;
                        selection.GetComponent<MeshRenderer>().material = selectionMaterial;
                        selectedShopModule = selection.GetComponentInParent<ShopModule>();
                        //selectedShopModule.SetActive();

                        lastSelection = selection;

                        // buy Panel ----
                        buyBtnText.text = "buy for " + selectedShopModule.itemCost.ToString() + " CD";
                        // buy btn
                        buyButton.SetActive(true);

                        int inUse = playerData.moduleCounts[selectedShopModule.itemIndex] + shipModulesInUse[selectedShopModule.itemIndex];
                        if (inUse >= selectedShopModule.itemMaxCount && selectedShopModule.itemMaxCount != -1)
                        {
                            buyButton.SetActive(false);
                            AudioManager.Instance.PlaySFX("HangarCantLoadSelect");
                        }
                        else
                        {
                            AudioManager.Instance.PlaySFX("HangarSelectPart");
                        }


                        // sell btn
                        if (playerData.moduleCounts[selectedShopModule.itemIndex] > 0)
                        {
                            sellBtn.SetActive(true);
                            sellBtnText.text = "sell for " + selectedShopModule.itemSellPrice.ToString() + " CD";
                        }
                        else
                        {
                            sellBtn.SetActive(false);
                        }

                        buyPanel.DOKill();
                        if (buyPanel.alpha < 1)
                        {
                            buyPanel.DOFade(1, 0.2f);
                            
                        }
                        buyPanel.blocksRaycasts = true;
                    }
                    // klick at a free space
                    else
                    {
                        DeselectAll();
                    }
                }
            }


        }
    }

    private void OnDestroy()
    {
        /* playerData.moduleCounts.Clear();

         foreach (int i in moduleCounts)
         {
             playerData.moduleCounts.Add(i);
         }*/

        playerData.credits = credits;

        // Save Data into Player File
        AudioManager.Instance.SavePlayerData();
    }

    /* **************************************************************************** */
    /* SELECTION CONTROL----------------------------------------------------------- */
    /* **************************************************************************** */


    public void DeselectAll()
    {
        if (lastSelection != null)
            lastSelection.GetComponentInChildren<MeshRenderer>().material = originalMaterialSelectet;
        selection = null;
        selectedShopModule = null;
        lastSelection = null;
        //OnDeselect?.Invoke();

        // buy Panel
        buyPanel.DOKill();
        if (buyPanel.alpha > 0)
        {
            buyPanel.blocksRaycasts = false;
            buyPanel.DOFade(0, 0.2f);
        }
    }

    public void ObjectDeselect()
    {
        if (lastSelection != null)
            lastSelection.GetComponentInChildren<MeshRenderer>().material = originalMaterialSelectet;
        //OnDeselect?.Invoke();

        // buy Panel
        buyPanel.DOKill();
        if (buyPanel.alpha > 0)
        {
            buyPanel.blocksRaycasts = false;
            buyPanel.DOFade(0, 0.2f);
        }
    }

    public void UpdateCredits(int credits_)
    {
        credits += credits_;
        creditsText.text = "Credits: " + credits.ToString() + " CD";

        creditsText.gameObject.transform.DOKill();
        creditsText.gameObject.transform.DOShakePosition(0.5f, 5, 25, 90);

        if (credits_ < 0)
        {
            creditsText.color = Color.red;
            creditsText.DOColor(Color.white, 0.7f);
        }
        else
        {
            creditsText.color = Color.green;
            creditsText.DOColor(Color.white, 0.7f);
        }
    }

    /* **************************************************************************** */
    /* BuyPanel-------------------------------------------------------------------- */
    /* **************************************************************************** */
    public void BuyModule()
    {
        if (selectedShopModule != null)
        {
            if (credits >= selectedShopModule.itemCost)
            {
                UpdateCredits(-selectedShopModule.itemCost);
                playerData.moduleCounts[selectedShopModule.itemIndex] += 1;

                selectedShopModule.BuyModule(0);
                AudioManager.Instance.PlaySFX("ShopBuy");
            }
            else
            {
                // ToDo what happents if we have not enough credits
                AudioManager.Instance.PlaySFX("ShopCantBuyModules");
            }
        }

        onBuyModule?.Invoke();
        DeselectAll();
    }

    public void SellModule()
    {
        if (selectedShopModule != null)
        {
            AudioManager.Instance.PlaySFX("ShopSell");
            UpdateCredits(selectedShopModule.itemSellPrice);
            playerData.moduleCounts[selectedShopModule.itemIndex] -= 1;

            selectedShopModule.BuyModule(1);
        }

        onBuyModule?.Invoke();
        DeselectAll();
    }


    /* **************************************************************************** */
    /* Scene Management------------------------------------------------------------ */
    /* **************************************************************************** */
    #region Scene MAnagement
    public void GoBackToHangar()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        AudioManager.Instance.SceneTransition("HangarScene");
        //SceneManager.LoadScene("HangarScene");
    }
    #endregion
}

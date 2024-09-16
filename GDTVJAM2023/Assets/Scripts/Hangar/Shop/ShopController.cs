using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class ShopController : MonoBehaviour
{
    // Module Management
    public ModuleList moduleList;
    public int credits = 0;
    public int activeLevel = 0;
    public List<ShopModuleContainer> moduleContainers;
    public PlayerData playerData;
    public List<int> shipModulesPlayerBuyed;

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
            shipModulesPlayerBuyed.Add(0);
        }


        Debug.Log(playerData.GetActiveShip().Count);

        // Get Moduls from activ Ship
        for (int i = 0; i < playerData.GetActiveShip().Count; i++)
        {
            shipModulesPlayerBuyed[playerData.GetActiveShip()[i].moduleTypeIndex] += 1;
        }
        // Get Modules from buyed module List
        for (int i = 0; i < playerData.moduleCounts.Count; i++)
        {
            shipModulesPlayerBuyed[i] += playerData.moduleCounts[i];
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

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

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
        if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
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
                        buyBtnText.text = "Buy for " + selectedShopModule.itemCost.ToString() + " CD";
                        // buy btn
                        buyButton.SetActive(true);

                        int inUse = playerData.moduleCounts[selectedShopModule.itemIndex] + shipModulesPlayerBuyed[selectedShopModule.itemIndex];
                        if (inUse >= selectedShopModule.itemMaxCount && selectedShopModule.itemMaxCount != -1)
                        {
                            buyButton.SetActive(false);
                            AudioManager.Instance.PlaySFX("HangarCantLoadSelect");
                        }
                        else
                        {
                            AudioManager.Instance.PlaySFX("HangarSelectPart");
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

        // back to Hangar
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBackToHangar();
        }*/
    }

    private void OnDestroy()
    {
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
        creditsText.text = "Scrap: " + credits.ToString() + " CD";

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

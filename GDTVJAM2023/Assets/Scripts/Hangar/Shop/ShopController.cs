using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopController : MonoBehaviour
{
    // Module Management
    public ModuleList moduleList;
    public List<int> moduleCounts;
    public int credits = 5000;
    public int activeLevel = 0;
    public List<ShopModuleContainer> moduleContainers;

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
    public GameObject sellBtn;
    public TextMeshProUGUI sellBtnText;

    // Events
    //public event Action OnDeselect;

    private void Awake()
    {
        // TODO Load from SaveData---------
        int count = moduleList.moduls.Count;
        for (int i = 0; i < count; i++)
        {
            moduleCounts.Add(0);
        }

        // credits
        UpdateCredits(0);

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

                        // buy Panel
                        buyBtnText.text = "buy for " + selectedShopModule.itemCost.ToString() + " CD";
                        if (moduleCounts[selectedShopModule.itemIndex] > 0)
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
                            buyPanel.blocksRaycasts = true;
                        }

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
                moduleCounts[selectedShopModule.itemIndex] += 1;

                selectedShopModule.BuyModule();
            }

        }
        DeselectAll();
    }

    public void SellModule()
    {
        if (selectedShopModule != null)
        {
            UpdateCredits(selectedShopModule.itemSellPrice);
            moduleCounts[selectedShopModule.itemIndex] -= 1;

            selectedShopModule.BuyModule();
        }
        DeselectAll();
    }


    /* **************************************************************************** */
    /* Scene Management------------------------------------------------------------ */
    /* **************************************************************************** */
    #region Scene MAnagement
    public void GoBackToHangar()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("HangarScene");
    }
    #endregion
}

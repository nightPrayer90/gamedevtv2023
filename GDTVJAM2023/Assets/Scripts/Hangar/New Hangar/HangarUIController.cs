using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class HangarUIController : MonoBehaviour
{
    [Header("Scene Management")]
    public string gameScene = "GameScene";
    public string MenueScene = "MenueScene";

    [Header("UI Controls")]
    public CanvasGroup modulePanel;
    public CanvasGroup removePanel;
    public CanvasGroup selectionContentPanel;

    [Header("Selection Content Panel")]
    public TextMeshProUGUI scpHeader;
    public TextMeshProUGUI scpDescription;
    public TextMeshProUGUI scpCostMassValue;
    public TextMeshProUGUI scpCostEnergieValue;

    [Header("Ship Panel")]
    public TextMeshProUGUI spMassValue;
    public TextMeshProUGUI spEnergieProduction;
    public TextMeshProUGUI spEnergieRegen;
    public TextMeshProUGUI spEnergieStorage;
    public TextMeshProUGUI spHealth;
    public TextMeshProUGUI spProtection;
    public TextMeshProUGUI spMainEngine;
    public TextMeshProUGUI spDirectionEngine;
    public TextMeshProUGUI spStrafeEngine;

    [Header("Game Objects")]
    public ModuleStorage moduleStorage;
    public Transform contentParent;
    public GameObject moduleContentPanelPrefab;
    private Selection selectionController;
    private List<GameObject> goContentPanels = new List<GameObject>();

    private void Awake()
    {
        // if the player comes from the Gamescene Time.timeScale = 0;
        Time.timeScale = 1;
    }

    private void Start()
    {
        selectionController = gameObject.GetComponent<Selection>();
        selectionController.OnDeselect += HandleDeselect;
        modulePanel.alpha = 0;
        removePanel.alpha = 0;
        selectionContentPanel.alpha = 0;
    }

    // handle Sphere selection
    public void HandleShpereSelect(Transform selection)
    {
        // cash selectet Sphere Controller
        Sphere sph = selection.gameObject.GetComponent<Sphere>();

            
        // open Panel
        modulePanel.DOKill();
        if (modulePanel.alpha != 1)
        {
            modulePanel.blocksRaycasts = true;
            modulePanel.DOFade(1, 0.2f);
        }
    
        // load Content Panel
        if (sph != null)
        {
            int moduls = sph.availableModuls.Count;
            MeshFilter mRSph = selection.GetComponent<MeshFilter>();
            
            // duplicate Content Moduls
            for (int i = 0; i < moduls; i++)
            {
                GameObject go = Instantiate(moduleContentPanelPrefab);
                ModulContentPanelManager mCPM = go.GetComponent<ModulContentPanelManager>();
                go.transform.SetParent(contentParent);
                go.transform.localScale = new Vector3(1, 1, 1);
                
                mCPM.modulIndex = sph.availableModuls[i];
                mCPM.selectedSphere = mRSph;

                goContentPanels.Add(go);
            }
        }
    }

    // handle Module selection
    public void HandleModulSelect(Transform selection)
    {
        HangarModul selectedModul = selection.GetComponentInParent<HangarModul>();

        // Handle Panel UI
        removePanel.DOKill();
        selectionContentPanel.DOKill();
        if (removePanel.alpha != 1 && selectedModul.hasNoParentControll == false) //TODO hasNoParentControll - cant delete Cockpit or StrafeEngine
        {
            removePanel.blocksRaycasts = true;
            removePanel.DOFade(1, 0.2f);
        }
        if (removePanel.alpha == 1 && selectedModul.hasNoParentControll == true)
        {
            removePanel.blocksRaycasts = false;
            removePanel.DOFade(0, 0.2f);
        }

        if (selectionContentPanel.alpha != 1)
        {
            selectionContentPanel.blocksRaycasts = true;
            selectionContentPanel.DOFade(1, 0.2f);
        }

        // set content selectionPanel
        scpHeader.text = selectedModul.moduleValues.moduleName;
        scpDescription.text = selectedModul.moduleValues.modulDescription_multiLineText;
        scpCostMassValue.text = selectedModul.moduleValues.costMass.ToString() + " t";
        scpCostEnergieValue.text = selectedModul.moduleValues.costEnergie.ToString() + " 1/s";

    }

    // deselect all
    public void HandleDeselect()
    {
        modulePanel.DOFade(0, 0.2f).OnComplete(() => { modulePanel.blocksRaycasts = false; });
        removePanel.DOFade(0, 0.2f).OnComplete(() => { removePanel.blocksRaycasts = false; });
        selectionContentPanel.DOFade(0, 0.2f).OnComplete(() => { selectionContentPanel.blocksRaycasts = true; });

        // delete all Panels
        for (int i = 0; i < goContentPanels.Count; i++)
        {
            Destroy(goContentPanels[i]);
        }
        goContentPanels.Clear();
    }



    /* **************************************************************************** */
    /* SHIP PANEL------------------------------------------------------------------ */
    /* **************************************************************************** */
    #region SHIP PANEL
    public void SetShipPanel()
    {
        float massResult = 0f;
        float energieProductionResult = 0f;
        float energieRegenResult = 0f;
        int energieStorage = 0;
        int health = 0;
        float protection = 0;
        float mainEngine = 0;
        float strafeEngine = 0;
        float directionEngine = 0;

        foreach(HangarModul modul in moduleStorage.installedHangarModules)
        {
            massResult += modul.moduleValues.costMass;
            energieProductionResult += modul.moduleValues.energieProduction;
            energieRegenResult += modul.moduleValues.costEnergie;
            energieStorage += modul.moduleValues.energieStorage;
            health += modul.moduleValues.health;
            protection += modul.moduleValues.protection;
            mainEngine += modul.moduleValues.mainEngine;
            strafeEngine += modul.moduleValues.strafeEngine;
            directionEngine += modul.moduleValues.directionEngine;
        }
        spMassValue.text = massResult.ToString() + " t";
        spEnergieProduction.text = energieProductionResult.ToString() + " TW/s";
        energieRegenResult =   Mathf.Round((energieProductionResult - energieRegenResult)*100)/100;
        spEnergieRegen.text = (energieRegenResult).ToString() + " TW/s"; // TODO: do it red if it is smaller than 0
        spEnergieStorage.text = energieStorage.ToString() + " TW";
        spHealth.text = health.ToString() + " TP";
        spProtection.text = protection.ToString() + " %";
        spMainEngine.text = mainEngine.ToString() + " TJ";
        Debug.Log(directionEngine);
        directionEngine = Mathf.Round((directionEngine / 2) * 100) / 100;
        spDirectionEngine.text = directionEngine.ToString() + " TJ";
        spStrafeEngine.text = strafeEngine.ToString() + " TJ";
        // boost!
        // klassen
    }

    #endregion

    /* **************************************************************************** */
    /* BUTTON CONTOLS-------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Button Controls
    public void GameStart()
    {
        if (moduleStorage.canGameStart == true)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");
            SceneManager.LoadScene(gameScene);
        }
    }
    public void BackToMenue()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene(MenueScene);
    }
    #endregion
}

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
    public CanvasGroup modulPanel;
    public CanvasGroup removePanel;
    public CanvasGroup selectionContentPanel;

    [Header("Selection Content Panel")]
    public TextMeshProUGUI scpHeader;
    public TextMeshProUGUI scpDescription;
    public TextMeshProUGUI scpCostMassValue;
    public TextMeshProUGUI scpCostEnergieValue;

    [Header("Game Objects")]
    public ModuleStorage modulStorage;
    public Transform contentParent;
    public GameObject modulContentPanelPrefab;
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
        modulPanel.alpha = 0;
        removePanel.alpha = 0;
        selectionContentPanel.alpha = 0;
    }

    // handle Sphere selection
    public void HandleShpereSelect(Transform selection)
    {
        // cash selectet Sphere Controller
        Sphere sph = selection.gameObject.GetComponent<Sphere>();

        // open Panel
        modulPanel.DOKill();
        if (modulPanel.alpha != 1)
        {
            modulPanel.blocksRaycasts = true;
            modulPanel.DOFade(1, 0.2f);
        }

        // load Content Panel
        if (sph != null)
        {
            int moduls = sph.availableModuls.Count;
            MeshFilter mRSph = selection.GetComponent<MeshFilter>();

            // duplicate Content Moduls
            for (int i = 0; i < moduls; i++)
            {
                GameObject go = Instantiate(modulContentPanelPrefab);
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
        scpCostMassValue.text = selectedModul.moduleValues.costMass.ToString();
        scpCostEnergieValue.text = selectedModul.moduleValues.costEnergie.ToString();
    }

    // deselect all
    public void HandleDeselect()
    {
        modulPanel.DOFade(0, 0.2f).OnComplete(() => { modulPanel.blocksRaycasts = false; });
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
    /* BUTTON CONTOLS-------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Button Controls
    public void GameStart()
    {
        if (modulStorage.canGameStart == true)
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

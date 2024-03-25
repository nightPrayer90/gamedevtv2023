using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HangarUIController : MonoBehaviour
{
    public string gameScene = "GameScene";
    public CanvasGroup modulPanel;
    public CanvasGroup removePanel;
    private Selection selectionController;
    public GameObject modulContentPanel;
    public Transform contentParent;
    private List<GameObject> goContentPanels = new List<GameObject>();
    public ModuleStorage modulStorage;

    private void Start()
    {
        selectionController = gameObject.GetComponent<Selection>();
        selectionController.OnDeselect += HandleDeselect;
        modulPanel.alpha = 0;
        removePanel.alpha = 0;
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
            int moduls =  sph.availableModuls.Count;
            MeshFilter mRSph = selection.GetComponent<MeshFilter>();

            // duplicate Content Moduls
            for (int i = 0; i < moduls; i++)
            {
                GameObject go = Instantiate(modulContentPanel);
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

        removePanel.DOKill();
        if (removePanel.alpha != 1 && selectedModul.hasNoParentControll == false) //TODO hasNoParentControll - cant delete Cockpit or StrafeEngine
        {
            removePanel.blocksRaycasts = true;
            removePanel.DOFade(1, 0.2f);
        }
    }

    // deselect all
    public void HandleDeselect()
    {
        modulPanel.DOFade(0, 0.2f).OnComplete(() => { modulPanel.blocksRaycasts = false; });
        removePanel.DOFade(0, 0.2f).OnComplete(() => { removePanel.blocksRaycasts = false; });

        // delete all Panels
        for (int i = 0; i < goContentPanels.Count; i++)
        {
            Destroy(goContentPanels[i]);
        }
        goContentPanels.Clear();
    }

    public void GameStart()
    {
        if (modulStorage.canGameStart == true)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");
            SceneManager.LoadScene(gameScene);
        }
    }
}

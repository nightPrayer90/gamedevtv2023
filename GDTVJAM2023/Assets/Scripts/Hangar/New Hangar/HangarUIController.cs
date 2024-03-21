using UnityEngine;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

public class HangarUIController : MonoBehaviour
{
    public CanvasGroup modulPanel;
    public CanvasGroup removePanel;
    private Selection selectionController;
    public GameObject modulContentPanel;
    public Transform contentParent;
    private List<GameObject> goContentPanels = new List<GameObject>();

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

                mCPM.SetText(sph.availableModuls[i].moduleName);
                mCPM.selectedSphere = mRSph;
                mCPM.modulMesh = sph.availableModuls[i].modulMesh;

                // to do set Sprite mCPM.SetText(sph.availableModuls[i].moduleSprite);

                goContentPanels.Add(go);
            }
        }
    }

    // handle Module selection
    public void HandleModulSelect(Transform selection)
    {
        removePanel.DOKill();
        if (removePanel.alpha != 1)
        {
            removePanel.DOFade(1, 0.2f);
        }
    }

    // deselect all
    public void HandleDeselect()
    {
        modulPanel.DOFade(0, 0.2f);
        removePanel.DOFade(0, 0.2f);

        // delete all Panels
        for (int i = 0; i < goContentPanels.Count; i++)
        {
            Destroy(goContentPanels[i]);
        }
        goContentPanels.Clear();
    }
}

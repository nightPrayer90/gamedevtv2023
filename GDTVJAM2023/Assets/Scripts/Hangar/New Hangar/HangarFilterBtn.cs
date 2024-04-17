using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HangarFilterBtn : MonoBehaviour
{
    public ModuleStorage moduleStorage;
    public GameObject moduleContentPanelPrefab;
    public Transform moduleParentTransform;
    private List<ModulContentPanelManager> goContentPanels = new();

    private List<int> cockpits = new();
    private List<int> engines = new();
    private List<int> connectors = new();
    private List<int> weapons = new();
    private List<int> wings = new();

    private Transform contentParent;
    private MeshFilter mRSph;

    public GameObject cockpitBtn;
    public GameObject engineBtn;
    public GameObject connecktorBtn;
    public GameObject weaponBtn;
    public GameObject wingBtn;

    public void BuildLists()
    {
        // reset Lists
        cockpits.Clear();
        engines.Clear();
        connectors.Clear();
        weapons.Clear();
        wings.Clear();

        // build Filter Lists
        foreach (int i in moduleStorage.possibleModules)
        {
            switch (moduleStorage.moduleList.moduls[i].moduleType)
            {
                case ModuleType.Cockpit:
                    cockpits.Add(i);
                    break;
                case ModuleType.DirectionEngine:
                case ModuleType.MainEngine:
                case ModuleType.StrafeEngine:
                    engines.Add(i);
                    break;
                case ModuleType.Connector:
                    connectors.Add(i);
                    break;
                case ModuleType.Weapon:
                    weapons.Add(i);
                    break;
                case ModuleType.Wings:
                    wings.Add(i);
                    break;
            }
        }

        // hide Btn
        if (cockpits.Count > 0) cockpitBtn.SetActive(true); else cockpitBtn.SetActive(false);
        if (engines.Count > 0) engineBtn.SetActive(true); else engineBtn.SetActive(false);
        if (connectors.Count > 0) connecktorBtn.SetActive(true); else connecktorBtn.SetActive(false);
        if (weapons.Count > 0) weaponBtn.SetActive(true); else weaponBtn.SetActive(false);
        if (wings.Count > 0) wingBtn.SetActive(true); else wingBtn.SetActive(false);
    }

    public void CreateFromHangarUIController(Transform contentParent_, MeshFilter mRSph_ = null)
    {
        contentParent = contentParent_;
        mRSph = mRSph_;

        CreateModulePanels(0);
    }

    public void NoFilter()
    { CreateModulePanels(0); }

    public void FilterCockpits()
    { CreateModulePanels(1); }

    public void FilterThruster()
    { CreateModulePanels(2); }

    public void FilterConnectors()
    { CreateModulePanels(3); }

    public void FilterWeapon()
    { CreateModulePanels(4); }

    public void FilterWings()
    { CreateModulePanels(5); }


    public void CreateModulePanels(int btnIndex)
    {
        DeleteAllPanels();
        int modules = 0;
     
        switch (btnIndex)
        {
            case 0: // Filter all
                modules = moduleStorage.possibleModules.Count;
                break;

            case 1: // Cockpits
                modules = cockpits.Count;
                break;

            case 2: // Truster
                modules = engines.Count;
                break;

            case 3: // Connectors
                modules = connectors.Count;
                break;

            case 4: //Weapon
                modules = weapons.Count;
                break;

            case 5: // Wings
                modules = wings.Count;
                break;
        }

        // duplicate Content Moduls
        for (int i = 0; i < modules; i++)
        {
            GameObject go = Instantiate(moduleContentPanelPrefab, contentParent);
            ModulContentPanelManager mCPM = go.GetComponent<ModulContentPanelManager>();

            switch (btnIndex)
            {
                case 0: mCPM.moduleIndex = moduleStorage.possibleModules[i]; break;
                case 1: mCPM.moduleIndex = cockpits[i]; break;
                case 2: mCPM.moduleIndex = engines[i]; break;
                case 3: mCPM.moduleIndex = connectors[i]; break;
                case 4: mCPM.moduleIndex = weapons[i]; break;
                case 5: mCPM.moduleIndex = wings[i]; break;
            }

            mCPM.selectedSphere = mRSph;
            mCPM.parentHangarModuleTransform = moduleParentTransform;
            goContentPanels.Add(mCPM);
        }
    }

    private void DeleteAllPanels()
    {
        // delete all Panels
        for (int i = 0; i < goContentPanels.Count; i++)
        {
            Destroy(goContentPanels[i].gameObject);
        }
        goContentPanels.Clear();
    }

    public void DeactivatePanelInterface()
    {
        for (int i = 0; i < goContentPanels.Count; i++)
        {
            goContentPanels[i].isClickt = true;
        }
    }
}

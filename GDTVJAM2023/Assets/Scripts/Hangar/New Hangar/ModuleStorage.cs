using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public class ModuleDataRuntime : ModuleData
{
    [SerializeField]
    public ModuleDataRuntime parentModule;
    public ModuleDataRuntime()
    {

    }
    public ModuleDataRuntime(ModuleData baseData)
    {
        level = baseData.level;
        x = baseData.x;
        z = baseData.z;
        moduleTypeIndex = baseData.moduleTypeIndex;
    }

    public bool WorkingEquals(object obj)
    {
        if (obj.GetType() == typeof(ModuleDataRuntime))
        {
            ModuleDataRuntime mdr = (ModuleDataRuntime)obj;
            return mdr.x == x && mdr.z == z;
        }
        else
        {
            return base.Equals(obj);
        }
    }
}

[Serializable]
public class ModuleData
{
    public int level;
    public float x;
    public float z;
    public int moduleTypeIndex;
}

public class ModuleStorage : MonoBehaviour
{
    public List<ModuleDataRuntime> installedModuleData;
    JsonDataService dataService;
    public ModuleList moduleList;
    public Transform transformParent;
    private GameObject gameManager;
    private Selection selectionManager;

    public ShipPreset shipPreset;
    // TODO can be replaced by List<HangarModule> to get rid of GetComponent<HangarModule>
    public List<GameObject> installedModulesGameobjects;

    // TODO just temporary
    public bool canGameStart = true;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager");

        installedModuleData = new();
        dataService = new();
        List<ModuleData> loadedModules = dataService.LoadData<List<ModuleData>>("modules.json", false);
        if (loadedModules.Count <= 0)
        {
            foreach (ModuleData item in shipPreset.baseModules)
            {
                installedModuleData.Add(new ModuleDataRuntime(item));
            }
            Debug.Log("LoadPreset");
        }
        else
        {
            foreach (ModuleData item in loadedModules)
            {
                installedModuleData.Add(new ModuleDataRuntime(item));
            }
        }
        foreach (ModuleDataRuntime instance in installedModuleData) {
            // Hangar
            if (gameManager == null)
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleTypeIndex].hangarPrefab, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                installedModulesGameobjects.Add(go);
            }

            // Level
            else
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleTypeIndex].modulePrefabs, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        if (gameManager == null)
        {
            // find selection manager
            selectionManager = GameObject.Find("SelectionController").GetComponent<Selection>();

            // rebuild parents
            RebuildParents();
            
        }
    }

    public void RebuildParents()
    {
        for (int i = 0; i < installedModuleData.Count; i++)
        {
            HangarModul hgm = installedModulesGameobjects[i].GetComponent<HangarModul>();
            // load base data
            hgm.moduleData = new(installedModuleData[i]);
            // get all modules in level below
            foreach (ModuleDataRuntime module in GetModulesByLevel(hgm.moduleData.level - 1))
            {
                // check if one is positioned next to this one
                float x_diff = module.x - hgm.moduleData.x;
                float z_diff = module.z - hgm.moduleData.z;
                if (x_diff >= -1 && x_diff <= 1 && z_diff >= -1 && z_diff <= 1 && Mathf.Abs(x_diff) - Mathf.Abs(z_diff) != 0)
                {
                    // acceptable distance and not a diagonal
                    // will always take the first one as parent
                    // this is not deterministic
                    hgm.moduleData.parentModule = module;
                    installedModuleData[i].parentModule = module;
                }
            }

            // TODO - have the gameObject no Parent - canGameStart = false;
            hgm.ParentControl();
        }
    }

    public void HangarRemoveModule()
    {
        // delete the correct module
        for (int i = 0; i < installedModulesGameobjects.Count; i++)
        {
            HangarModul hgm = installedModulesGameobjects[i].GetComponent<HangarModul>();
            if (hgm.isSelected == true)
            {
                // clean up depending modules
                //List<ModuleDataRuntime> children = GetModulesByLevel(hgm.moduleData.level + 1);
                //canGameStart = true;
                /*foreach (ModuleDataRuntime child in children)
                {
                    // TODO: children of children
                    if (child.parentModule == null)
                    {
                        break;
                    }
                    /*if (child.parentModule.WorkingEquals(hgM.moduleData))
                    {
                        Debug.Log(child.moduleTypeIndex);   
                    }
                }*/
                
                // destroy gameObject
                Destroy(installedModulesGameobjects[i]);

                // delete GameObject from List
                installedModulesGameobjects.RemoveAt(i);

                // delete GameObject from savelist
                installedModuleData.RemoveAt(i);

                // deselsect
                selectionManager.DeselectAll();

                // exit For
                break;
            }
        }

        // Refresh all Shperes
        RefreshModulSpheres();
    }

    public void RefreshModulSpheres()
    {

        // TODO - if any Modul with no parent - the HangarModul script set canGameStart to false;
        canGameStart = true;
        RebuildParents();

        // Refresh all other Moduls
        for (int i = 0; i < installedModulesGameobjects.Count; i++)
        {
            HangarModul hgm = installedModulesGameobjects[i].GetComponent<HangarModul>();
            hgm.ControllDelete();
        }


        Debug.Log("gameCanStart? " + canGameStart);
    }

    private void OnDestroy()
    {
        List <ModuleData> moduleDataSave = new();
        foreach (ModuleDataRuntime item in installedModuleData)
        {
            ModuleData moduleData = new();
            moduleData.level = item.level;
            moduleData.x = item.x;
            moduleData.z = item.z;
            moduleData.moduleTypeIndex = item.moduleTypeIndex;
            moduleDataSave.Add(moduleData);
        }
        dataService.SaveData("modules.json", moduleDataSave, false);
    }

    public List<ModuleDataRuntime> GetModulesByLevel(int level)
    {
        List<ModuleDataRuntime> result = new();
        // TODO temporary helper function
        foreach (ModuleDataRuntime item in installedModuleData)
        {
            if(item.level == level)
            {
                result.Add(item);
            }
        }
        return result;
    }
}

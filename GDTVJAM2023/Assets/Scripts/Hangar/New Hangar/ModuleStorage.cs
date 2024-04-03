using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
    private HangarUIController hangarUIController;

    public ShipPreset shipPreset;
    // TODO can be replaced by List<HangarModule> to get rid of GetComponent<HangarModule>
    public List<HangarModul> installedHangarModules;

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
            LoadPreset();
        }
        else
        {
            foreach (ModuleData item in loadedModules)
            {
                installedModuleData.Add(new ModuleDataRuntime(item));
            }
        }
        BuildShipFromModuleData();

        if (gameManager == null)
        {
            // find selection manager
            selectionManager = GameObject.Find("SelectionController").GetComponent<Selection>();

            // rebuild parents
            RebuildParents();

            // set ShipPanel
            hangarUIController = selectionManager.gameObject.GetComponent<HangarUIController>();
            hangarUIController.SetShipPanel();
        }
    }

    public void RebuildParents()
    {
        for (int i = 0; i < installedModuleData.Count; i++)
        {
            HangarModul hgm = installedHangarModules[i].GetComponent<HangarModul>();
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
        for (int i = 0; i < installedHangarModules.Count; i++)
        {
            HangarModul hgm = installedHangarModules[i].GetComponent<HangarModul>();
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
                Destroy(installedHangarModules[i].gameObject);

                // delete GameObject from List
                installedHangarModules.RemoveAt(i);

                // delete GameObject from savelist
                installedModuleData.RemoveAt(i);

                // reset ship ui panel
                hangarUIController.SetShipPanel();

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
        for (int i = 0; i < installedHangarModules.Count; i++)
        {
            HangarModul hgm = installedHangarModules[i].GetComponent<HangarModul>();
            hgm.ControllDelete();
        }


        Debug.Log("gameCanStart? " + canGameStart);
    }

    private void OnDestroy()
    {
        List<ModuleData> moduleDataSave = new();
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
            if (item.level == level)
            {
                result.Add(item);
            }
        }
        return result;
    }

    public void NewShip()
    {
        installedModuleData.Add(new ModuleDataRuntime());
    }

    public void LoadPreset()
    {
        foreach (ModuleData item in shipPreset.baseModules)
        {
            installedModuleData.Add(new ModuleDataRuntime(item));
        }
        Debug.Log("LoadPreset");
    }

    public void BuildShipFromModuleData()
    {
        foreach (ModuleDataRuntime instance in installedModuleData)
        {
            // Hangar
            if (gameManager == null)
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleTypeIndex].hangarPrefab, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                HangarModul hangarModul = go.GetComponent<HangarModul>();

                // copy all Module Values
                hangarModul.moduleValues = moduleList.moduls[instance.moduleTypeIndex].moduleValues;

                // set inspector Values
                hangarModul.moduleValues.moduleName = moduleList.moduls[instance.moduleTypeIndex].moduleName;
                hangarModul.moduleValues.moduleType = moduleList.moduls[instance.moduleTypeIndex].moduleType;
                hangarModul.moduleValues.canLeft = moduleList.moduls[instance.moduleTypeIndex].canLeft;
                hangarModul.moduleValues.canRight = moduleList.moduls[instance.moduleTypeIndex].canRight;
                hangarModul.moduleValues.canFront = moduleList.moduls[instance.moduleTypeIndex].canFront;
                hangarModul.moduleValues.canBack = moduleList.moduls[instance.moduleTypeIndex].canBack;

                installedHangarModules.Add(hangarModul);
            }

            // Level
            else
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleTypeIndex].modulePrefabs, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, 0, 0);

                //TODO: copy and use ModuleValues
            }
        }
    }

    public void RemoveAllModule()
    {
        // deselsect
        selectionManager.DeselectAll();

        for (int i = 0; i < installedHangarModules.Count; i++)
        {
            // destroy gameObject
            Destroy(installedHangarModules[i].gameObject);

           
        }

        // delete GameObject from List
        installedHangarModules.Clear();

        // delete GameObject from savelist
        installedModuleData.Clear();

        // reset ship ui panel
        hangarUIController.SetShipPanel();
    }
}

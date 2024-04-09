using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModuleDataRuntime : ModuleData
{
    public ushort cost = 1; // cannot be 0
    public ushort bestCost = ushort.MaxValue;

    public ModuleDataRuntime()
    {

    }
    public ModuleDataRuntime(ModuleData baseData)
    {
        x = baseData.x;
        z = baseData.z;
        moduleTypeIndex = baseData.moduleTypeIndex;
    }

    public void ResetCost()
    {
        cost = 1;
        bestCost = ushort.MaxValue;
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
    public float x;
    public float z;
    public int moduleTypeIndex;
}

public class ModuleStorage : MonoBehaviour
{
    public List<ModuleDataRuntime> installedModuleData;
    public ModuleDataRuntime[,] installedModuleGrid;
    JsonDataService dataService;
    public ModuleList moduleList;
    public Transform transformParent;
    private GameObject gameManager;
    private Selection selectionManager;
    private HangarUIController hangarUIController;

    public ShipPreset shipPreset;
    // TODO can be replaced by List<HangarModule> to get rid of GetComponent<HangarModule>
    public List<HangarModul> installedHangarModules;

    //ModuleLists
    public List<int> leftModules;
    public List<int> rightModules;
    public List<int> frontModules;
    public List<int> backModules;
    public List<int> strafeModules;

    public bool isAllConnected = true;
    public bool isEnergiePositiv = true;

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

            // set Module lists für spheres
            CreateModuleLists();

            // set ShipPanel
            hangarUIController = selectionManager.gameObject.GetComponent<HangarUIController>();
            hangarUIController.SetShipPanel();
        }

        BuildModuleGrid();
    }

    private void OnDestroy()
    {
        List<ModuleData> moduleDataSave = new();
        foreach (ModuleDataRuntime item in installedModuleData)
        {
            ModuleData moduleData = new();
            moduleData.x = item.x;
            moduleData.z = item.z;
            moduleData.moduleTypeIndex = item.moduleTypeIndex;
            moduleDataSave.Add(moduleData);
        }
        dataService.SaveData("modules.json", moduleDataSave, false);
    }

    public void HangarRemoveModule()
    {
        // delete the correct module
        for (int i = 0; i < installedHangarModules.Count; i++)
        {
            HangarModul hgm = installedHangarModules[i];
            if (hgm.isSelected == true)
            {
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

                // 
                BuildModuleGrid();

                AudioManager.Instance.PlaySFX("HangarDelete");

                // exit For
                break;
            }
        }

        // Refresh all Shperes
        RefreshModulSpheres();
    }

    public void HangarChangeModule()
    {
        // delete the correct module
        for (int i = 0; i < installedHangarModules.Count; i++)
        {
            HangarModul hgm = installedHangarModules[i];
            if (hgm.isSelected == true)
            {
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
    }

    public void RemoveDisconnectedModules()
    {
        int i = 0;
        while (i < installedHangarModules.Count)
        {
            HangarModul hgm = installedHangarModules[i];
            if (hgm.moduleData.bestCost == ushort.MaxValue && moduleList.moduls[hgm.moduleData.moduleTypeIndex].moduleType != ModuleType.StrafeEngine)
            {
                // destroy gameObject
                Destroy(hgm.gameObject);

                // delete GameObject from List
                installedHangarModules.RemoveAt(i);

                // delete GameObject from savelist
                installedModuleData.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        // reset ship ui panel
        hangarUIController.SetShipPanel();

        // deselsect
        selectionManager.DeselectAll();

        // HideButton
        isAllConnected = true;
        ControllUnconnectedModules();

        AudioManager.Instance.PlaySFX("HangarDeleteAll");

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

    public void RefreshModulSpheres()
    {

        // TODO - if any Modul with no parent - the HangarModul script set canGameStart to false;
        isAllConnected = true;

        // Refresh all other Moduls
        for (int i = 0; i < installedHangarModules.Count; i++)
        {
            HangarModul hgm = installedHangarModules[i];
            hgm.ControllChildSpheres();
        }
    }

    public void BuildModuleGrid()
    {
        isAllConnected = true;
        ControllUnconnectedModules(); // reset

        installedModuleGrid = new ModuleDataRuntime[101, 101]; // allow coordinates from -50 to 50
        foreach (ModuleDataRuntime mdr in installedModuleData)
        {
            installedModuleGrid[(int)mdr.x + 50, (int)mdr.z + 50] = mdr;
            mdr.ResetCost();
        }

        Queue<ModuleDataRuntime> cellsToCheck = new Queue<ModuleDataRuntime>();
        installedModuleGrid[50, 50].cost = 0;
        installedModuleGrid[50, 50].bestCost = 0;
        cellsToCheck.Enqueue(installedModuleGrid[50, 50]);

        while (cellsToCheck.Count > 0)
        {
            ModuleDataRuntime curCell = cellsToCheck.Dequeue();
            List<ModuleDataRuntime> curNeighbors = GetNeighborCells((int)curCell.x + 50, (int)curCell.z + 50, curCell);
            foreach (ModuleDataRuntime curNeighbor in curNeighbors)
            {
                if (curNeighbor == null) continue;
                if (curNeighbor.cost == ushort.MaxValue) { continue; } // we do not need to calculate where I could go from there because I can never be there
                if (curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost) // calculate where to go from that neighbor cell
                {
                    curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost); // this works because initially all bestCost are ushort.max
                    cellsToCheck.Enqueue(curNeighbor);
                }
            }
        }
    }

    

    private List<ModuleDataRuntime> GetNeighborCells(int x, int z, ModuleDataRuntime curCell)
    {
        List<ModuleDataRuntime> result = new();
        // check all neighbors if they can be connected in the direction of the current field and if the current field can be connected in the neighbor's direction. If both apply, that direction is a valid path
        if (moduleList.moduls[curCell.moduleTypeIndex].canHaveFrontNeighbor && installedModuleGrid[x - 1, z] != null && moduleList.moduls[installedModuleGrid[x - 1, z].moduleTypeIndex].canHaveBackNeighbor)
        {
            result.Add(installedModuleGrid[x - 1, z]);
        }
        if (moduleList.moduls[curCell.moduleTypeIndex].canHaveBackNeighbor && installedModuleGrid[x + 1, z] != null && moduleList.moduls[installedModuleGrid[x + 1, z].moduleTypeIndex].canHaveFrontNeighbor)
        {
            result.Add(installedModuleGrid[x + 1, z]);
        }
        if (moduleList.moduls[curCell.moduleTypeIndex].canHaveRightNeighbor && installedModuleGrid[x, z + 1] != null && moduleList.moduls[installedModuleGrid[x, z + 1].moduleTypeIndex].canHaveLeftNeighbor)
        {
            result.Add(installedModuleGrid[x, z + 1]);
        }
        if (moduleList.moduls[curCell.moduleTypeIndex].canHaveLeftNeighbor && installedModuleGrid[x, z - 1] != null && moduleList.moduls[installedModuleGrid[x, z - 1].moduleTypeIndex].canHaveRightNeighbor)
        {
            result.Add(installedModuleGrid[x, z - 1]);
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
                hangarModul.moduleData = instance;

                installedHangarModules.Add(hangarModul);
            }

            // Level
            else
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleTypeIndex].modulePrefabs, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, 0, 0);

                BaseModule[] baseModules = go.GetComponents<BaseModule>();

                int i = 0;
                //copy all Module Vales
                foreach (BaseModule bm in baseModules)
                {
                    if (bm != null)
                    {
                        if (i == 0)
                        {
                            bm.moduleValues = moduleList.moduls[instance.moduleTypeIndex].moduleValues;
                        }
                    }
                    i++;
                }
            }
        }
    }

  

    private void CreateModuleLists()
    {
        foreach (Modules module in moduleList.moduls)
        {
            if (module.canLeft == true)
                leftModules.Add(moduleList.moduls.IndexOf(module));
            if (module.canRight == true)
                rightModules.Add(moduleList.moduls.IndexOf(module));
            if (module.canFront == true)
                frontModules.Add(moduleList.moduls.IndexOf(module));
            if (module.canBack == true)
                backModules.Add(moduleList.moduls.IndexOf(module));
            if (module.moduleType == ModuleType.StrafeEngine)
                strafeModules.Add(moduleList.moduls.IndexOf(module));
        }
    }

    public void ControllUnconnectedModules()
    {
        if (gameManager == null)
        hangarUIController.ControllUnconnectedModules(isAllConnected);
    }
}

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
    public ModuleData()
    {

    }
    public ModuleData(ModuleDataRuntime mdr)
    {
        x = mdr.x;
        z = mdr.z;
        moduleTypeIndex = mdr.moduleTypeIndex;
    }

    public float x;
    public float z;
    public int moduleTypeIndex;
}

public class ModuleStorage : MonoBehaviour
{
    /// <summary>
    /// The current ship's modules with hangar specific information
    /// </summary>
    public List<ModuleDataRuntime> installedModuleData;
    public ModuleDataRuntime[,] installedModuleGrid;
    public ModuleList moduleList;
    public Transform transformParent;
    public HangarRotateShips rotateHangarObject;
    private HangarSelection selectionManager;
    private HangarUIController hangarUIController;

    public ShipPreset shipPreset;
    // TODO can be replaced by List<HangarModule> to get rid of GetComponent<HangarModule>
    public List<HangarModul> installedHangarModules;

    //ModuleLists
    public List<int> possibleModules;

    public bool isAllConnected = true;
    public bool isEnergiePositiv = true;

    //PlayerData
    public PlayerData playerData;

    private HangarInputHandler hangarInputHandler;


    /* **************************************************************************** */
    /* LIFECYCLE------------------------------------------------------------------- */
    /* **************************************************************************** */

    void Start()
    {
        // find selection manager
        selectionManager = GameObject.Find("SelectionController").GetComponent<HangarSelection>();

        // set ShipPanel
        hangarUIController = selectionManager.gameObject.GetComponent<HangarUIController>();
        hangarUIController.SetShipPanel();

        installedModuleGrid = new ModuleDataRuntime[101, 101]; // allow coordinates from -50 to 50


        Debug.Log("ModuleStorage GetActiveShip : " + playerData.GetActiveShip());

        // Set Hangar Rotation to loaded position
        rotateHangarObject.RotateShip(playerData.ActiveShip+1, false);

        LoadShip();

        hangarInputHandler = selectionManager.gameObject.GetComponent<HangarInputHandler>();
        hangarInputHandler.OnModuleRemove += HangarRemoveModule;
    }

    public void LoadShip()
    {
        // load active ship
        installedModuleData = new();

        foreach (ModuleData item in playerData.GetActiveShip())
        {
            installedModuleData.Add(new ModuleDataRuntime(item));
        }

        BuildShipFromModuleData();

        BuildModuleGrid();

        hangarUIController.SetShipPanel();
    }

    public void LoadShipFromPreset()
    {
        // load active ship
        installedModuleData.Clear();

        foreach (ModuleData item in playerData.GetActiveShip())
        {
            installedModuleData.Add(new ModuleDataRuntime(item));
            playerData.moduleCounts[item.moduleTypeIndex] -= 1;
        }

        BuildShipFromModuleData();

        BuildModuleGrid();

        hangarUIController.SetShipPanel();
    }

    /// <summary>
    /// Called when exiting the Hangar scene. Copies runtime module lists to savable data
    /// </summary>
    private void OnDestroy()
    {
        List<ModuleData> ship = new();
        foreach (ModuleDataRuntime item in installedModuleData)
        {
            ModuleData moduleData = new(item);
            ship.Add(moduleData);
        }
        playerData.SetActiveShip(ship);
        AudioManager.Instance.SavePlayerData();
    }

    public void SetShipToPlayerData()
    {
        List<ModuleData> ship = new();
        foreach (ModuleDataRuntime item in installedModuleData)
        {
            ModuleData moduleData = new(item);
            ship.Add(moduleData);
        }
        playerData.SetActiveShip(ship);
    }


    /* **************************************************************************** */
    /* HANDLE HANGAR FUNCTIONS----------------------------------------------------- */
    /* **************************************************************************** */

    public void HangarRemoveModule()
    {
        // delete the correct module
        for (int i = 0; i < installedHangarModules.Count; i++)
        {
            HangarModul hgm = installedHangarModules[i];

            if (hgm.isSelected == true && hgm.moduleValues.moduleType != ModuleType.Cockpit)
            {
                // delete from playerData
                playerData.moduleCounts[hgm.moduleData.moduleTypeIndex] += 1;

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
                // delete from playerData
                playerData.moduleCounts[hgm.moduleData.moduleTypeIndex] += 1;

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
                // delete from playerData
                playerData.moduleCounts[hgm.moduleData.moduleTypeIndex] += 1;

                // destroy gameObject
                Destroy(hgm.gameObject);

                // delete GameObject from List
                installedHangarModules.RemoveAt(i);

                // delete from playerData
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
            // delete from playerData
            playerData.moduleCounts[installedHangarModules[i].moduleData.moduleTypeIndex] += 1;

            // destroy gameObject
            Destroy(installedHangarModules[i].gameObject);

        }

        // delete GameObject from List
        installedHangarModules.Clear();

        // delete GameObject from savelist
        installedModuleData.Clear();
    }

    public void RemoveStrafeEngine(GameObject strafeEngine)
    {
        HangarModul hgm = strafeEngine.GetComponent<HangarModul>();

        // delete from playerData
        playerData.moduleCounts[hgm.moduleData.moduleTypeIndex] += 1;

        // destroy gameObject
        Destroy(strafeEngine);

        for (int i = 0; i < installedHangarModules.Count; i++)
        {
            if (installedModuleData[i].moduleTypeIndex == hgm.moduleData.moduleTypeIndex)
            {

                // delete GameObject from List
                installedHangarModules.RemoveAt(i);

                // delete GameObject from savelist
                installedModuleData.RemoveAt(i);

                break;
            }
        }
        // reset ship ui panel
        hangarUIController.SetShipPanel();
    }


    public void RefreshModulSpheres()
    {
        // TODO - if any Modul with no parent - the HangarModul script set canGameStart to false;

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

        // reset Grid
        for (int i = 0; i < installedModuleGrid.GetLength(0); i++)
        {
            for (int j = 0; j < installedModuleGrid.GetLength(1); j++)
            {
                installedModuleGrid[i, j] = null;
            }
        }

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
        playerData.moduleCounts[installedModuleData[0].moduleTypeIndex] -= 1;
    }

    public void LoadPreset()
    {
        foreach (ModuleData item in shipPreset.baseModules)
        {
            installedModuleData.Add(new ModuleDataRuntime(item));
            playerData.moduleCounts[item.moduleTypeIndex] -= 1;
        }
        Debug.Log("LoadPreset");
    }

    /// <summary>
    /// Create game objects for active ship
    /// </summary>
    public void BuildShipFromModuleData()
    {
        foreach (ModuleDataRuntime instance in installedModuleData)
        {
            // Hangar
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
    }

    public void CreateModuleLists(Sphere.SphereSide sphSide)
    {
        possibleModules.Clear();

        switch (sphSide)
        {
            case Sphere.SphereSide.left:
                foreach (Modules module in moduleList.moduls)
                {
                    int index = moduleList.moduls.IndexOf(module);

                    if (module.canLeft == true && playerData.moduleCounts[index] > 0)
                        possibleModules.Add(moduleList.moduls.IndexOf(module));
                }
                break;

            case Sphere.SphereSide.right:
                foreach (Modules module in moduleList.moduls)
                {
                    int index = moduleList.moduls.IndexOf(module);

                    if (module.canRight == true && playerData.moduleCounts[index] > 0)
                        possibleModules.Add(moduleList.moduls.IndexOf(module));
                }
                break;

            case Sphere.SphereSide.front:
                foreach (Modules module in moduleList.moduls)
                {
                    int index = moduleList.moduls.IndexOf(module);

                    if (module.canFront == true && playerData.moduleCounts[index] > 0)
                        possibleModules.Add(moduleList.moduls.IndexOf(module));
                }
                break;

            case Sphere.SphereSide.back:
                foreach (Modules module in moduleList.moduls)
                {
                    int index = moduleList.moduls.IndexOf(module);

                    if (module.canBack == true && playerData.moduleCounts[index] > 0)
                        possibleModules.Add(moduleList.moduls.IndexOf(module));
                }
                break;

            case Sphere.SphereSide.strafe:
                foreach (Modules module in moduleList.moduls)
                {
                    int index = moduleList.moduls.IndexOf(module);

                    if (module.moduleType == ModuleType.StrafeEngine && playerData.moduleCounts[index] > 0)
                        possibleModules.Add(moduleList.moduls.IndexOf(module));
                }
                break;


        }
    }

    public void CreateReplacementList(HangarModul hangarModule)
    {
        possibleModules.Clear();

        foreach (Modules module in moduleList.moduls)
        {
            int index = moduleList.moduls.IndexOf(module);

            switch (hangarModule.moduleValues.moduleType)
            {
                case ModuleType.Cockpit:
                case ModuleType.StrafeEngine:
                    if (playerData.moduleCounts[index] > 0 && module.moduleType == hangarModule.moduleValues.moduleType && module.moduleName != hangarModule.moduleValues.moduleName)
                    {
                        possibleModules.Add(moduleList.moduls.IndexOf(module));
                    }
                    break;
                default:
                    if (playerData.moduleCounts[index] > 0 &&
                        (hangarModule.moduleValues.canLeft == false || (module.canLeft == hangarModule.moduleValues.canLeft)) &&
                        (hangarModule.moduleValues.canRight == false || (module.canRight == hangarModule.moduleValues.canRight)) &&
                        (hangarModule.moduleValues.canFront == false || (module.canFront == hangarModule.moduleValues.canFront)) &&
                        (hangarModule.moduleValues.canBack == false || (module.canBack == hangarModule.moduleValues.canBack)) &&
                        !hangarModule.moduleValues.moduleName.Equals(module.moduleName)
                        )
                    {
                        possibleModules.Add(moduleList.moduls.IndexOf(module));
                    }
                    break;
            }


        }
    }

    public void ControllUnconnectedModules()
    {
        hangarUIController.ControllUnconnectedModules(isAllConnected);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ModuleInstance
{
    public float x;
    public float z;
    public float rotation;
    public int moduleIndex;
}

public class ModuleStorage : MonoBehaviour
{
    public List<ModuleInstance> baseModules;
    JsonDataService dataService;
    public ModuleList moduleList;
    public Transform transformParent;
    private GameObject gameManager;

    public ShipPreset shipPreset;
    public List<GameObject> installedModules;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager");

        baseModules = new();
        dataService = new();
        baseModules = dataService.LoadData<List<ModuleInstance>>("modules.json", false);

        if (baseModules.Count <= 0)
        {
            baseModules = shipPreset.baseModules;
            Debug.Log("LoadPreset");
        }
        foreach (ModuleInstance instance in baseModules) {
                       
            // Hangar
            if (gameManager == null)
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleIndex].hangarPrefab, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, instance.rotation, 0);
                installedModules.Add(go);
            }

            // Level
            else
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleIndex].modulePrefabs, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, instance.rotation, 0);
            }
        }
    }

    public void HangarRemoveModule()
    {
        // delete the right Modul
        for (int i = 0; i < installedModules.Count; i++)
        {
            HangarModul hM = installedModules[i].GetComponent<HangarModul>();
            if (hM.isActiv == true)
            {
                // destroy gameObject
                Destroy(installedModules[i]);

                // delete GameObject from List
                installedModules.RemoveAt(i);

                // delete GameObject from savelist
                baseModules.RemoveAt(i);

                // exit For
                break;
            }
        }

        // Refresh all Shperes
        RefreshModulSpheres();
    }

    public void RefreshModulSpheres()
    {
        // Refresh all other Moduls
        for (int i = 0; i < installedModules.Count; i++)
        {
            HangarModul hM = installedModules[i].GetComponent<HangarModul>();
            hM.ControllDelete();
        }
    }

    private void OnDestroy()
    {
        dataService.SaveData("modules.json", baseModules, false);
        
    }
}

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
            //Instantiate(moduleList.moduls[instance.moduleIndex].modulePrefabs, new Vector3(transform.position.x + instance.x * transform.localScale.x, transform.position.y, transform.position.z + instance.z * transform.localScale.z), Quaternion.Euler(0, instance.rotation, 0), transformParent);            
            if (gameManager == null)
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleIndex].hangarPrefab, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, instance.rotation, 0);
            }
            else
            {
                GameObject go = Instantiate(moduleList.moduls[instance.moduleIndex].modulePrefabs, transformParent, false);
                go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
                go.transform.localRotation = Quaternion.Euler(0, instance.rotation, 0);
            }
        }
    }

 

    private void OnDestroy()
    {
        dataService.SaveData("modules.json", baseModules, false);
        
    }
}

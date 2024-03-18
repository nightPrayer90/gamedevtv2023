using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ModuleInstance
{
    public float x;
    public float y;
    public float rotation;
    public int moduleIndex;
}

public class ModuleStorage : MonoBehaviour
{
    public List<ModuleInstance> baseModules;
    JsonDataService dataService;
    private const float moduleEdgeLength = 1;
    public HangarController hangarController;

    // Start is called before the first frame update
    void Start()
    {
        baseModules = new();
        dataService = new();
        baseModules = dataService.LoadData<List<ModuleInstance>>("modules.json", false);
        foreach(ModuleInstance instance in baseModules) {
            Instantiate(hangarController.modulePrefabs[instance.moduleIndex], new Vector3(instance.x, transform.position.y, instance.y), Quaternion.Euler(0, instance.rotation, 0), gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        dataService.SaveData("modules.json", baseModules, false);
        
    }
}

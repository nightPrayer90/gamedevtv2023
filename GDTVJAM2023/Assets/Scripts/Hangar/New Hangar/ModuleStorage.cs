using System;
using System.Collections;
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
    private const float moduleEdgeLength = 1;
    public ModuleList moduleList;

    // Start is called before the first frame update
    void Start()
    {
        baseModules = new();
        dataService = new();
        baseModules = dataService.LoadData<List<ModuleInstance>>("modules.json", false);
        foreach(ModuleInstance instance in baseModules) {
            Instantiate(moduleList.modulePrefabs[instance.moduleIndex], new Vector3(transform.position.x + instance.x * transform.localScale.x, transform.position.y, transform.position.z + instance.z * transform.localScale.z), Quaternion.Euler(0, instance.rotation, 0), gameObject.transform);
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

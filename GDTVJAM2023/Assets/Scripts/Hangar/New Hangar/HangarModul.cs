using System.Collections.Generic;
using UnityEngine;


public class HangarModul : MonoBehaviour
{
    [Header("Management")]
    [HideInInspector] public bool isSelected = false;

    [Header("GameObjects")]
    public List<Sphere> spheres;
    private Selection selectionController;
    private ModuleStorage moduleStorage;
    [HideInInspector] public List<int> possibleReplacements;
    private MeshRenderer childMeshRenderer;
    public Material highlightMaterial;
    private Material shipMaterial;

    [Header("Module Data")]
    public ModuleDataRuntime moduleData;

    [Header("Module Values")]
    public ModuleValues moduleValues;



    private void Awake()
    {
        selectionController = GameObject.Find("SelectionController").GetComponent<Selection>();
        selectionController.OnDeselect += HandleSetDeselect;
        childMeshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        shipMaterial = childMeshRenderer.materials[0];

        moduleStorage = GameObject.Find("Ship").GetComponentInParent<ModuleStorage>();
    }

    private void Start()
    {
        ControllOwnMaterial();
    }

    public void HandleSetDeselect()
    {
        if (isSelected == true)
        {
            isSelected = false;
        }
    }

    public void SetActive()
    {
        isSelected = true;
    }

    // control function, if an installed Modul was deleted
    public void ControllChildSpheres()
    {
        ControllOwnMaterial();

        // turn Shperes on or off
        foreach (Sphere sph in spheres)
        {
            sph.ControllSpheres();
        }
    }

    private void ControllOwnMaterial()
    {
        Material[] materials = childMeshRenderer.materials;

       if (moduleData.bestCost == ushort.MaxValue && moduleValues.moduleType != ModuleType.StrafeEngine)
        {
            materials[0] = highlightMaterial;
            moduleStorage.isAllConnected = false;
            moduleStorage.ControllUnconnectedModules();
        }
        else
        {
            materials[0] = shipMaterial;
        }
        childMeshRenderer.materials = materials;
    }

    public void CreateModuleList()
    {
        possibleReplacements.Clear();
        foreach (Modules module in moduleStorage.moduleList.moduls)
        {
            int index = moduleStorage.moduleList.moduls.IndexOf(module);
 
            switch (moduleValues.moduleType)
            {
                case ModuleType.Cockpit:
                case ModuleType.StrafeEngine:
                    if (moduleStorage.playerData.moduleCounts[index] >  0 && module.moduleType == moduleValues.moduleType && module.moduleName != moduleValues.moduleName)
                    {
                        possibleReplacements.Add(moduleStorage.moduleList.moduls.IndexOf(module));
                    }
                    break;
                default:
                    if (moduleStorage.playerData.moduleCounts[index] > 0 &&
                        (moduleValues.canLeft == false || (module.canLeft == moduleValues.canLeft)) &&
                        (moduleValues.canRight == false || (module.canRight == moduleValues.canRight)) &&
                        (moduleValues.canFront == false || (module.canFront == moduleValues.canFront)) &&
                        (moduleValues.canBack == false || (module.canBack == moduleValues.canBack)) &&
                        !moduleValues.moduleName.Equals(module.moduleName)
                        )
                    {
                        possibleReplacements.Add(moduleStorage.moduleList.moduls.IndexOf(module));
                    }
                    break;
            }


        }
    }
}

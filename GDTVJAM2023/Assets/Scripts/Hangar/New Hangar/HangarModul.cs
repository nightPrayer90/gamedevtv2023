using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class HangarModul : MonoBehaviour
{
    [Header("Management")]
    public bool isSelected = false;
    public bool hasNoParentControll = false; // only ture on Cockpit or Strafe modules
    public bool haveParent = false;

    [Header("GameObjects")]
    public List<Sphere> spheres;
    private Selection selectionController;
    private ModuleStorage moduleStorage;
    public List<int> possibleReplacements;
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

        // only for Cockpit or Strafe
        if (hasNoParentControll == true)
            haveParent = true;
    }

    private void Start()
    {
        CreateModuleList();
        ControllDelete();
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
    public void ControllDelete()
    {
        Material[] materials = childMeshRenderer.materials;

        materials[0] = moduleData.bestCost == ushort.MaxValue ? highlightMaterial : shipMaterial;

        childMeshRenderer.materials = materials;

        // turn Shperes on or off
        foreach (Sphere sph in spheres)
        {
            sph.ControllSpheres();
        }
    }

    public void CreateModuleList()
    {
        foreach (Modules module in moduleStorage.moduleList.moduls)
        {
            switch (moduleValues.moduleType)
            {
                case ModuleType.Cockpit:
                case ModuleType.StrafeEngine:
                    if (module.moduleType == moduleValues.moduleType && module.moduleName != moduleValues.moduleName)
                    {

                        possibleReplacements.Add(moduleStorage.moduleList.moduls.IndexOf(module));
                    }
                    break;
                default:
                    if ((moduleValues.canLeft == false || (module.canLeft == moduleValues.canLeft)) &&
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

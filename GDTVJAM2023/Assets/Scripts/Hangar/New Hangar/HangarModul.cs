using System;
using System.Collections.Generic;
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

    [Header("Module Data")]
    public ModuleDataRuntime moduleData;

    [Header("Module Values")]
    public ModuleValues moduleValues;



    private void Awake()
    {
        selectionController = GameObject.Find("SelectionController").GetComponent<Selection>();
        selectionController.OnDeselect += HandleSetDeselect;

        moduleStorage = GameObject.Find("Ship").GetComponentInParent<ModuleStorage>();

        // only for Cockpit or Strafe
        if (hasNoParentControll == true)
            haveParent = true;
    }

    private void Start()
    {
        CreateModulList();
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
        // turn Shperes on or off
        foreach (Sphere sph in spheres)
        {
            sph.ControllSpheres();
        }
    }

    public void CreateModulList()
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
                    if (module.canLeft == moduleValues.canLeft && module.canRight == moduleValues.canRight &&
                    module.canFront == moduleValues.canFront && module.canBack == moduleValues.canBack
                     && module.moduleName != moduleValues.moduleName)
                    {
                        possibleReplacements.Add(moduleStorage.moduleList.moduls.IndexOf(module));
                    }
                    break;
            }

            
        }
    }
}

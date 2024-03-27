using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class ModuleValues
{
    public string moduleName; // set from other objects (create)
    [TextArea]
    public string modulDescription_multiLineText;
    public float costMass;
    public float costEnergie;
    public float energieProduction;
    public int energieStorage;
    public int health;
    public float protection;
    public float mainEngine;
    public float directionEngine;
    public float strafeEngine;
}


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
        if (moduleData.parentModule == null)
        {
            Debug.Log("I have no Parent (select)");
        }
        else
        {
            Debug.Log("level Parent Modul " + moduleData.parentModule.level);
        }
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

    // TODO - just temporary
    public void ParentControl()
    {
        if (moduleData.parentModule == null && hasNoParentControll == false)
        {
            haveParent = false;
            Debug.Log("i have no Parent (ParentControl)");
            moduleStorage.canGameStart = false;
        }
        else
        {
            haveParent = true;
        }
    }

}

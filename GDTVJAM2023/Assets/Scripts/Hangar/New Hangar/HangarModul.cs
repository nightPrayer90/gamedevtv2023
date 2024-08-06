using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class HangarModul : MonoBehaviour
{
    [Header("Management")]
    [HideInInspector] public bool isSelected = false;

    [Header("GameObjects")]
    public List<Sphere> spheres;
    private HangarSelection selectionController;
    private ModuleStorage moduleStorage;
    [HideInInspector] public MeshRenderer childMeshRenderer;
    [FormerlySerializedAs("highlightMaterial")] public Material unconnectedMaterial;
    private Material shipMaterial;

    [Header("Module Data")]
    public ModuleDataRuntime moduleData;

    [Header("Module Values")]
    public ModuleValues moduleValues;



    private void Awake()
    {
        selectionController = GameObject.Find("SelectionController").GetComponent<HangarSelection>();
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
        // Maybe TODO Later -> if change from another Cockpit to Cockpit 0 Delete the StrafeEngine
        if (moduleValues.moduleType == ModuleType.StrafeEngine)
        {
            foreach(ModuleDataRuntime module in moduleStorage.installedModuleData)
            {
                if (module.moduleTypeIndex == 0)
                {
                    moduleStorage.RemoveStrafeEngine(gameObject);
                    break;
                }
            }
        }

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
            materials[0] = unconnectedMaterial;
            moduleStorage.isAllConnected = false;
            moduleStorage.ControllUnconnectedModules();
        }
        else
        {
            if (moduleValues.moduleType == ModuleType.Cockpit)
            {
                selectionController.SelectModuleAtPosition(0, 0); // this is me
                return;
            }
            else
            {
                materials[0] = shipMaterial;
            }
        }
        childMeshRenderer.materials = materials;
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public bool isModulSet = false;
    private bool isActiv = false;
    private Selection selectionController;
    public  Transform parentTransform;
    private ModuleStorage moduleStorage;
    private List<ModuleInstance> moduleInstances;
    public float spawnPositionX;
    public float spawnPositionZ;
    private MeshRenderer meshRenderer;
    private ModuleList moduleList;
    private GameObject ship;
    private Collider meshCollider;
    public List<int> availableModuls;

    public enum SphereSide
    {
        left,
        right,
        front,
        back,
        strafe
    }
    public SphereSide sphereSide;

    private void Awake()
    {
        selectionController = GameObject.Find("SelectionController").GetComponent<Selection>();
        selectionController.OnDeselect += HandleSetDeselect;
        ship = GameObject.Find("Ship");
        moduleStorage = ship.GetComponent<ModuleStorage>();
        moduleList = moduleStorage.moduleList;

        moduleInstances = moduleStorage.baseModules;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshCollider = gameObject.GetComponent<Collider>();
    }

    private void Start()
    {
        spawnPositionX = parentTransform.localPosition.x;
        spawnPositionZ = parentTransform.localPosition.z;
        ControllSpheres();
        CreateModulList();
    }

    public void HandleSetDeselect()
    {
        if (isActiv == true)
        {
            isActiv = false;
        }
    }

    public void SetActive()
    {
        isActiv = true;
    }

    public void CreateModulList()
    {
        foreach (Modules module in moduleList.moduls)
        {
            switch (sphereSide)
            {
                case SphereSide.left:
                    if (module.canLeft == true)
                        availableModuls.Add(moduleList.moduls.IndexOf(module));
                    break;
                case SphereSide.right:
                    if (module.canRight == true)
                        availableModuls.Add(moduleList.moduls.IndexOf(module));
                    break;
                case SphereSide.front:
                    if (module.canFront == true)
                        availableModuls.Add(moduleList.moduls.IndexOf(module));
                    break;
                case SphereSide.back:
                    if (module.canBack == true)
                        availableModuls.Add(moduleList.moduls.IndexOf(module));
                    break;
                case SphereSide.strafe:
                    if (module.myEnumVariable == Modules.ModulTyp.StrafeEngine)
                        availableModuls.Add(moduleList.moduls.IndexOf(module));
                    break;
            }
        }
    }

    public void ControllSpheres()
    {
        if (moduleInstances.Count > 0)
        {
            switch (sphereSide)
            {
                case SphereSide.left:
                    foreach (ModuleInstance module in moduleInstances)
                    {
                        if (module.x == spawnPositionX && module.z == spawnPositionZ - 1)
                        {
                            meshRenderer.enabled = false;
                            meshCollider.enabled = false;
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshCollider.enabled = true;
                            meshRenderer.enabled = true;
                            isModulSet = false;
                        }
                    }
                    break;

                case SphereSide.right:
                    foreach (ModuleInstance module in moduleInstances)
                    {
                        if (module.x == spawnPositionX && module.z == spawnPositionZ + 1)
                        {
                            meshRenderer.enabled = false;
                            meshCollider.enabled = false;
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshRenderer.enabled = true;
                            meshCollider.enabled = true;
                            isModulSet = false;
                        }
                    }
                    break;

                case SphereSide.front:
                    foreach (ModuleInstance module in moduleInstances)
                    {
                        if (module.x == spawnPositionX - 1 && module.z == spawnPositionZ)
                        {
                            meshCollider.enabled = false;
                            meshRenderer.enabled = false;
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshRenderer.enabled = true;
                            meshCollider.enabled = true;
                            isModulSet = false;
                        }
                    }
                    break;

                case SphereSide.back:
                    foreach (ModuleInstance module in moduleInstances)
                    {
                        if (module.x == spawnPositionX + 1 && module.z == spawnPositionZ)
                        {
                            meshRenderer.enabled = false;
                            meshCollider.enabled = false;
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshRenderer.enabled = true;
                            meshCollider.enabled = true;
                            isModulSet = false;
                        }
                    }

                    break;
                case SphereSide.strafe:
                    foreach (ModuleInstance module in moduleInstances)
                    {
                        if (module.x == -1 && module.z == 0)
                        {
                            meshRenderer.enabled = false;
                            meshCollider.enabled = false;
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshRenderer.enabled = true;
                            meshCollider.enabled = true;
                            isModulSet = false;
                        }
                    }
                    break;
            }
        }
    }
}

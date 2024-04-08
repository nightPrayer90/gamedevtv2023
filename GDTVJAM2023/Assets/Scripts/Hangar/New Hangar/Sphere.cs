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
    private HangarModul parentModul; //TODO
    private ModuleStorage moduleStorage;
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
        parentModul = parentTransform.gameObject.GetComponent<HangarModul>();

        //moduleInstances = moduleStorage.installedModuleData;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshCollider = gameObject.GetComponent<Collider>();
    }

    private void Start()
    {
        spawnPositionX = parentTransform.localPosition.x;
        spawnPositionZ = parentTransform.localPosition.z;
        //ControllSpheres();
        CreateModuleList();
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
        Debug.Log(parentModul.haveParent);
    }

    public void CreateModuleList()
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
                    if (module.moduleType == ModuleType.StrafeEngine)
                        availableModuls.Add(moduleList.moduls.IndexOf(module));
                    break;
            }
        }
    }

    public void ControllSpheres()
    {
        if (moduleStorage.installedModuleData.Count > 0)
        {
            switch (sphereSide)
            {
                case SphereSide.left:
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if ((module.x == spawnPositionX && module.z == spawnPositionZ - 1) || parentModul.moduleData.bestCost == ushort.MaxValue)
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
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if ((module.x == spawnPositionX && module.z == spawnPositionZ + 1) || parentModul.moduleData.bestCost == ushort.MaxValue)
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
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if ((module.x == spawnPositionX - 1 && module.z == spawnPositionZ) || parentModul.moduleData.bestCost == ushort.MaxValue)
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
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if ((module.x == spawnPositionX + 1 && module.z == spawnPositionZ) || parentModul.moduleData.bestCost == ushort.MaxValue)
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
                    foreach (ModuleData module in moduleStorage.installedModuleData)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (CompareTag(collision.transform.tag))
        {
            switch (sphereSide)
            {
                case SphereSide.back:
                case SphereSide.front:
                    
                    break;
                case SphereSide.left:
                case SphereSide.right:
                    
                    break;
            }
        }
    }
}

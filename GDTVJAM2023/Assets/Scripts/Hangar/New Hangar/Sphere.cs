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
        moduleStorage = GameObject.Find("Ship").GetComponent<ModuleStorage>();
        moduleInstances = moduleStorage.baseModules;

        spawnPositionX = parentTransform.localPosition.x;
        spawnPositionZ = parentTransform.localPosition.z;

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        spawnPositionX = parentTransform.localPosition.x;
        spawnPositionZ = parentTransform.localPosition.z;
        ControllSpheres();
    }

    public void HandleSetDeselect()
    {
        if (isActiv == true)
        {
            isActiv = false;
            Debug.Log(isActiv);
        }
    }

    public void SetActive()
    {
        isActiv = true;
        Debug.Log("isActiv");
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
                            isModulSet = true;
                            return;
                        }
                        else
                        {
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
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshRenderer.enabled = true;
                            isModulSet = false;
                        }
                    }
                    break;

                case SphereSide.front:
                    foreach (ModuleInstance module in moduleInstances)
                    {
                        if (module.x == spawnPositionX + 1 && module.z == spawnPositionZ)
                        {
                            meshRenderer.enabled = false;
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshRenderer.enabled = true;
                            isModulSet = false;
                        }
                    }
                    break;

                case SphereSide.back:
                    foreach (ModuleInstance module in moduleInstances)
                    {
                        if (module.x == spawnPositionX - 1 && module.z == spawnPositionZ)
                        {
                            meshRenderer.enabled = false;
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshRenderer.enabled = true;
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
                            isModulSet = true;
                            return;
                        }
                        else
                        {
                            meshRenderer.enabled = true;
                            isModulSet = false;
                        }
                    }
                    break;
            }
        }
    }
}

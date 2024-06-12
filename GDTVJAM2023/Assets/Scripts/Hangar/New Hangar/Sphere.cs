using UnityEngine;

public class Sphere : MonoBehaviour
{
    private bool isActiv = false;
    private HangarSelection selectionController;
    public  Transform parentTransform;
    private HangarModul parentModul; //TODO
    private ModuleStorage moduleStorage;
    public float spawnPositionX;
    public float spawnPositionZ;
    private MeshRenderer meshRenderer;
    private Collider meshCollider;

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
        selectionController = GameObject.Find("SelectionController").GetComponent<HangarSelection>();
        selectionController.OnDeselect += HandleSetDeselect;
        moduleStorage = GameObject.Find("Ship").GetComponent<ModuleStorage>();
        parentModul = parentTransform.gameObject.GetComponent<HangarModul>();

        //moduleInstances = moduleStorage.installedModuleData;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshCollider = gameObject.GetComponent<Collider>();
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
        }
    }

    public void SetActive()
    {
        isActiv = true;
    }

    public void ControllSpheres()
    {
        if (moduleStorage.installedModuleData.Count > 0)
        {
            Debug.Log(parentModul.gameObject.name +   " Control Sphers - Sphere: " + gameObject.name );
            switch (sphereSide)
            {
                case SphereSide.left:
                    meshCollider.enabled = true;
                    meshRenderer.enabled = true;
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if ((module.x == spawnPositionX && module.z == spawnPositionZ - 1) || (parentModul.moduleData.bestCost == ushort.MaxValue) || (spawnPositionX == -1 && spawnPositionZ == 1))
                        {
                            meshRenderer.enabled = false;
                            meshCollider.enabled = false;
                            return;
                        }
                    }
                    break;

                case SphereSide.right:
                    meshCollider.enabled = true;
                    meshRenderer.enabled = true;
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if ((module.x == spawnPositionX && module.z == spawnPositionZ + 1) || (parentModul.moduleData.bestCost == ushort.MaxValue) || (spawnPositionX == -1 && spawnPositionZ == -1))
                        {
                            meshRenderer.enabled = false;
                            meshCollider.enabled = false;
                            return;
                        }
                    }
                    break;

                case SphereSide.front:
                    meshCollider.enabled = true;
                    meshRenderer.enabled = true;
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if ((module.x == spawnPositionX - 1 && module.z == spawnPositionZ) || parentModul.moduleData.bestCost == ushort.MaxValue)
                        {
                            meshCollider.enabled = false;
                            meshRenderer.enabled = false;
                            return;
                        }
                    }
                    break;

                case SphereSide.back:
                    meshCollider.enabled = true;
                    meshRenderer.enabled = true;
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if ((module.x == spawnPositionX + 1 && module.z == spawnPositionZ) || parentModul.moduleData.bestCost == ushort.MaxValue)
                        {
                            meshRenderer.enabled = false;
                            meshCollider.enabled = false;
                            return;
                        }
                    }

                    break;
                case SphereSide.strafe:
                    meshCollider.enabled = true;
                    meshRenderer.enabled = true;
                    foreach (ModuleData module in moduleStorage.installedModuleData)
                    {
                        if (module.x == -1 && module.z == 0)
                        {
                            meshRenderer.enabled = false;
                            meshCollider.enabled = false;
                            return;
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

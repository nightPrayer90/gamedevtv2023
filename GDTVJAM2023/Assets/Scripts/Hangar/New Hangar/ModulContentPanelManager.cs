using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using static Sphere;

public class ModulContentPanelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI nameText;
    public Image imagePanel;
    public Mesh modulMesh;
    public Mesh baseSphereMesh;
    public MeshFilter selectedSphere;
    private Vector3 sphereStartSize;
    [HideInInspector] public GameObject modulToCreate;
    [HideInInspector] public int modulIndex;
    private Transform shipParent;
    public ModuleList moduleList;
    private ModuleStorage moduleStorage;
    private Selection selection;
    private Sphere sph;
    private HangarModul parentHangarModule;

    private void Start()
    {
        shipParent = GameObject.Find("Ship").GetComponent<Transform>();
        moduleStorage = shipParent.GetComponent<ModuleStorage>();
        selection = GameObject.Find("SelectionController").GetComponent<Selection>();
        sph = selectedSphere.GetComponent<Sphere>();
        parentHangarModule = sph.parentTransform.gameObject.GetComponent<HangarModul>();
    }

    public void UpdatePanel()
    {
        SetText(moduleList.moduls[modulIndex].moduleName);

        modulToCreate = moduleList.moduls[modulIndex].hangarPrefab;
    }

    public void SetText(string text)
    {
        nameText.text = text;
    }

    public void SetPanel(Sprite sprite)
    {
        imagePanel.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.1f);

        sphereStartSize = selectedSphere.transform.localScale;

        if (selectedSphere != null && modulMesh != null)
        {
            selectedSphere.transform.localScale = new Vector3(1f, 1f, 1f);
            selectedSphere.mesh = modulMesh;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.05f);

        if (selectedSphere != null && modulMesh != null)
        {
            selectedSphere.transform.localScale = sphereStartSize;
            selectedSphere.mesh = baseSphereMesh;
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        float spawnpos_x = 0;
        float spawnpos_z = 0;

        switch (sph.sphereSide)
        {
            case SphereSide.left:
                spawnpos_x = sph.spawnPositionX;
                spawnpos_z = sph.spawnPositionZ - 1;
                break;

            case SphereSide.right:
                spawnpos_x = sph.spawnPositionX;
                spawnpos_z = sph.spawnPositionZ + 1;
                break;

            case SphereSide.front:
                spawnpos_x = sph.spawnPositionX - 1;
                spawnpos_z = sph.spawnPositionZ;
                break;

            case SphereSide.back:
                spawnpos_x = sph.spawnPositionX + 1;
                spawnpos_z = sph.spawnPositionZ;

                break;
            case SphereSide.strafe:
                spawnpos_x = sph.spawnPositionX - 1 ;
                spawnpos_z = sph.spawnPositionZ;
                break;
        }

        GameObject go = Instantiate(modulToCreate, new Vector3(spawnpos_x, sph.parentTransform.position.y, spawnpos_z), Quaternion.Euler(0f, 0f, 0f));
        go.transform.SetParent(shipParent);
        HangarModul newHangarModule = go.GetComponent<HangarModul>();


        //sph.ControllSpheres();

        ModuleDataRuntime newModuleData = new();
        newModuleData.x = spawnpos_x;
        newModuleData.z = spawnpos_z;
        newModuleData.moduleTypeIndex = modulIndex;
        newModuleData.parentModule = parentHangarModule.moduleData;
        newModuleData.level = parentHangarModule.moduleData.level + 1;
        newHangarModule.moduleData = newModuleData;

        moduleStorage.installedModuleData.Add(newModuleData);
        moduleStorage.installedModulesGameobjects.Add(go);
        moduleStorage.RefreshModulSpheres();

        selection.DeselectAll();

    }
}

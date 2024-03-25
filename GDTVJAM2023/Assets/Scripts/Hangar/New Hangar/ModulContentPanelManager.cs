using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using static Sphere;
using Unity.VisualScripting;
using UnityEngine.Rendering.PostProcessing;

public class ModulContentPanelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI Controls")]
    public TextMeshProUGUI nameText;
    public Image moduleImagePanel;
    public Image modulContentPanel;

    [Header("Selection Controls")]
    public Color32 baseColor = new Color32(8,57,156,255);
    public Color32 selectionColor = new Color32(124,124,255,255);
    public MeshFilter selectedSphere;
    private Selection selectionManager;
    private Sphere sph;

    [Header("Create new Mesh Controls")]
    public int modulIndex;
    private GameObject modulToCreate;
    private Transform shipParent;
    private ModuleStorage moduleStorage;
    private HangarModul parentHangarModule;
    private ModuleList moduleList;


    /* **************************************************************************** */
    /* LIFECYCLE------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Lifecycle
    private void Start()
    {
        // find GameObjects
        shipParent = GameObject.Find("Ship").GetComponent<Transform>();
        selectionManager = GameObject.Find("SelectionController").GetComponent<Selection>();

        sph = selectedSphere.GetComponent<Sphere>();
        parentHangarModule = sph.parentTransform.gameObject.GetComponent<HangarModul>();

        moduleStorage = shipParent.GetComponent<ModuleStorage>();
        moduleList = moduleStorage.moduleList;

        // Update the Panel view
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        // Header
        nameText.text = moduleList.moduls[modulIndex].moduleName;

        // Modul Sprite
        if (moduleList.moduls[modulIndex].modulSprite != null)
        moduleImagePanel.sprite = moduleList.moduls[modulIndex].modulSprite;

        // Hangar Modul Prefab
        modulToCreate = moduleList.moduls[modulIndex].hangarPrefab;

    }
    #endregion



    /* **************************************************************************** */
    /* INTERFACES------------------------------------------------------------------ */
    /* **************************************************************************** */
    #region Interfaces
    // Handle Mouse over UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.01f, 1.01f, 1.01f), 0.1f);

        modulContentPanel.color = selectionColor;
    }

    // Handle Mouse exit UI
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.05f);

        modulContentPanel.color = baseColor;
    }

    // Handle Mouse klick on UI
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

        selectionManager.DeselectAll();
    }
    #endregion
}

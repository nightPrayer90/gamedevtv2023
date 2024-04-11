using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using static Sphere;


public class ModulContentPanelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI Controls")]
    public TextMeshProUGUI nameText;
    public Image moduleImagePanel;
    public Image modulContentPanel;

    [Header("Selection Controls")]
    public Color32 baseColor = new Color32(8, 57, 156, 255);
    public Color32 selectionColor = new Color32(124, 124, 255, 255);
    public MeshFilter selectedSphere;
    private Selection selectionManager;
    private Sphere sph;

    [Header("Create new Mesh Controls")]
    public int modulIndex;
    private GameObject modulToCreate;
    private Transform shipParent;
    private ModuleStorage moduleStorage;
    public HangarModul parentHangarModule;
    private ModuleList moduleList;
    private HangarUIController hangarUIController;



    /* **************************************************************************** */
    /* LIFECYCLE------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Lifecycle
    private void Start()
    {
        // find GameObjects
        shipParent = GameObject.Find("Ship").GetComponent<Transform>();
        selectionManager = GameObject.Find("SelectionController").GetComponent<Selection>();
        hangarUIController = selectionManager.gameObject.GetComponent<HangarUIController>();

        if (selectedSphere != null)
        {
            sph = selectedSphere.GetComponent<Sphere>();
            parentHangarModule = sph.parentTransform.gameObject.GetComponent<HangarModul>();
        }

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

    private void OnDestroy()
    {
        hangarUIController.MouseExitModulePanel(0f);
        gameObject.transform.DOComplete();
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
        hangarUIController.MouseOverModulePanel(modulIndex);
    }

    // Handle Mouse exit UI
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.05f);

        modulContentPanel.color = baseColor;
        hangarUIController.MouseExitModulePanel(0.2f);
    }

    // Handle Mouse klick on UI
    public void OnPointerClick(PointerEventData eventData)
    {
        float spawnpos_x = 0;
        float spawnpos_z = 0;
        GameObject go = null;
        ModuleDataRuntime newModuleData = new();

        // Handle Sphere Select
        if (sph != null)
        {
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
                    spawnpos_x = sph.spawnPositionX - 1;
                    spawnpos_z = sph.spawnPositionZ;
                    break;
            }
            // create a new Module
            go = Instantiate(modulToCreate, new Vector3(spawnpos_x, parentHangarModule.transform.position.y, spawnpos_z), Quaternion.Euler(0f, 0f, 0f));
            go.transform.SetParent(shipParent);
        }
        else
        {
            // handle Module select
            // delete the old module
            moduleStorage.HangarChangeModule();

            // create a new Module
            go = Instantiate(modulToCreate, parentHangarModule.transform.position, Quaternion.Euler(0f, 0f, 0f));
            go.transform.SetParent(shipParent);

        }
        newModuleData.x = go.transform.position.x;
        newModuleData.z = go.transform.position.z;
        newModuleData.moduleTypeIndex = modulIndex;

        HangarModul newHangarModule = go.GetComponent<HangarModul>();
        newHangarModule.moduleData = newModuleData;

        // set all Module values
        newHangarModule.moduleValues = moduleList.moduls[modulIndex].moduleValues;

        // set inspector Modlue values
        newHangarModule.moduleValues.moduleName = moduleList.moduls[modulIndex].moduleName;
        newHangarModule.moduleValues.moduleType = moduleList.moduls[modulIndex].moduleType;

        newHangarModule.moduleValues.canLeft = moduleList.moduls[modulIndex].canLeft;
        newHangarModule.moduleValues.canRight = moduleList.moduls[modulIndex].canRight;
        newHangarModule.moduleValues.canFront = moduleList.moduls[modulIndex].canFront;
        newHangarModule.moduleValues.canBack = moduleList.moduls[modulIndex].canBack;

        moduleStorage.installedModuleData.Add(newModuleData);
        moduleStorage.installedHangarModules.Add(newHangarModule);
        moduleStorage.BuildModuleGrid();
        moduleStorage.RefreshModulSpheres();

        hangarUIController.SetShipPanel();

        moduleStorage.playerData.moduleCounts[modulIndex] -= 1;

        int ran = Random.Range(0, 4);
        switch (ran)
        {
            case 0: AudioManager.Instance.PlaySFX("HangarBuild1"); break;
            case 1: AudioManager.Instance.PlaySFX("HangarBuild2"); break;
            case 2: AudioManager.Instance.PlaySFX("HangarBuild3"); break;
            case 3: AudioManager.Instance.PlaySFX("HangarBuild4"); break;
        }



        selectionManager.DeselectAll();
    }
    #endregion
}

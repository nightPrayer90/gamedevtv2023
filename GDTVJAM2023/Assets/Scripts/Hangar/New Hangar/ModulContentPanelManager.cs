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
    public TextMeshProUGUI quantityText;
    public Image moduleImagePanel;
    public Image moduleContentPanel;

    [Header("Selection Controls")]
    public Color32 baseColor = new Color32(8, 57, 156, 255);
    public Color32 selectionColor = new Color32(124, 124, 255, 255);
    [HideInInspector] public MeshFilter selectedSphere;
    private HangarSelection selectionManager;
    private Sphere sph;
    private MeshRenderer shpMR;

    [Header("Create new Mesh Controls")]
    [HideInInspector] public int moduleIndex;
    private GameObject moduleToCreate;
    private GameObject moduleGhost;
    private GameObject moduleCreatedGhost;
    private Transform shipParent;
    private ModuleStorage moduleStorage;
    [HideInInspector] public Transform parentHangarModuleTransform;
    private ModuleList moduleList;
    private HangarUIController hangarUIController;

    public bool isClickt = false;
    private HangarFilterBtn hangarFilterBtn;

    /* **************************************************************************** */
    /* LIFECYCLE------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Lifecycle
    private void Start()
    {
        // find GameObjects
        shipParent = GameObject.Find("Ship").GetComponent<Transform>();
        selectionManager = GameObject.Find("SelectionController").GetComponent<HangarSelection>();
        hangarUIController = selectionManager.gameObject.GetComponent<HangarUIController>();
        hangarFilterBtn = selectionManager.gameObject.GetComponent<HangarFilterBtn>();

        if (selectedSphere != null)
        {
            sph = selectedSphere.GetComponent<Sphere>();
            shpMR = selectedSphere.GetComponent<MeshRenderer>();
            //parentHangarModule = sph.parentTransform.gameObject.GetComponent<HangarModul>();
        }

        moduleStorage = shipParent.GetComponent<ModuleStorage>();
        moduleList = moduleStorage.moduleList;

        // Update the Panel view
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        // Content
        nameText.text = moduleList.moduls[moduleIndex].moduleName;

        quantityText.text = (moduleStorage.playerData.moduleCounts[moduleIndex] > 1) ? moduleStorage.playerData.moduleCounts[moduleIndex].ToString() : "";

        // Modul Sprite
        if (moduleList.moduls[moduleIndex].modulSprite != null)
            moduleImagePanel.sprite = moduleList.moduls[moduleIndex].modulSprite;

        // Hangar Modul Prefab
        moduleToCreate = moduleList.moduls[moduleIndex].hangarPrefab;
        moduleGhost = moduleList.moduls[moduleIndex].hangarGhost;
    }

    private void OnDestroy()
    {
        //TODO!!!
        if (hangarUIController != null)
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
        if (isClickt == false)
        {
            AudioManager.Instance.PlaySFX("MouseHover");

            gameObject.transform.DOComplete();
            gameObject.transform.DOScale(new Vector3(1.01f, 1.01f, 1.01f), 0.1f);

            moduleContentPanel.color = selectionColor;
            hangarUIController.MouseOverModulePanel(moduleIndex);


            // Create a GhostObject
            if (selectedSphere != null && moduleGhost != null)
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
                        spawnpos_x = sph.spawnPositionX - 1;
                        spawnpos_z = sph.spawnPositionZ;
                        break;
                }

                moduleCreatedGhost = Instantiate(moduleGhost, new Vector3(spawnpos_x, parentHangarModuleTransform.position.y, spawnpos_z), Quaternion.identity);
                shpMR.enabled = false;
            }
        }
    }

    // Handle Mouse exit UI
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClickt == false)
        {
            gameObject.transform.DOComplete();
            gameObject.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.05f);

            moduleContentPanel.color = baseColor;
            hangarUIController.MouseExitModulePanel(0.2f);

            // delete Ghost if exist
            if (moduleCreatedGhost != null)
            {
                Destroy(moduleCreatedGhost);
                shpMR.enabled = true;
            }
        }
    }

    // Handle Mouse klick on UI
    public void OnPointerClick(PointerEventData eventData)
    {
        hangarFilterBtn.DeactivatePanelInterface();

        float spawnpos_x = 0;
        float spawnpos_z = 0;
        GameObject go = null;
        ModuleDataRuntime newModuleData = new();

        // delete Ghost if exist
        if (moduleCreatedGhost != null)
        {
            Destroy(moduleCreatedGhost);
            shpMR.enabled = true;
        }

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
            go = Instantiate(moduleToCreate, new Vector3(spawnpos_x, parentHangarModuleTransform.position.y, spawnpos_z), Quaternion.Euler(0f, 0f, 0f));
            go.transform.SetParent(shipParent);
        }
        else
        {
            // handle Module select
            // delete the old module
            moduleStorage.HangarChangeModule();

            // create a new Module
            go = Instantiate(moduleToCreate, parentHangarModuleTransform.position, Quaternion.Euler(0f, 0f, 0f));
            go.transform.SetParent(shipParent);

        }
        newModuleData.x = go.transform.position.x;
        newModuleData.z = go.transform.position.z;
        newModuleData.moduleTypeIndex = moduleIndex;

        HangarModul newHangarModule = go.GetComponent<HangarModul>();
        newHangarModule.moduleData = newModuleData;

        // set all Module values
        newHangarModule.moduleValues = moduleList.moduls[moduleIndex].moduleValues;

        // set inspector Modlue values
        newHangarModule.moduleValues.moduleName = moduleList.moduls[moduleIndex].moduleName;
        newHangarModule.moduleValues.moduleType = moduleList.moduls[moduleIndex].moduleType;

        newHangarModule.moduleValues.canLeft = moduleList.moduls[moduleIndex].canLeft;
        newHangarModule.moduleValues.canRight = moduleList.moduls[moduleIndex].canRight;
        newHangarModule.moduleValues.canFront = moduleList.moduls[moduleIndex].canFront;
        newHangarModule.moduleValues.canBack = moduleList.moduls[moduleIndex].canBack;

        moduleStorage.installedModuleData.Add(newModuleData);
        moduleStorage.installedHangarModules.Add(newHangarModule);
        moduleStorage.BuildModuleGrid();
        moduleStorage.RefreshModulSpheres();

        hangarUIController.SetShipPanel();

        moduleStorage.playerData.moduleCounts[moduleIndex] -= 1;

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

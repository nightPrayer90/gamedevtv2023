using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

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

    private void Start()
    {
        shipParent = GameObject.Find("Ship").GetComponent<Transform>();
        moduleStorage = shipParent.GetComponent<ModuleStorage>();
        selection = GameObject.Find("SelectionController").GetComponent<Selection>();
    }

    public void UpdatePanel()
    {
       SetText( moduleList.moduls[modulIndex].moduleName);

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
        Sphere Sph = selectedSphere.GetComponent<Sphere>();
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);

        switch (Sph.sphereSide)
        {
            case Sphere.SphereSide.left:
                rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case Sphere.SphereSide.right:
                rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case Sphere.SphereSide.front:
                rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case Sphere.SphereSide.back:
                rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case Sphere.SphereSide.strafe:
                rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
        }

        GameObject go = Instantiate(modulToCreate, selectedSphere.transform.position, rotation);
        go.transform.SetParent(shipParent);

        selectedSphere.GetComponent<Sphere>().ControllSpheres();

        ModuleInstance newModuleInstance = new ModuleInstance();
        newModuleInstance.x = selectedSphere.transform.position.x;
        newModuleInstance.z = selectedSphere.transform.position.z;
        newModuleInstance.rotation = 0;
        newModuleInstance.moduleIndex = modulIndex;

        moduleStorage.baseModules.Add(newModuleInstance);
        moduleStorage.installedModules.Add(go);
        moduleStorage.RefreshModulSpheres();

        selection.DeselectAll();

    }
}

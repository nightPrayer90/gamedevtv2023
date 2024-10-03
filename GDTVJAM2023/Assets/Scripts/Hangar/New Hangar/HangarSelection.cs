using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class HangarSelection : MonoBehaviour
{
    public Material highligtMaterial;
    public Material selectionMaterial;

    public Material highligtSphereMaterial;
    public Material selectionSphereMaterial;

    private Material originalMaterial;
    private Material originalMaterialSelectet;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    private HangarUIController hangarUIController;
    private Transform lastSelection;
    private ModuleStorage moduleStorage;
    private HangarInputHandler hangarInputHandler;

    public event Action OnDeselect;

    private int curX;
    private int curZ;
    

    private void Awake()
    {
        hangarInputHandler = GetComponent<HangarInputHandler>();
        hangarInputHandler.OnNavigatePress += HangarInputHandler_OnNavigatePress;
        hangarUIController = GetComponent<HangarUIController>();
        moduleStorage = GameObject.Find("Ship").GetComponent<ModuleStorage>();
    }

    private void OnDestroy()
    {
        hangarInputHandler.OnNavigatePress -= HangarInputHandler_OnNavigatePress;
    }

    private void HangarInputHandler_OnNavigatePress(Vector2 v)
    {
        SelectModuleAtPosition((int)v.x + curX, (int)v.y + curZ);
    }

    private void Update()
    {
        if (highlight != null && highlight != selection)
        {
            highlight.GetComponent<MeshRenderer>().material = originalMaterial;
            highlight = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // handle mouse over
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;

            if (highlight != selection)
            {
                if (highlight.CompareTag("Selectable"))
                {
                    if (highlight.GetComponent<MeshRenderer>().material != highligtMaterial)
                    {
                        originalMaterial = highlight.GetComponent<MeshRenderer>().material;
                        highlight.GetComponent<MeshRenderer>().material = highligtMaterial;
                    }
                }
                else if (highlight.CompareTag("Sphere"))
                {
                    if (highlight.GetComponent<MeshRenderer>().material != highligtSphereMaterial)
                    {
                        originalMaterial = highlight.GetComponent<MeshRenderer>().material;
                        highlight.GetComponent<MeshRenderer>().material = highligtSphereMaterial;
                    }
                }
                else
                {
                    highlight = null;
                }
            }
        }

        // handle Mouse klick
        if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Physics.Raycast(ray, out raycastHit))
            {
                selection = raycastHit.transform;
              
                //ObjectDeselect();
                if (selection != lastSelection || lastSelection == null)
                {
                    // select a part of a ship
                    if (selection.CompareTag("Selectable"))
                    {
                        SelectModule(true);
                    }

                    // select a part modul sphere
                    else if (selection.CompareTag("Sphere"))
                    {
                        SelectSphere();
                    }

                    // klick at a free space
                    else
                    {
                        DeselectAll();
                    }
                }
            }


        }
    }

    public void SelectModuleAtPosition(int x, int z)
    {
        foreach (HangarModul hm in moduleStorage.installedHangarModules)
        {
            if(hm.moduleData.x == x && hm.moduleData.z == z)
            {
                foreach(Transform t in hm.gameObject.GetComponentsInChildren<Transform>())
                {
                    if(t.CompareTag("Selectable"))
                    {
                        selection = t;
                        SelectModule(false);
                        return;
                    }
                }
            }
        }
    }

    private void SelectModule(bool selectsound = true)
    {
        // deselect the old Object
        ObjectDeselect();

        // select the new Object
        if (highlight == selection)
        {
            originalMaterialSelectet = originalMaterial;
        }
        else
        {
            originalMaterialSelectet = selection.GetComponent<MeshRenderer>().material;
        }
        selection.GetComponent<MeshRenderer>().material = selectionMaterial;
        HangarModul hm = selection.GetComponentInParent<HangarModul>();
        moduleStorage.CreateReplacementList(hm);
        hm.SetActive();
        curX = (int)hm.moduleData.x;
        curZ = (int)hm.moduleData.z;

        hangarUIController.HandleModulSelect(hm, selectsound);
        lastSelection = selection;
    }

    private void SelectSphere()
    {
        // deselect the old Object
        ObjectDeselect();

        // select the new Object
        if (highlight == selection)
        {
            originalMaterialSelectet = originalMaterial;
        }
        else
        {
            originalMaterialSelectet = selection.GetComponent<MeshRenderer>().material;
        }
        selection.GetComponent<MeshRenderer>().material = selectionSphereMaterial;
        Sphere sph = selection.GetComponentInParent<Sphere>();

        moduleStorage.CreateModuleLists(sph.sphereSide);
        sph.SetActive();

        hangarUIController.HandleShpereSelect(sph); // TODO selection maybe = sph?
        lastSelection = selection;
    }

    public void DeselectAll()
    {
        if (lastSelection != null)
            lastSelection.GetComponentInChildren<MeshRenderer>().material = originalMaterialSelectet;
        selection = null;
        lastSelection = null;
        OnDeselect?.Invoke();
    }

    public void ObjectDeselect()
    {
        if (lastSelection != null)
            lastSelection.GetComponentInChildren<MeshRenderer>().material = originalMaterialSelectet;
        OnDeselect?.Invoke();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Selection : MonoBehaviour
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

    public event Action OnDeselect;
    

    private void Start()
    {
        hangarUIController = gameObject.GetComponent<HangarUIController>();
    }

    private void Update()
    {
        if (highlight != null && highlight != selection)
        {
            highlight.GetComponent<MeshRenderer>().material = originalMaterial;
            highlight = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
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
                        // deselect the old Object
                        ObjectDeselect();

                        // select the new Object
                        originalMaterialSelectet = originalMaterial;
                        selection.GetComponent<MeshRenderer>().material = selectionMaterial;
                        selection.GetComponentInParent<HangarModul>().SetActive();
                        hangarUIController.HandleModulSelect(selection);
                        lastSelection = selection;
                    }

                    // select a part modul sphere
                    else if (selection.CompareTag("Sphere"))
                    {
                        // deselect the old Object
                        ObjectDeselect();

                        // select the new Object
                        originalMaterialSelectet = originalMaterial;
                        selection.GetComponent<MeshRenderer>().material = selectionSphereMaterial;
                        selection.GetComponentInParent<Sphere>().SetActive();
                        hangarUIController.HandleShpereSelect(selection);
                        lastSelection = selection;
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

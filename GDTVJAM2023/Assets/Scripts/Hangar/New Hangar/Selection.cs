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

    public event Action OnDeselect;

    private void Update()
    {
        if (highlight != null && highlight != selection)
        {
            highlight.GetComponent<MeshRenderer>().material = originalMaterial;
            highlight = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;

            if (highlight != selection)
            {
                if (highlight.CompareTag("Selectable"))
                {
                    if (highlight.GetComponentInChildren<MeshRenderer>().material != highligtMaterial)
                    {
                        originalMaterial = highlight.GetComponentInChildren<MeshRenderer>().material;
                        highlight.GetComponentInChildren<MeshRenderer>().material = highligtMaterial;
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

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ObjectDeselect();

            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
            {
                selection = raycastHit.transform;
               
                if (selection.CompareTag("Selectable"))
                {
                    Debug.Log(selection);

                    originalMaterialSelectet = originalMaterial;
                    selection.GetComponentInChildren<MeshRenderer>().material = selectionMaterial;
                    selection.GetComponentInParent<HangarModul>().SetActive();
                }
                else if (selection.CompareTag("Sphere"))
                {
                    originalMaterialSelectet = originalMaterial;
                    selection.GetComponent<MeshRenderer>().material = selectionSphereMaterial;
                    selection.GetComponentInParent<Sphere>().SetActive();
                }
                else
                {
                    selection = null;
                    OnDeselect?.Invoke();
                }
            }

           
        }

    }

    public void ObjectDeselect()
    {
        if (selection != null)
        {
            selection.GetComponentInChildren<MeshRenderer>().material = originalMaterialSelectet;
            selection = null;
            OnDeselect?.Invoke();
        }
    }
}

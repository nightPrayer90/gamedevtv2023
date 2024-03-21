using System.Collections.Generic;
using UnityEngine;

public class HangarModul : MonoBehaviour
{
    public bool isActiv = false;
    private Selection selectionController;
    private ModuleStorage moduleStorage;

    private float startX;
    private float startZ;

    public int listPosition = -1;

    public List<Sphere> spheres;

    private void Start()
    {
        startX = transform.position.x;
        startZ = transform.position.z;

        selectionController = GameObject.Find("SelectionController").GetComponent<Selection>();
        selectionController.OnDeselect += HandleSetDeselect;

        moduleStorage = gameObject.GetComponentInParent<ModuleStorage>();

        // find Position in List
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

    // Controll function, if an installed Modul was deleted
   public void ControllDelete()
    {
        foreach (Sphere sph in spheres)
        {
            sph.ControllSpheres();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HangarModul : MonoBehaviour
{
    public bool isActiv = false;
    private Selection selectionController;

    public List<GameObject> Spheres;

    private void Start()
    {
        selectionController = GameObject.Find("SelectionController").GetComponent<Selection>();
        selectionController.OnDeselect += HandleSetDeselect;
    }

    public void HandleSetDeselect()
    {
        if (isActiv == true)
        {
            isActiv = false;
            Debug.Log(isActiv);
        }
    }

    public void SetActive()
    {
        isActiv = true;
        Debug.Log(isActiv);
    }
}

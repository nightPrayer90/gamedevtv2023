using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KlickOnObject : MonoBehaviour
{
    public Material material1;
    public Material material2;
    public Material material3;
    private MeshRenderer meshRenderer;

    private bool klickFlag = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnMouseEnter()
    {
        Debug.Log("Enter");
        if (klickFlag == false)
            meshRenderer.material = material2;
    }

    private void OnMouseExit()
    {
        Debug.Log("Exit");
        if (klickFlag == false)
            meshRenderer.material = material1;
    }

    private void OnMouseDown()
    {
        Debug.Log("Down");
        if (klickFlag == false)
        {
            meshRenderer.material = material3;
            klickFlag = true;
        }
        else
            klickFlag = false;
    }
}

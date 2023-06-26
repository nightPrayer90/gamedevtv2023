using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEnemyReplacement : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    private Material[] meshMaterials;
    private float counter;
    public float dissolveRate= 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        if (meshRenderer != null)
            meshMaterials = meshRenderer.materials;

        counter = meshMaterials[0].GetFloat("_DissolveAmount");

        InvokeRepeating("FadeOutwithDissolve", 0.0f, 0.01f);
    }


    private void FadeOutwithDissolve()
    {
        if (meshMaterials[0].GetFloat("_DissolveAmount") < 2)
        {
            counter += dissolveRate;
            meshMaterials[0].SetFloat("_DissolveAmount", counter);
            meshMaterials[1].SetFloat("_DissolveAmount", counter);
        }
        else
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

}

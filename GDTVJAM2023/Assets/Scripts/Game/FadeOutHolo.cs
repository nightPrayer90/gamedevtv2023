using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutHolo : MonoBehaviour
{
    public float minDelay = 1.0f;
    public float maxDelay = 3.0f;
    private Rigidbody[] childRigidbodys;


    private List<MeshRenderer> childRenderers = new List<MeshRenderer>();
    private List<int> randomOrder = new List<int>();

    void Start()
    {
        childRigidbodys = GetComponentsInChildren<Rigidbody>();

        foreach (var rb in childRigidbodys)
        {
            rb.AddExplosionForce(150, rb.position, 2);
        }

        // Sammle alle MeshRenderer-Komponenten der Kindobjekte
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
        childRenderers.AddRange(renderers);

        // Erstelle eine Liste mit den Indizes, die die Reihenfolge der Kindobjekte repräsentieren
        for (int i = 0; i < childRenderers.Count; i++)
        {
            randomOrder.Add(i);
        }

        // Mische die Liste der Indizes, um eine zufällige Reihenfolge zu erhalten
        Shuffle(randomOrder);

        // Rufe die Coroutine für jedes MeshRenderer auf, um sie in zufälliger Reihenfolge auszublenden
        foreach (int index in randomOrder)
        {
            StartCoroutine(HideMeshRendererWithDelay(childRenderers[index]));
        }

        Invoke("DeleteParent", maxDelay + 1f);
    }

    IEnumerator HideMeshRendererWithDelay(MeshRenderer renderer)
    {
        // Warte für eine zufällige Verzögerungszeit, bevor das MeshRenderer ausgeblendet wird
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        // Blende das MeshRenderer aus
        renderer.enabled = false;
    }

    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void DeleteParent()
    {
        Destroy(gameObject);
    }
}
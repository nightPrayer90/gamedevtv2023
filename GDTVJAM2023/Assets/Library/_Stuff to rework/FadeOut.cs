using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float fadeSpeed = 0.5f; // Geschwindigkeit des Ausblendens
    public bool canExplode = false;

    private Renderer[] childRenderers;
    private Rigidbody[] childRigidbodys;
    private Color[] targetColors;
    private bool fadingOut;

    private void Start()
    {
        // randomize fadingtime
        fadeSpeed += Random.Range(-0.2f, 0.2f);

        // Renderer-Komponenten aller Child-Objekte abrufen
        childRenderers = GetComponentsInChildren<Renderer>();

        // Ziel-Farben-Array initialisieren
        targetColors = new Color[childRenderers.Length];
        for (int i = 0; i < childRenderers.Length; i++)
        {
            Material[] materials = childRenderers[i].materials;

            // Materialen auf den transparenten Rendering-Modus setzen
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                materials[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                materials[j].SetInt("_ZWrite", 0);
                materials[j].DisableKeyword("_ALPHATEST_ON");
                materials[j].EnableKeyword("_ALPHABLEND_ON");
                materials[j].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                materials[j].renderQueue = 3000;

                targetColors[i] = materials[j].color;
            }
        }

        if (canExplode == true)
        {
            childRigidbodys = GetComponentsInChildren<Rigidbody>();

            foreach (var rb in childRigidbodys)
            {
                rb.AddExplosionForce(300, rb.position, 2);
            }
        }

        StartFadeOut();
    }

    private void Update()
    {
        if (fadingOut)
        {
            // Alpha-Wert aller Materialien schrittweise verringern
            for (int i = 0; i < childRenderers.Length; i++)
            {
                targetColors[i].a -= fadeSpeed * Time.deltaTime;

                // Überprüfen, ob der Alpha-Wert unter 0 liegt, um das GameObject zu deaktivieren
                if (targetColors[i].a <= 0f)
                {
                    Destroy(gameObject);
                    return;
                }

                // Neue Farbe dem Material zuweisen
                Material[] materials = childRenderers[i].materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    Color color = materials[j].color;
                    color.a = targetColors[i].a;
                    materials[j].color = color;
                }
            }
        }
    }

    public void StartFadeOut()
    {
        fadingOut = true;
    }
}

using UnityEngine;
using TMPro;

public class EffectsManager : MonoBehaviour
{
    public GameObject textPrefab;
    public static void DoFloatingText(Vector3 position, string text, Color c)
    {
        EffectsManager effectsManager = FindObjectOfType<EffectsManager>();
        GameObject floatingText = Instantiate(effectsManager.textPrefab, position, Quaternion.LookRotation(Camera.main.transform.forward));
        floatingText.GetComponent<TMP_Text>().color = c;
        floatingText.GetComponent<DamagePopup>().displayText = text;
    }
    // You can call this from anywhere by calling EffectsManager.DoFloatingText(position, text, c);
}

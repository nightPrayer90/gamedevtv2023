using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    [TextArea(5, 20)]
    public string textToType;
    public string typingSoundAudio;
    public string menueScene;
    public float typingSpeed = 0.1f;
    public TMP_Text displayText;
    private bool isTyping = true;

    public float transitionDuration = 1f; // Dauer des Blendeffekts
    public Image fadePanel;

    private void Start()
    {
        displayText.text = "";
        StartCoroutine(TypeText());
        AudioManager.Instance.PlaySFX("WindowOpen");
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (isTyping && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Beenden des Tippens, um den gesamten Text anzuzeigen
            StopAllCoroutines();
            displayText.text = textToType;
            isTyping = false;
            AudioManager.Instance.PlaySFX("MouseKlick");
        }
        else if (!isTyping && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Zur MenuScene wechseln
            AudioManager.Instance.PlaySFX("DimensionSwap");
            StartFade();
        }
    }

    private System.Collections.IEnumerator TypeText()
    {
        foreach (char letter in textToType)
        {
            displayText.text += letter;
            PlayTypingSound();
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void PlayTypingSound()
    {
        if (typingSoundAudio != "")
        {
            AudioManager.Instance.PlaySFX(typingSoundAudio);
        }
    }

    private void StartFade()
    {
        fadePanel.gameObject.SetActive(true); // Aktiviere das Panel

        // Starte den Blendeffekt
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color panelColor = fadePanel.color;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / transitionDuration;

            // Aktualisiere den Alpha-Wert des Panels
            panelColor.a = Mathf.Lerp(0f, 1f, normalizedTime);
            fadePanel.color = panelColor;

            yield return null;
        }

        // Lade die n�chste Szene
        SceneManager.LoadScene(menueScene);
    }

}
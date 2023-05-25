using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    public Vector3 cameraOffset_;
    private Vector3 cameraOffset;

    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.1f;
    private float shakeIntensity_;

    public GameManager gameManager;
    private float shakeTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        cameraOffset = transform.position - cameraOffset_;

        shakeIntensity_ = shakeIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        // Blockt alle Screenshakes
        if (!gameManager.gameIsPlayed || gameManager.gameOver)
            shakeTimer = 0f;
        
        if (shakeTimer > 0f)
        {
            // Zufällige Verschiebung der Kamera-Position
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            transform.localPosition = player.transform.position + cameraOffset + randomOffset;

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.position = player.transform.position + cameraOffset;
        }
    }

        
    public void ShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration;
        shakeIntensity = shakeIntensity_;
    }

    public void BigShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration*10;
        shakeIntensity = shakeIntensity_*3;
    }

    public void BigShortShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration * 5;
        shakeIntensity = shakeIntensity_ * 2;
    }

    public void LongShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration * 70;
        shakeIntensity = shakeIntensity_ * 0.3f;
    }
}

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

    public GameManager gameManager;
    private float shakeTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        cameraOffset = transform.position - cameraOffset_;
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}

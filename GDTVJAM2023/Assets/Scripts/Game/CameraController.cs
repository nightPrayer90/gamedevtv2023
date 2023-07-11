using UnityEngine;
using DG.Tweening;


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

    private bool isShake = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cameraOffset = transform.position - cameraOffset_;

        shakeIntensity_ = shakeIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        /*
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
            
        }*/
        //if (isShake == false)
            transform.position = player.transform.position + cameraOffset;

    }


    public void ShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration;
        shakeIntensity = shakeIntensity_;

        if (isShake == false)
        {
            isShake = true;
            transform.DOShakeRotation(shakeTimer, new Vector3(shakeIntensity, shakeIntensity, shakeIntensity), 30, 90f, true, ShakeRandomnessMode.Full).OnComplete(() => { isShake = false; });
        }
    }

    public void BigShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration*7;
        shakeIntensity = shakeIntensity_*1.5f;

        if (isShake == false)
        {
            isShake = true;
            transform.DOShakeRotation(shakeTimer, new Vector3(shakeIntensity, shakeIntensity, shakeIntensity), 30, 90f, true, ShakeRandomnessMode.Full).OnComplete(() => { isShake = false; });
        }
    }

    public void BigShortShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration * 5;
        shakeIntensity = shakeIntensity_ * 1.5f;

        if (isShake == false)
        {
            isShake = true;
            transform.DOShakeRotation(shakeTimer, new Vector3(shakeIntensity, shakeIntensity, shakeIntensity), 30, 90f, true, ShakeRandomnessMode.Full).OnComplete(() => { isShake = false; });
        }
    }

    public void LongShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration * 48;
        shakeIntensity = shakeIntensity_ * 0.4f;

        if (isShake == false)
        {
            isShake = true;
            transform.DOShakeRotation(shakeTimer, new Vector3(shakeIntensity, shakeIntensity, shakeIntensity), 30, 90f, true, ShakeRandomnessMode.Full).OnComplete(() => { isShake = false; });
        }
    }
}

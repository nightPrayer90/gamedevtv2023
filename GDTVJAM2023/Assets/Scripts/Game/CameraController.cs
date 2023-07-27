using UnityEngine;
using DG.Tweening;


public class CameraController : MonoBehaviour
{
    private GameObject player;
    private float moveOffsetY=0;
    public Vector3 startOffset;
    public Vector3 cameraOffset_;
    private Vector3 cameraOffset;

    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.1f;
    private float shakeIntensity_;

    public GameManager gameManager;
    public Transform mainCameraTr;
    public bool flyModeToggle = true;
    private float shakeTimer = 0f;

    private bool isShake = false;
    private bool isMoving = false;
    private bool toggleSwith = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        shakeIntensity_ = shakeIntensity;


        cameraOffset = transform.position - startOffset;

        DOTween.To(() => moveOffsetY, x => moveOffsetY = x, 3.5f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        // position emptys
        
        if (flyModeToggle == true)
        {
            transform.position = player.transform.position + new Vector3(cameraOffset.x, cameraOffset.y + moveOffsetY, cameraOffset.z);
            mainCameraTr.position = transform.position;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            mainCameraTr.rotation = Quaternion.Euler(70f, 0f, 0f);
        }
        else
        {
            // postion empty
            transform.position = player.transform.position;
            // roation empty
            transform.rotation = player.transform.rotation;


            if (toggleSwith == false)
            {
                

                // position camera
                mainCameraTr.localPosition = new Vector3(0, 2f, 2f);
                
                mainCameraTr.localRotation = Quaternion.Euler(30, 180 , 0);
                
                toggleSwith = true;
            }
        }
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

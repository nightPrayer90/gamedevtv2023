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
    public Camera mainCamera;
    public bool flyModeToggle = true;
    private float shakeTimer = 0f;

    private bool isShake = false;
    private bool toggleSwith = false;
    private PlayerController playerController;
    public CanvasGroup toggelViewText;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        shakeIntensity_ = shakeIntensity;


        cameraOffset = transform.position - startOffset;

        DOTween.To(() => moveOffsetY, x => moveOffsetY = x, 3.5f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        // position emptys

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isShake == false)
            {
                if (flyModeToggle)
                    flyModeToggle = false;
                else
                    flyModeToggle = true;
            }
            else
            {
                toggelViewText.DOFade(1,0.1f);
                Invoke("InvokeToggleViewText", 1.0f);
            }
        }


        if (flyModeToggle == true) // normal view
        {
            mainCamera.farClipPlane = 20f;
            transform.position = player.transform.position + new Vector3(cameraOffset.x, cameraOffset.y + moveOffsetY, cameraOffset.z);
            

            if (toggleSwith == true)
            {
                AudioManager.Instance.PlaySFX("ViewChange");
                mainCameraTr.DOLocalMove(new Vector3(0f,0f,0f),0.5f);
                //mainCameraTr.position = transform.position;
                //transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.5f);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            
                mainCameraTr.rotation = Quaternion.Euler(70f, 0f, 0f);
                toggleSwith = false;
            }
        }
        else // first person view
        {
            mainCamera.farClipPlane = 1000f;
            transform.position = player.transform.position;


            transform.rotation = Quaternion.RotateTowards(transform.rotation, player.transform.rotation, Time.deltaTime * 150f);

            var rotationZ =  (playerController.currentRotationX + 90)*0.2f;
            mainCameraTr.localRotation = Quaternion.Euler(30, 180, rotationZ);  

            if (toggleSwith == false)
            {
                AudioManager.Instance.PlaySFX("ViewChange");
                transform.rotation = player.transform.rotation;
                // position camera

                mainCameraTr.DOLocalMove(new Vector3(0, 2f, 2f), 0.5f);
                //mainCameraTr.localPosition = new Vector3(0, 2f, 2f);

                mainCameraTr.localRotation = Quaternion.Euler(30, 180, 0);

                toggleSwith = true;
            }
        }
    }

    private void InvokeToggleViewText()
    {
        toggelViewText.DOFade(0,0.5f);
    }
   
    // TODOO - ScreenShake auf main Camera
    public void ShakeScreen()
    {
        // Starte den Screen Shake
        shakeTimer = shakeDuration*2;
        shakeIntensity = shakeIntensity_;

        if (isShake == false)
        {
            isShake = true;
            if (flyModeToggle == true) mainCameraTr.DOShakeRotation(shakeTimer, new Vector3(shakeIntensity, shakeIntensity, shakeIntensity), 30, 90f, true, ShakeRandomnessMode.Full).OnComplete(() => { isShake = false; });
            else mainCameraTr.DOShakePosition(shakeTimer, shakeIntensity/5).OnComplete(() => { isShake = false; });
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
            if (flyModeToggle == true) mainCameraTr.DOShakeRotation(shakeTimer, new Vector3(shakeIntensity, shakeIntensity, shakeIntensity), 30, 90f, true, ShakeRandomnessMode.Full).OnComplete(() => { isShake = false; });
            else mainCameraTr.DOShakePosition(shakeTimer, shakeIntensity / 5).OnComplete(() => { isShake = false; });
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
            if (flyModeToggle == true) mainCameraTr.DOShakeRotation(shakeTimer, new Vector3(shakeIntensity, shakeIntensity, shakeIntensity), 30, 90f, true, ShakeRandomnessMode.Full).OnComplete(() => { isShake = false; });
            else mainCameraTr.DOShakePosition(shakeTimer, shakeIntensity / 5).OnComplete(() => { isShake = false; });
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
            if (flyModeToggle == true) mainCameraTr.DOShakeRotation(shakeTimer, new Vector3(shakeIntensity, shakeIntensity, shakeIntensity), 30, 90f, true, ShakeRandomnessMode.Full).OnComplete(() => { isShake = false; });
            else mainCameraTr.DOShakePosition(shakeTimer, shakeIntensity / 5).OnComplete(() => { isShake = false; });
        }
    }

       

}

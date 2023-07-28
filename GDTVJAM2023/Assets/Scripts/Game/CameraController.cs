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
    private float timer = 0f;
    public float interpolationDuration = 2f;
    private PlayerController playerController;

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
            if (flyModeToggle)
                flyModeToggle = false;
            else
                flyModeToggle = true;
        }


        if (flyModeToggle == true)
        {
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
        else
        {
            // postion empty
            transform.position = player.transform.position;


            //float var = Mathf.Lerp(transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.y,t);
            //Debug.Log(transform.rotation.eulerAngles.y + " - " + player.transform.rotation.eulerAngles.y + " = " + var);

            //Quaternion targetRotation = Quaternion.Euler(0f, player.transform.rotation.eulerAngles.y, 0f);


            transform.rotation = Quaternion.RotateTowards(transform.rotation, player.transform.rotation, Time.deltaTime * 150f);

            var rotationZ =  (playerController.currentRotationX + 90)*0.2f;
            mainCameraTr.localRotation = Quaternion.Euler(30, 180, rotationZ);  

            Debug.Log(playerController.currentRotationX);

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

using UnityEngine;

public class ab_FrontShield : MonoBehaviour
{
    public GameObject shieldprefab;
    public float realoadTime = 10f;
    public bool shieldFlag = true;
    public Sprite abSprite; // gameManager use this for UI

    private NewPlayerController playerController;
    private GameManager gameManager;
    private ab_ShieldController enabledShieldController;

    private void Awake()
    {
        //playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = gameObject.GetComponentInParent<NewPlayerController>();

        gameManager.InitAbilityUI(abSprite);
        SetShieldFlag();
    }


    private void Update()
    {
        if (shieldFlag == false && playerController.abilityInput == true)
        {
            SpawnFrontShield();
            shieldFlag = true;
        }
    }

    public void SpawnFrontShield()
    {
        // FrontShield
        if (enabledShieldController == null)
        {
            GameObject go = Instantiate(shieldprefab, transform.position, transform.rotation); // Quaternion.Euler(0f, 0f, 0f) * 
            //playerWaeponController.frontShield_ = go;
            enabledShieldController = go.GetComponent<ab_ShieldController>();
            //enabledShieldController.ShieldEnable(playerWaeponController.fsShieldLife);
        }

        if (enabledShieldController != null)
        {
            enabledShieldController.ShieldEnable(3);
            gameManager.SetAbilityUItoZero();
        }
    }

    public void SetShieldFlag()
    {
        Invoke(nameof(InvokeShieldFlag), realoadTime);
        gameManager.SetAbilityValue(realoadTime);
    }

    private void InvokeShieldFlag()
    {
        AudioManager.Instance.PlaySFX("AbilityIsReady");
        shieldFlag = false;
    }
}

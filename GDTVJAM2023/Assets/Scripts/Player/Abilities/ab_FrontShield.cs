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
    private PlayerWeaponController playerWeaponController;

    private void Start()
    {
        //playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = gameObject.GetComponentInParent<NewPlayerController>();
        playerWeaponController = gameObject.GetComponentInParent<PlayerWeaponController>();

        gameManager.InitAbilityUI(abSprite);
        SetShieldFlag();

        // activate shield upgrades
        UpgradeChooseList uCl = gameManager.GetComponent<UpgradeChooseList>();
        uCl.upgrades[40].upgradeStartCount = uCl.uLObject.upgradeList[40].UpgradeCount; // Fortified Defense
        uCl.upgrades[41].upgradeStartCount = uCl.uLObject.upgradeList[41].UpgradeCount; // Shield Strike
        uCl.upgrades[62].upgradeStartCount = uCl.uLObject.upgradeList[62].UpgradeCount; // Explosive Shield
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
            gameManager.ScreenShake(5);
            enabledShieldController.ShieldEnable();
            gameManager.SetAbilityUItoZero();
        }
    }

    public void SetShieldFlag()
    {
        float realoadTime_ = (1 - playerWeaponController.shipData.mcSupportLvl * 0.1f) * realoadTime;
        Invoke(nameof(InvokeShieldFlag), realoadTime_);
        gameManager.SetAbilityValue(realoadTime_);
    }

    private void InvokeShieldFlag()
    {
        AudioManager.Instance.PlaySFX("AbilityIsReady");
        shieldFlag = false;
    }
}

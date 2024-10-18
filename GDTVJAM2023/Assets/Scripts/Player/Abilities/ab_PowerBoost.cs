using System.Collections.Generic;
using UnityEngine;

public class ab_PowerBoost : MonoBehaviour
{
    public float realoadTime = 10f;
    public bool reloadFlag = true;
    public Sprite abSprite; // gameManager use this for UI

    private NewPlayerController playerController;
    private PlayerWeaponController playerWeaponController;
    private GameManager gameManager;
    private Rigidbody playerRb;
    [SerializeField] private List<NewBaseEngine> baseEngines;

    public Collider hitCollider;
    public ParticleSystem hitMarker;
    public string abilityName = "";

    private void Start()
    {
        //playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = gameObject.GetComponentInParent<NewPlayerController>();
        playerWeaponController = gameObject.GetComponentInParent<PlayerWeaponController>();

        playerRb = playerController.gameObject.GetComponent<Rigidbody>();

        gameManager.InitAbilityUI(abSprite, "Power Boost", realoadTime);
        SetReloadFlag();

        
        // activate boost upgrades
        //UpgradeChooseList uCl = gameManager.gameObject.GetComponent<UpgradeChooseList>();
        //uCl.upgrades[31].upgradeStartCount = uCl.uLObject.upgradeList[31].UpgradeCount; // Fortified Overdrive
        //uCl.upgrades[23].upgradeStartCount = uCl.uLObject.upgradeList[23].UpgradeCount; // Guardian Drive
        //uCl.upgrades[24].upgradeStartCount = uCl.uLObject.upgradeList[24].UpgradeCount; // Force Multiplier
    }

    
    private void Update()
    {
        if (reloadFlag == false && playerController.abilityInput == true)
        {
            ActivatePowerBoost();
            reloadFlag = true;
        }
    }

    public void ActivatePowerBoost()
    {
        if (baseEngines.Count == 0)
        {
            baseEngines = new List<NewBaseEngine>(playerController.gameObject.GetComponentsInChildren<NewBaseEngine>());
        }

        if (gameManager.upgradeChooseList.upgrades[27].upgradeIndexInstalled == 1)
        {
            playerController.NovaOnHit(2.5f, playerWeaponController.shipData.boostDamage);
        }

        SetBoostShield(true);

        gameManager.ScreenShake(5);
        AudioManager.Instance.PlaySFX("PlayerBoostKick");

        foreach (NewBaseEngine engine in baseEngines)
        {
            engine.ps_boostParticle.Emit(150);
            engine.ps_boostEngine.Emit(30);
            engine.powerBoosParticle.Play();
        }

        if (baseEngines[0].useBoost == true) playerRb.AddForce(-transform.right * baseEngines[0].totalThrustForce * 20, ForceMode.Force);
        else playerRb.AddForce(-transform.right * baseEngines[0].totalThrustForce * 100, ForceMode.Force);

        playerController.GetInvulnerability();
        gameManager.SetAbilityUItoZero();

        Invoke(nameof(SetReloadFlag), playerWeaponController.shipData.boostInvulnerability);


    }

    private void OnTriggerEnter(Collider other)
    {
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }

        if (other.gameObject.CompareTag(tagStr))
        {
            other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(800f, transform.position, 1);
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();


            if (gameManager.upgradeChooseList.upgrades[42].upgradeIndexInstalled == 1)
            {
                int damage = enemyHealth.CritDamage(playerWeaponController.shipData.boostDamage);
                enemyHealth.TakeDamage(damage);
                enemyHealth.ShowDamageFromObjectsColor(damage, enemyHealth.critColor);
            }
            else
            {
                int damage = playerWeaponController.shipData.boostDamage;
                enemyHealth.TakeDamage(damage);
                enemyHealth.ShowDamageFromObjects(damage);
            }
            if (gameManager.upgradeChooseList.upgrades[61].upgradeIndexInstalled == 1)
            {
                enemyHealth.InvokeBurningDamage();
                enemyHealth.burnTickCount = 0;
            }
        }

    }

    public void SetReloadFlag()
    {
        SetBoostShield(false);

        foreach (NewBaseEngine engine in baseEngines)
        {
            engine.powerBoosParticle.Stop();
        }

        float realoadTime_ = realoadTime * (1 - playerWeaponController.shipData.mcSupportLvl * 0.1f);

        Invoke(nameof(InvokePowerBoostFlag), realoadTime_);
        gameManager.SetAbilityValue(realoadTime_);
    }

    private void InvokePowerBoostFlag()
    {
        AudioManager.Instance.PlaySFX("AbilityIsReady");
        reloadFlag = false;
        gameManager.SetAbilityGlow(true);

        foreach (NewBaseEngine engine in baseEngines)
        {
            engine.ps_boostParticle.Emit(100);
        }
    }

    private void SetBoostShield(bool status)
    {
        if (status == true)
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, playerWeaponController.shipData.boostSize);
            hitCollider.enabled = true;
            hitMarker.Play();
        }
        else
        {
            hitCollider.enabled = false;
            hitMarker.Stop();
        }
    }
}

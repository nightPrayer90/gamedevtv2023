using DG.Tweening.Core.Easing;
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
    public GameObject hitMarker;


    private void Awake()
    {
        //playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = gameObject.GetComponentInParent<NewPlayerController>();
        playerWeaponController = gameObject.GetComponentInParent<PlayerWeaponController>();

        playerRb = playerController.gameObject.GetComponent<Rigidbody>();

        gameManager.InitAbilityUI(abSprite);
        SetReloadFlag();
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

            enemyHealth.TakeDamage(20);
            enemyHealth.ShowDamageFromObjects(20);
        }

    }

    public void SetReloadFlag()
    {
        SetBoostShield(false);

        foreach (NewBaseEngine engine in baseEngines)
        {
            engine.powerBoosParticle.Stop();
        }
        Invoke(nameof(InvokePowerBoostFlag), realoadTime);
        gameManager.SetAbilityValue(realoadTime);
    }

    private void InvokePowerBoostFlag()
    {
        AudioManager.Instance.PlaySFX("AbilityIsReady");
        reloadFlag = false;

        foreach (NewBaseEngine engine in baseEngines)
        {
            engine.ps_boostParticle.Emit(100);
        }
    }

    private void SetBoostShield(bool status)
    {
        if (status == true)
        {
            hitCollider.enabled = true;
            hitMarker.SetActive(true);
        }
        else
        {
            hitCollider.enabled = false;
            hitMarker.SetActive(false);
        }
    }
}

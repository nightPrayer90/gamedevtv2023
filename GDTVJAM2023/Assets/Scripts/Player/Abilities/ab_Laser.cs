using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ab_Laser : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public float realoadTime = 1f;
    public bool reloadFlag = true;
    public Sprite abSprite; // gameManager use this for UI
    public int projectileDamage = 5;

    private NewPlayerController playerController;
    public Transform projectilespawner;
    private GameManager gameManager;
    private Rigidbody playerRB;

    public ParticleSystem loadParticle;

    private void Awake()
    {
        //playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = gameObject.GetComponentInParent<NewPlayerController>();
        playerRB = gameObject.GetComponentInParent<Rigidbody>();

        gameManager.InitAbilityUI(abSprite);
        SetReloadFlag();
    }


    private void Update()
    {
        if (reloadFlag == false && playerController.abilityInput == true)
        {
            gameManager.ScreenShake(5);
            LoadPhase();
            gameManager.SetAbilityUItoZero();
            reloadFlag = true;
        }
    }

    public void LoadPhase()
    {
        AudioManager.Instance.PlaySFX("LaserBeamReload");
        loadParticle.transform.localScale = Vector3.zero;
        loadParticle.Play();
        loadParticle.transform.DOScale(new Vector3(2f, 2f, 2f), 2f);
        Invoke(nameof(ActivateProjectileStart),2f);
    }


    public void ActivateProjectileStart()
    {
        loadParticle.Stop();
        playerRB.AddForce(transform.right.normalized * 5f , ForceMode.Impulse);
        GameObject go = ObjectPoolManager.SpawnObject(ProjectilePrefab, projectilespawner.position, projectilespawner.rotation, ObjectPoolManager.PoolType.Gameobject);
        ab_LaserProjectile lP = go.GetComponent<ab_LaserProjectile>();
        lP.damage = projectileDamage;
        
        Invoke(nameof(SetReloadFlag), 2f);
    }

    public void SetReloadFlag()
    {
        Invoke(nameof(InvokeReloadFlag), realoadTime);
        gameManager.SetAbilityValue(realoadTime);
    }

    private void InvokeReloadFlag()
    {
        AudioManager.Instance.PlaySFX("AbilityIsReady");
        reloadFlag = false;
    }
}
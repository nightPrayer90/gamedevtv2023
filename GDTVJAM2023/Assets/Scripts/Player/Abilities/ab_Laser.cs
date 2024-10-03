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
    private PlayerWeaponController playerWeaponController;
    private UpgradeChooseList upgradeChooseList;

    public ParticleSystem loadParticle;
    private float loadingtime = 2;
    private float recoilForce = 5f;

    private void Start()
    {
        //playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.upgradeChooseList;
        playerController = gameObject.GetComponentInParent<NewPlayerController>();
        playerWeaponController = playerController.playerWeaponController;
        playerRB = playerController.playerRigidbody;

        gameManager.InitAbilityUI(abSprite);
        SetReloadFlag();

        // Shpere upgrades
        //UpgradeChooseList uCl = gameManager.gameObject.GetComponent<UpgradeChooseList>();
        //uCl.upgrades[92].upgradeStartCount = uCl.uLObject.upgradeList[92].UpgradeCount; // Fortified Overdrive
        //uCl.upgrades[93].upgradeStartCount = uCl.uLObject.upgradeList[93].UpgradeCount; // Guardian Drive
        //uCl.upgrades[94].upgradeStartCount = uCl.uLObject.upgradeList[94].UpgradeCount; // Force Multiplier
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
        switch (upgradeChooseList.upgrades[92].upgradeIndexInstalled)
        {
            case 1:
                loadingtime = 1f;
                recoilForce = 3f;
                break;

            case 2:
                loadingtime = 0.5f;
                recoilForce = 1f;
                break;
        }

        AudioManager.Instance.PlaySFX("LaserBeamReload");
        loadParticle.transform.localScale = Vector3.zero;
        loadParticle.Play();
        loadParticle.transform.DOScale(new Vector3(2f, 2f, 2f), loadingtime);
        Invoke(nameof(ActivateProjectileStart), loadingtime);
    }


    public void ActivateProjectileStart()
    {
        loadParticle.Stop();
        playerRB.AddForce(transform.right.normalized * recoilForce, ForceMode.Impulse);
        GameObject go = ObjectPoolManager.SpawnObject(ProjectilePrefab, projectilespawner.position, projectilespawner.rotation, ObjectPoolManager.PoolType.Gameobject);
        ab_LaserProjectile lP = go.GetComponent<ab_LaserProjectile>();
        lP.damage = projectileDamage;

        Invoke(nameof(SetReloadFlag), 2f);
    }

    public void SetReloadFlag()
    {
        float realoadTime_ = realoadTime * (1 - playerWeaponController.shipData.mcSupportLvl * 0.1f);

        Invoke(nameof(InvokeReloadFlag), realoadTime_);
        gameManager.SetAbilityValue(realoadTime_);
    }

    private void InvokeReloadFlag()
    {
        AudioManager.Instance.PlaySFX("AbilityIsReady");
        reloadFlag = false;
        gameManager.SetAbilityGlow(true);
    }
}

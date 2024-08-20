using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ab_InfernoRockets : MonoBehaviour
{
    public GameObject rocketPrefab;
    public int rocketSpawnCount;
    public float realoadTime = 1f;
    public bool reloadFlag = true;
    public Sprite abSprite; // gameManager use this for UI
    public int rocketDamage = 5;
    private int rocketSpawnCount_;

    private NewPlayerController playerController;
    private PlayerWeaponController playerWeaponController;
    public Transform rocketSpawner;
    private GameManager gameManager;


    private void Start()
    {
        //playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = gameObject.GetComponentInParent<NewPlayerController>();
        playerWeaponController = gameObject.GetComponentInParent<PlayerWeaponController>();

        gameManager.InitAbilityUI(abSprite);
        SetReloadFlag();

        // activate shield upgrades
        UpgradeChooseList uCl = gameManager.gameObject.GetComponent<UpgradeChooseList>();
        uCl.upgrades[36].upgradeStartCount = uCl.uLObject.upgradeList[36].UpgradeCount; // Rocket Overdrive
        uCl.upgrades[86].upgradeStartCount = uCl.uLObject.upgradeList[86].UpgradeCount; // Overheating Rockets
        uCl.upgrades[87].upgradeStartCount = uCl.uLObject.upgradeList[87].UpgradeCount; // Corpse explosion
    }


    private void Update()
    {
        if (reloadFlag == false && playerController.abilityInput == true)
        {
            AudioManager.Instance.PlaySFX("PlayerMineDetect");

            Invoke(nameof(RocketLoad), 0.5f);
            gameManager.SetAbilityUItoZero();
            reloadFlag = true;
        }
        transform.Rotate(0, 300f * Time.deltaTime, 0);
    }

    private void RocketLoad()
    {
        gameManager.ScreenShake(5);
        ActivateRocketStart();
    }


    public void ActivateRocketStart()
    {
        AudioManager.Instance.PlaySFX("PlayerRocketStart");
   
        GameObject go = ObjectPoolManager.SpawnObject(rocketPrefab, rocketSpawner.position, rocketSpawner.rotation, ObjectPoolManager.PoolType.Gameobject);
        RocketController rocket = go.GetComponent<RocketController>();
        rocket.damage = rocketDamage + playerWeaponController.shipData.extraDamage;
        rocket.SetDestroyTimer();
        rocket.isMainWeapon = false;

        rocketSpawnCount_++;

        if (rocketSpawnCount_ == (rocketSpawnCount+ playerWeaponController.shipData.extraRockets))
        {
            Invoke(nameof(SetReloadFlag), 2f);
            rocketSpawnCount_ = 0;
        }
        else
        {
            Invoke(nameof(ActivateRocketStart),0.1f);
        }
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
    }
}

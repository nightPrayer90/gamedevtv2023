using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public Transform rocketSpawner;
    private GameManager gameManager;


    private void Awake()
    {
        //playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = gameObject.GetComponentInParent<NewPlayerController>();

        gameManager.InitAbilityUI(abSprite);
        SetReloadFlag();
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
        rocket.damage = rocketDamage;
        rocket.SetDestroyTimer();
        rocket.isMainWeapon = false;

        rocketSpawnCount_++;

        if (rocketSpawnCount_ == rocketSpawnCount)
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
        Invoke(nameof(InvokeReloadFlag), realoadTime);
        gameManager.SetAbilityValue(realoadTime);
    }

    private void InvokeReloadFlag()
    {
        AudioManager.Instance.PlaySFX("AbilityIsReady");
        reloadFlag = false;
    }
}
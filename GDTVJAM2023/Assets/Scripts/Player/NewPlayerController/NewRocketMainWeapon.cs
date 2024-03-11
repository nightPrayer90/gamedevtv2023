using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRocketMainWeapon : MonoBehaviour
{
    [Header("Main Weapon")]
    public float fireRate;
    public int rockedBaseDamage;

    [Header("Rocked Settings")]
    public float detectionRange = 10f;
    [HideInInspector] public float lifeTime = 10f;
    public GameObject rockedToLaunch;
    public GameObject spawnPoint;
    private bool enemyDetected = false;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);
    
    //private Objects
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    private PlayerWeaponController playerWeaponController;
    private NewPlayerController playerController;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region Lifecycle Methoden
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.gameObject.GetComponent<UpgradeChooseList>();
        playerController = GetComponentInParent<NewPlayerController>();
        playerController.OnIntroOver += HandleStartShooting;
        playerWeaponController = GetComponentInParent<PlayerWeaponController>();
        playerWeaponController.OnMWDamage += HandleDamageUpdate;
    }

    public void HandleDamageUpdate(int damageToUpdate)
    {
        rockedBaseDamage += damageToUpdate;
    }

    private void HandleStartShooting()
    {
        InvokeRepeating("SpawnRocked", fireRate, fireRate);
    }
    #endregion



    /* **************************************************************************** */
    /* Main weapon----------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Main Weapon
    // update main Weapon base values from playerController
    public void UpdateBulletValues()
    {
 
        // rocket 
        float temp_fireRate = fireRate;
        //fireRate = playerController.playerFireRate;

        // restart Invoke
        if (fireRate != temp_fireRate)
        {
            CancelInvoke("SpawnRocked");
            InvokeRepeating("SpawnRocked", fireRate, fireRate);
        }
    }

    // the main weapon start to fire
    public void StartShooting()
    {
        // set the main weapon particle damage
        UpdateBulletValues();

        // start invoke for main weapons
         if (!IsInvoking("SpawnRocked"))
            InvokeRepeating("SpawnRocked", fireRate, fireRate);
    }

    // stop the main waepon fire
    public void StopShooting()
    {
        CancelInvoke("SpawnRocked");
    }
    #endregion



    /* **************************************************************************** */
    /* ROCKET---------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Rocked
    // in an enemy in range;
    private void DetectEnemyRocket()
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }

        // list all enemys with tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tagStr);

        // is any enemy in range?
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= detectionRange)
            {
                enemyDetected = true;
                break;
            }
        }
    }

    // Invoke, spawns a rocked after spawnInterval sec
    private void SpawnRocked()
    {
        // in an enemy in range;
        DetectEnemyRocket();

        // if an anemy detected
        if (enemyDetected == true)
        {
            GameObject go = ObjectPoolManager.SpawnObject(rockedToLaunch, spawnPoint.transform.position, Quaternion.Euler(0f, 0f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
            RocketController rocket = go.GetComponent<RocketController>();
            rocket.damage = rockedBaseDamage;
            rocket.hitColor = hitColor;
            rocket.SetDestroyTimer();
            rocket.isMainWeapon = true;

            AudioManager.Instance.PlaySFX("PlayerRocketStart");

            enemyDetected = false;
        }
    }
    #endregion
}

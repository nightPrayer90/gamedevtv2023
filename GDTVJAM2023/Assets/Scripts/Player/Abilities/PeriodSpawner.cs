using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodSpawner : MonoBehaviour
{
    [Header("Rocked Settings")]
    public int rocketDamage = 5;
    public float lifeTime = 10f;
    public float spawnInterval = 4f;
    public float detectionRange = 10f;
    public GameObject rockedToLaunch;
    public GameObject spawnPoint;
    public GameObject spawnPoint2;
    private bool enemyDetected = false;
    private GameManager gameManager;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);




    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();
        InvokeRepeating("SpawnRocked", spawnInterval, spawnInterval);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }




    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController
    private void StartValues()
    {
        PlayerWeaponController weaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        rocketDamage = weaponController.rlDamage;
        lifeTime = weaponController.rlLifeTime;
        spawnInterval = weaponController.rlReloadTime;
    }

    // in an enemy in range;
    private void DetectEnemy()
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
        DetectEnemy();

        // if an anemy detected
        if (enemyDetected == true)
        {
            // spawn rocket one
            GameObject go = ObjectPoolManager.SpawnObject(rockedToLaunch, spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
            RocketController rocket = go.GetComponent<RocketController>();
            rocket.damage = rocketDamage;
            rocket.hitColor = hitColor;
            rocket.maxLifeTime = lifeTime;
            rocket.SetDestroyTimer();

            // spwan rocket two
            go = ObjectPoolManager.SpawnObject(rockedToLaunch, spawnPoint2.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
            rocket = go.GetComponent<RocketController>();
            rocket.damage = rocketDamage;
            rocket.hitColor = hitColor;
            rocket.maxLifeTime = lifeTime;
            rocket.SetDestroyTimer();

            AudioManager.Instance.PlaySFX("PlayerRocketStart");

            enemyDetected = false;
        }
    }


}

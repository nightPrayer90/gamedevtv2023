using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMWController : MonoBehaviour
{
    public enum MWeapontyp
    {
        bullet,
        rocket,
        laser,
    }

    [Header("Main Weapon")]
    public MWeapontyp weaponType;
    [HideInInspector] public int bulletBaseDamage;
    [HideInInspector] public float fireRate;
    public List<ParticleSystem> mainWeapons = new List<ParticleSystem>();
    public List<ParticleBullet> particleBullets;
    public AudioSource mainWeaponSound;

    [Header("Rocked Settings")]
    public float lifeTime = 10f;
    public float detectionRange = 10f;
    public GameObject rockedToLaunch;
    public GameObject spawnPoint;
    public GameObject spawnPoint2;
    private bool enemyDetected = false;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);
    private bool spawnSideToggle = false;


    //private Objects
    private PlayerController playerController;
    private GameManager gameManager;


    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // bullet
        playerController = GetComponent<PlayerController>();
        fireRate = playerController.playerFireRate;

        UpdateBulletValues();

        // rocket

        InvokeRepeating("SpawnRocked", fireRate, fireRate);        
    }

    /* **************************************************************************** */
    /* Main weapon----------------------------------------------------------------- */
    /* **************************************************************************** */
    // update main Weapon base values from playerController
    public void UpdateBulletValues()
    {
        bulletBaseDamage = playerController.playerBulletBaseDamage;

        // update bullet values
        if (weaponType == MWeapontyp.bullet)
        {
            float temp_fireRate = fireRate;
            fireRate = playerController.playerFireRate;

            // restart Invoke
            if (fireRate != temp_fireRate)
            {
                CancelInvoke("ShotEmit");
                InvokeRepeating("ShotEmit", 0.3f, fireRate);
            }
            SetBulletDamage();
        }

        // update Rocket values
        else if (weaponType == MWeapontyp.rocket)
        {
            // rocket 
            float temp_fireRate = fireRate;
            fireRate = playerController.playerFireRate;

            // restart Invoke
            if (fireRate != temp_fireRate)
            {
                CancelInvoke("SpawnRocked");
                InvokeRepeating("SpawnRocked", fireRate, fireRate);
            }
        }
    }


    // the main weapon start to fire
    public void StartShooting()
    {
        // set the main weapon particle damage
        UpdateBulletValues();

        // start invoke for main weapons
        if (weaponType == MWeapontyp.bullet)
        {
            if (!IsInvoking("ShotEmit"))
                InvokeRepeating("ShotEmit", 0.5f, fireRate);
        }
        else if (weaponType == MWeapontyp.rocket)
        {
            if (!IsInvoking("SpawnRocked"))
                InvokeRepeating("SpawnRocked", fireRate, fireRate);
        }
    }

    // stop the main waepon fire
    public void StopShooting()
    {
        if (weaponType == MWeapontyp.bullet)
        {
            CancelInvoke("ShotEmit");
        }
        else if (weaponType == MWeapontyp.rocket)
        {
            CancelInvoke("SpawnRocked");
        }

    }

    /* **************************************************************************** */
    /* Bullet---------------------------------------------------------------------- */
    /* **************************************************************************** */

    // invoke function - make the main weapon fire
    void ShotEmit()
    {
        // shooting sound
        mainWeaponSound.Play();

        // emit 1 particle of each mainweapon
        foreach (ParticleSystem mainWeapon in mainWeapons)
        {
            if (mainWeapon != null)
                mainWeapon.Emit(1);
        }
    }

    // set the main weapon particle damage
    public void SetBulletDamage()
    {
        foreach (ParticleBullet particle in particleBullets)
        {
            particle.BulletSetDamage(bulletBaseDamage);
        }
    }



    /* **************************************************************************** */
    /* Rocket---------------------------------------------------------------------- */
    /* **************************************************************************** */
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
            spawnSideToggle = !spawnSideToggle;

            if (spawnSideToggle)
            {
                // spawn rocket one
                GameObject go = ObjectPoolManager.SpawnObject(rockedToLaunch, spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
                RocketController rocket = go.GetComponent<RocketController>();
                rocket.damage = bulletBaseDamage;
                rocket.hitColor = hitColor;
                rocket.maxLifeTime = lifeTime;
            }
            else
            {
                // spwan rocket two
                GameObject go = ObjectPoolManager.SpawnObject(rockedToLaunch, spawnPoint2.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
                RocketController rocket = go.GetComponent<RocketController>();
                rocket.damage = bulletBaseDamage;
                rocket.hitColor = hitColor;
                rocket.maxLifeTime = lifeTime;
            }


            AudioManager.Instance.PlaySFX("PlayerRocketStart");

            enemyDetected = false;
        }
    }

}

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


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

    [Header("Laser Settings")]
    public float laserRange = 5f;
    public int laserShootTime = 3;
    public float spawnInterval = 0.1f;
    public string audioClip = "";
    private float nextSpawnTime = 0f;
    public LineRenderer lr;
    public LineRenderer lr2;
    public ParticleSystem hitParticle;
    public ParticleSystem hitParticle2;
    public ParticleSystem muzzleParticle;
    public ParticleSystem muzzleParticle2;
    public bool laserIsEnable = false;
    public Transform LaserSpawnPoint1;
    public Transform LaserSpawnPoint2;
    private Color whiteZero = new Color(1f, 1f, 1f, 0f);
    private  Color whiteStart = new Color(1f, 1f, 1f, 0.8f);
    private Color whiteEnd = new Color(1f, 1f, 1f, 0.3f);

    //private Objects
    private PlayerController playerController;
    private GameManager gameManager;


    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GetComponent<PlayerController>();

        // bullet
        fireRate = playerController.playerFireRate;

        UpdateBulletValues();

        // rocket
        if (weaponType == MWeapontyp.rocket)
            InvokeRepeating("SpawnRocked", fireRate, fireRate);

    }

    private void Update()
    {
        if (weaponType == MWeapontyp.laser)
            LaserShooting();
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
                CancelInvoke("BulletShotEmit");
                InvokeRepeating("BulletShotEmit", 0.3f, fireRate);
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

        //update Laser values
        else if (weaponType == MWeapontyp.laser)
        {
            // set damage to particle system
            foreach (ParticleBullet weapon in particleBullets)
            {
                weapon.bulletDamage = bulletBaseDamage;
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
            if (!IsInvoking("BulletShotEmit"))
                InvokeRepeating("BulletShotEmit", 0.5f, fireRate);
        }
        else if (weaponType == MWeapontyp.rocket)
        {
            if (!IsInvoking("SpawnRocked"))
                InvokeRepeating("SpawnRocked", fireRate, fireRate);
        }
        else if (weaponType == MWeapontyp.laser)
        {
            if (!IsInvoking("StartLaserShooting") || IsInvoking("StopLaserShooting"))
                StartLaserShooting();
        }
    }

    // stop the main waepon fire
    public void StopShooting()
    {
        if (weaponType == MWeapontyp.bullet)
        {
            CancelInvoke("BulletShotEmit");
        }
        else if (weaponType == MWeapontyp.rocket)
        {
            CancelInvoke("SpawnRocked");
        }

    }

    /* **************************************************************************** */
    /* BULLET---------------------------------------------------------------------- */
    /* **************************************************************************** */

    // invoke function - make the main weapon fire
    void BulletShotEmit()
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
    /* ROCKET---------------------------------------------------------------------- */
    /* **************************************************************************** */
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




    /* **************************************************************************** */
    /* LASER ---------------------------------------------------------------------- */
    /* **************************************************************************** */
    // shooting controller
    void LaserShooting()
    {
        if (laserIsEnable == true)
        {
            SetLaserLRPosition();
            LaserRaycast();
            LaserRaycast2();

            if (Time.time >= nextSpawnTime)
            {
                // shooting sound
                AudioManager.Instance.PlaySFX(audioClip);

                // emit 1 particle of each weapon
                foreach (ParticleSystem weapon in mainWeapons)
                {
                    if (weapon != null)
                        weapon.Emit(1);
                }

                nextSpawnTime = Time.time + spawnInterval;
            }

        }
    }

    private void StopLaserShooting()
    {
        Invoke("StartLaserShooting", fireRate);

        muzzleParticle.Stop();
        muzzleParticle2.Stop();

        //FadeOut
        /*
        lr.DOColor(new Color2(whiteStart, whiteEnd), new Color2(whiteZero, whiteZero), 0.5f).OnComplete(() =>
        { lr.enabled = false; });

        lr2.DOColor(new Color2(whiteStart, whiteEnd), new Color2(whiteZero, whiteZero), 0.5f).OnComplete(() =>
        { lr2.enabled = false; laserIsEnable = false; });
        */

        //lr.enabled = false;
        //lr2.enabled = false;
        laserIsEnable = false;
    }

 

    // realod a salve of weapons
    void StartLaserShooting()
    {
        Invoke("StopLaserShooting", laserShootTime);

        muzzleParticle.Play();
        muzzleParticle2.Play();


        // FadeIn
        lr.DOColor(new Color2(whiteZero, whiteZero), new Color2(whiteStart, whiteEnd), 0.5f).OnComplete(() =>
        { lr.DOColor(new Color2(whiteStart, whiteEnd), new Color2(whiteZero, whiteZero), laserShootTime-0.5f).SetEase(Ease.InExpo); ; }) ;

        lr2.DOColor(new Color2(whiteZero, whiteZero), new Color2(whiteStart, whiteEnd), 0.5f).OnComplete(() =>
        { lr2.DOColor(new Color2(whiteStart, whiteEnd), new Color2(whiteZero, whiteZero), laserShootTime-0.5f).SetEase(Ease.InExpo); }); ;

        //lr.enabled = true;
        //lr2.enabled = true;
        laserIsEnable = true;
    }

    private void LaserRaycast()
    {
        lr.SetPosition(0, LaserSpawnPoint1.position);

        float raycastDistance = laserRange; // Die maximale Entfernung des Raycasts
        int layerMask = (1 << 6) | (1 << 9); // Bitmaske für Render-Layer 6 und 9

        // laser 1
        RaycastHit hit;
        if (Physics.Raycast(LaserSpawnPoint1.position, -LaserSpawnPoint1.forward, out hit, raycastDistance, layerMask))
        {
            
            // Kollision mit einem Objekt auf den gewünschten Render-Layern
            GameObject collidedObject = hit.collider.gameObject;

            lr.SetPosition(1, collidedObject.transform.position);

            Vector3 dir = LaserSpawnPoint1.position - collidedObject.transform.position;

            hitParticle.transform.position = collidedObject.transform.position + dir.normalized * .2f;
            if (!hitParticle.isPlaying)
                hitParticle.Play();
        }
        else
        {
            lr.SetPosition(1, LaserSpawnPoint1.position - LaserSpawnPoint1.forward * raycastDistance);
            hitParticle.Stop();
        }
    }

    private void LaserRaycast2()
    {
        lr2.SetPosition(0, LaserSpawnPoint2.position);

        float raycastDistance = laserRange; // Die maximale Entfernung des Raycasts
        int layerMask = (1 << 6) | (1 << 9); // Bitmaske für Render-Layer 6 und 9

        // laser 2
        RaycastHit hit;
        if (Physics.Raycast(LaserSpawnPoint2.position, -LaserSpawnPoint2.forward, out hit, raycastDistance, layerMask))
        {

            // Kollision mit einem Objekt auf den gewünschten Render-Layern
            GameObject collidedObject = hit.collider.gameObject;

            lr2.SetPosition(1, collidedObject.transform.position);

            Vector3 dir = LaserSpawnPoint2.position - collidedObject.transform.position;

            hitParticle.transform.position = collidedObject.transform.position + dir.normalized * .2f;
            if (!hitParticle.isPlaying)
                hitParticle.Play();
        }
        else
        {
            lr2.SetPosition(1, LaserSpawnPoint2.position - LaserSpawnPoint2.forward * raycastDistance);
            hitParticle.Stop();
        }
    }

    private void SetLaserLRPosition()
    {
        lr.SetPosition(0, LaserSpawnPoint1.position);
        lr.SetPosition(1, LaserSpawnPoint1.position + LaserSpawnPoint1.forward * laserRange);

        lr2.SetPosition(0, LaserSpawnPoint2.position);
        lr2.SetPosition(1, LaserSpawnPoint2.position + LaserSpawnPoint1.forward * laserRange);
    }
}

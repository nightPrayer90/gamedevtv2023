using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Objects")]
    public GameObject explosionObject;
    public GameObject collisionExplosionObject;
    public GameObject expOrb;
    public GameObject classPickup;
    public GameObject miniMapIcon;
    public ParticleSystem engineParticle;
    private Collider enemyCollider;
    public GameObject novaOnDie;

    [Header("Enemy Settings")]
    public float enemyHealth = 2.0f;
    [HideInInspector]public float enemyStartHealth;
    public int collisonDamage = 1;
    public float explosionForce = 5.0f;
    public bool expOrbSpawn = false;
    public bool secondDimensionEnemy = false;
    public bool canTakeDamage = true;
    public bool canPoolObject = true;
    public event EventHandler DieEvent;
    public bool isBoss = false;
    public bool isMine = false;

    [Header("Enemy Weapons")]
    public List<EnemyParticleBullet> enemyWeapons;
    public List<ParticleSystem> enemyWeaponParticles;
    public int bulletDamage;
    public float fireRate;
    private bool isShooting = false;
    private AudioSource audioSource;


    [Header("Collision Control")]
    public List<ParticleCollisionEvent> collisionEvents;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);
    public Color critColor = new Color(1f, 0.6f, 0.0f, 1f);
    private Color resultColor;
    private bool isdied = false;


    [Header("AOE Damage Control")]
    public GameObject _replacement;
    private float startCollisionMultiplier = 64;
    private float collisionMultiplier = 64;


    [Header("Laser burning Control")]
    public GameObject _burnReplacement;
    public ParticleSystem burnParticleSystem;
    public Color burningColor = new Color(1f, 0.0f, 0.01f, 1f);
    private bool isBurning = false;
    private int burnTickCount = 0;
    public bool[] canTakeLaserDamage = new bool[5];


    // gameObjects to find
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    private PlayerWeaponController playerWeaponController;
    



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        if (bulletDamage != 0) audioSource = GetComponent<AudioSource>();

        collisionMultiplier += startCollisionMultiplier + UnityEngine.Random.Range(-16, 128);
        enemyStartHealth = enemyHealth;

        enemyCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        isShooting = false;
        enemyHealth = enemyStartHealth;
        isBurning = false;
        if (canPoolObject == true)
            canTakeDamage = true;

        burnTickCount = 0;
        CancelInvoke();
        if (burnParticleSystem != null)
            burnParticleSystem.Stop();

        if (engineParticle != null)
            engineParticle.Play();

        isdied = false;
        canTakeLaserDamage[0] = true; //burning Damage
        canTakeLaserDamage[1] = true; // MW Laser 1
        canTakeLaserDamage[2] = true; // MW Laser 2
        canTakeLaserDamage[3] = true; // front Laser
        canTakeLaserDamage[4] = true; // Orbital Laser
    }

    private void Start()
    {
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
    }

    private void Update()
    {
        if (isdied == false)
        {
            if (gameManager.dimensionShift == !secondDimensionEnemy)
            {
                enemyCollider.enabled = false;
                if (miniMapIcon != null) miniMapIcon.SetActive(false);
                if (bulletDamage > 0) StopShooting();
                if (engineParticle != null) engineParticle.Stop();
            }
            else
            {
                if (isShooting == false)
                {
                    enemyCollider.enabled = true;
                    if (miniMapIcon != null) miniMapIcon.SetActive(true);
                    if (bulletDamage > 0) StartShooting();
                    
                }
                if (engineParticle != null && engineParticle.isPlaying == false) engineParticle.Play();
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (canTakeDamage == true)
        {
            ParticleSystem part = other.GetComponent<ParticleSystem>(); // *** important! Making a variable to acess the particle system of the emmiting object, in this case, the lasers from my player ship.
            var ps = other.GetComponent<ParticleBullet>();
            int damage = ps.bulletDamage;
            int damagetyp = ps.damageTyp;
            resultColor = hitColor;

            // damage from a bullet
            if (damagetyp == 0)
            {
                // calculate crit damage

                int ran = UnityEngine.Random.Range(0, 100);
                if (ran < playerWeaponController.bulletCritChance)
                {
                    damage = CritDamage(damage);
                    resultColor = critColor;
                    NovaOnDie(1);
                }

                TakeDamage(damage, 1); //damagetyp = 1 = bullet Damage
            }

            
            int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

            foreach (ParticleCollisionEvent collisionEvent in collisionEvents) //  for each collision, do the following:
            {
                Vector3 pos = collisionEvent.intersection; // the point of intersection between the particle and the enemy
                gameManager.DoFloatingText(pos, damage.ToString(), resultColor);
            }
        }
    }
   
    /* **************************************************************************** */
    /* TAKE DAMAGE CONTROL--------------------------------------------------------- */
    /* **************************************************************************** */

    // take damage from a bullet
    public void TakeDamage(int damage, int damageTyp = 0)
    {
        AudioManager.Instance.PlaySFX("ImpactShot");

        //if (canTakeDamage)
        {

            // calculate Enemy Health
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // cancle all Invokes
                CancelInvoke();

                // drop an Item
                Drop();

                // calculate chance of explosion
                NovaOnDie(0);

                // update player UI
                if (secondDimensionEnemy == false && isMine == false)
                {
                    gameManager.UpdateEnemyCounter(-1);
                    gameManager.UpdateEnemyToKill(1);
                }

                // Trigger Explosion
                ObjectPoolManager.SpawnObject(collisionExplosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

                Die();

           }
        }
    }

    // take damage from an explosion
    public void TakeExplosionDamage(int damage)
    {
        if (canTakeDamage == true)
        {
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // cancle all Invokes
                CancelInvoke();

                // drop an Item
                Drop();

                // create object to die effect
                if (_replacement != null)
                {
                    var replacement = Instantiate(_replacement, transform.position, transform.rotation);

                    // make the die effect object explode
                    var rbs = replacement.GetComponentsInChildren<Rigidbody>();
                    foreach (var rb in rbs)
                    {
                        rb.AddExplosionForce(collisionMultiplier, transform.position, 1);  //collision.contacts[0].point;
                    }
                }

                // update player UI
                if (secondDimensionEnemy == false)
                {
                    gameManager.UpdateEnemyCounter(-1);
                    gameManager.UpdateEnemyToKill(1);
                }

                Die();
            }
        }
    }


    // take damage from a laser
    private IEnumerator InvokeCanGetLaserDamage(int index)
    {
        yield return new WaitForSeconds(0.3f);
        canTakeLaserDamage[index] = true;
    }

    public void TakeLaserDamage(int damage, int index)
    {
        if (canTakeLaserDamage[index] == true)
        {
            AudioManager.Instance.PlaySFX("PlayerLaserHit");
            canTakeLaserDamage[index] = false;
            if (gameObject.activeSelf)
                StartCoroutine(InvokeCanGetLaserDamage(index));

            // calculate burning damage
            int ran = UnityEngine.Random.Range(0, 100);
            if (ran < playerWeaponController.burnDamageChance && isBurning == false)
            {
                isBurning = true;
                InvokeBurningDamage();
            }

            //if (canTakeDamage)
            {
                enemyHealth -= damage;

                if (enemyHealth <= 0)
                {
                    // cancle all Invokes
                    CancelInvoke();

                    // drop an Item
                    Drop();

                    // create object to die effect
                    if (_burnReplacement != null)
                    {
                        Instantiate(_burnReplacement, transform.position, transform.rotation);
                    }

                    // update player UI
                    if (secondDimensionEnemy == false)
                    {
                        gameManager.UpdateEnemyCounter(-1);
                        gameManager.UpdateEnemyToKill(1);
                    }

                    //die sound
                    AudioManager.Instance.PlaySFX("PlayerLaserDie");

                    Die();
                }
            }
        }
    }

    public void InvokeBurningDamage()
    {
        if (burnParticleSystem != null)
            burnParticleSystem.Play();
        InvokeRepeating("TakeBurningDamage", .2f, 1f);
    }

    public void TakeBurningDamage()
    {
        int burningDamage = Mathf.CeilToInt( upgradeChooseList.baseLaserTickDamage * (upgradeChooseList.laserBurningTickDamangePercent)/100);

        ShowDamageFromObjectsColor(burningDamage, burningColor);
        TakeLaserDamage(burningDamage,0);

        burnTickCount++;

        if (burnTickCount > upgradeChooseList.baseLaserTicks)
        {
            CancelInvoke("TakeBurningDamage");
            burnTickCount = 0;
            isBurning = false;
            if (burnParticleSystem != null)
                burnParticleSystem.Stop();
        }
    }

    private void Die()
    {
        canTakeDamage = false;
        isdied = true;
        burnParticleSystem.Stop();

        if (DieEvent != null) {
            DieEvent.Invoke(this, new EventArgs());
            return;
        }

        // instanstiate explosion
        if (explosionObject != null)
            ObjectPoolManager.SpawnObject(explosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

        // stop EngineParticleEffect
        if (engineParticle != null)
            engineParticle.Stop();

        // pool (destroy) enemy object
        if (canPoolObject == true)
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        else
            Destroy(gameObject);
    }

    // nova on die Ability
    private void NovaOnDie(int novaTyp) //0=die 1=crit
    {
        if (novaOnDie != null && (upgradeChooseList.upgrades[52].upgradeIndexInstalled == 1 || upgradeChooseList.upgrades[23].upgradeIndexInstalled == 1))
        {
            Vector3 pos = new Vector3(0,0,0);
            float explosionRadius = 0;
            int novaDamage = 0;

            switch (novaTyp)
            {
                case 0: // Nova triggert from die
                    if (UnityEngine.Random.Range(0, 100) > 10)
                    {
                        return;
                    }
                    pos = transform.position;

                    explosionRadius = 1.5f + playerWeaponController.rocketAOERadius;
                    novaDamage = 10;

                    break;

                case 1: // Nova triggert from crit
                    if (UnityEngine.Random.Range(0, 100) > 5)
                    {
                        return;
                    }
                    pos = transform.position;

                    explosionRadius = 0.5f + playerWeaponController.rocketAOERadius;
                    novaDamage = 6;

                    break;
            }

            LayerMask layerMask = (1 << 6);
            if (gameManager.dimensionShift == true)
            {
                layerMask = (1 << 9);
            }

            // Audio
            AudioManager.Instance.PlaySFX("Playernova");


            // array of all Objects in the explosionRadius
            var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

            foreach (var obj in surroundingObjects)
            {
                // get rigidbodys from all objects in range
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null) continue;

                // calculate distance between explosioncenter and objects in Range
                float distance = Vector3.Distance(pos, rb.transform.position);

                if (distance < explosionRadius)
                {
                    resultColor = hitColor;
                    float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
                    int adjustedDamage = Mathf.CeilToInt(novaDamage * scaleFactor);

                    if (upgradeChooseList.upgrades[54].upgradeIndexInstalled > 0)
                    {
                        int ran = UnityEngine.Random.Range(0, 100);
                        if (ran < playerWeaponController.bulletCritChance)
                        {
                            adjustedDamage = CritDamage(adjustedDamage);
                            resultColor = critColor;
                        }
                    }

                    // get EnemyHealthscript
                    EnemyHealth eHC = obj.GetComponent<EnemyHealth>();

                    if (eHC != null)
                    {
                        // show floating text
                        if (eHC.canTakeDamage == true)
                        gameManager.DoFloatingText(rb.transform.position,adjustedDamage.ToString(), resultColor);

                        // calculate enemy damage
                        eHC.TakeExplosionDamage(adjustedDamage);
                    }
                }
            rb.AddExplosionForce(400, pos, explosionRadius);
            }

            GameObject go = ObjectPoolManager.SpawnObject(novaOnDie, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
            go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;
        }
    }

    public int CritDamage(int damage)
    {
        damage = Mathf.CeilToInt(damage * ((float)playerWeaponController.bulletCritDamage / 100));
        //resultColor = critColor;
        return damage;
    }



    /* **************************************************************************** */
    /* SHOOTING CONTROL------------------------------------------------------------ */
    /* **************************************************************************** */
    public void StartShooting()
    {
        if (isShooting == false)
        {
            foreach (EnemyParticleBullet particle in enemyWeapons)
            {
                particle.bulletDamage = bulletDamage;
            }

            InvokeRepeating("InvokeShooting", 1f, fireRate);
        }
        isShooting = true;
    }

    public void InvokeShooting()
    {
        foreach (ParticleSystem particle in enemyWeaponParticles)
        {
            particle.Emit(1);
        }
        if (audioSource != null)
        {
            audioSource.volume = AudioManager.Instance.sfxVolume;
            audioSource.Play();
        }
    }

    public void StopShooting()
    {
        CancelInvoke("InvokeShooting");
        isShooting = false;
    }

    public void ShowDamageFromObjects(int damage)
    {
        Vector3 pos = transform.position; // the point of intersection between the particle and the enemy
        gameManager.DoFloatingText(pos, damage.ToString(), hitColor);
    }

    public void ShowDamageFromObjectsColor(int damage, Color hitColor_)
    {
        Vector3 pos = transform.position; // the point of intersection between the particle and the enemy
        gameManager.DoFloatingText(pos, damage.ToString(), hitColor_);
    }

    public void ShowDamageFromPosition(Vector3 pos, int damage)
    {
        gameManager.DoFloatingText(pos, damage.ToString(),new Color(1f, 0.6f, 0.0f, 1f));
    }

    private void Drop()
    {
        if (expOrbSpawn)
        {
            int ran = UnityEngine.Random.Range(0, 100);

            if (ran >= 0 || classPickup == null)
            {
                ObjectPoolManager.SpawnObject(expOrb, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);
            }
            else
            {
                ObjectPoolManager.SpawnObject(classPickup, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);
            }
        }
    }
}

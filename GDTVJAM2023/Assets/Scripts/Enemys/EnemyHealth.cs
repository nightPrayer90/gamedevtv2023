using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Objects")]
    public GameObject explosionObject;
    public GameObject dieExplosionObject;
    public GameObject expOrb;
    public GameObject miniMapIcon;


    [Header("Enemy Settings")]
    public float enemyHealth = 2.0f;
    private float enemyHealthTemp;
    public int collisonDamage = 1; 
    public float explosionForce = 5.0f;
    public bool expOrbSpawn = false;
    public bool secondDimensionEnemy = false;
    public bool canTakeDamage = true;
    public bool canPoolObject = true;


    [Header("Enemy Weapons")]
    public List<EnemyParticleBullet> enemyWeapons;
    public int bulletDamage;
    private bool isShooting = false;

    [Header("Collision Control")]
    public List<ParticleCollisionEvent> collisionEvents;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);
    public Color critColor = new Color(1f, 0.6f, 0.0f, 1f);
    private Color resultColor;


    [Header("AOE Damage Control")]
    public GameObject _replacement;
    private float startCollisionMultiplier = 64;
    private float collisionMultiplier = 64;

    [Header("Laser burning Control")]
    public GameObject _burnReplacement;

    // gameObjects to find
    private GameManager gameManager;
    private PlayerWeaponController playerWeaponController;




    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        collisionMultiplier += startCollisionMultiplier + Random.Range(-16, 128);
        enemyHealthTemp = enemyHealth;
    }

    private void OnEnable()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        isShooting = false;
        enemyHealth = enemyHealthTemp;
        

        
    }

    private void Start()
    {
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
    }

    private void Update()
    {
        if (gameManager.dimensionShift == !secondDimensionEnemy)
        {
            GetComponent<Collider>().enabled = false;
            if (miniMapIcon != null) miniMapIcon.SetActive(false);
            StopShooting();
        }
        else
        {
            if (isShooting == false)
            {
                GetComponent<Collider>().enabled = true;
                if (miniMapIcon != null) miniMapIcon.SetActive(true);
                //StartShooting();
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
                int ran = Random.Range(0, 100);
                if (ran < playerWeaponController.bulletCritChance)
                {
                    damage = Mathf.RoundToInt(damage * playerWeaponController.bulletCritDamage / 100);
                    resultColor = critColor;
                }

                TakeDamage(damage);
            }
            else if (damagetyp == 1)
            {
                // damage from Laser
                TakeLaserDamage(damage);
            }


            int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

            foreach (ParticleCollisionEvent collisionEvent in collisionEvents) //  for each collision, do the following:
            {
                Vector3 pos = collisionEvent.intersection; // the point of intersection between the particle and the enemy
                gameManager.DoFloatingText(pos, "+" + damage.ToString(), resultColor);
            }
        }
    }
   
    /* **************************************************************************** */
    /* TAKE DAMAGE CONTROL--------------------------------------------------------- */
    /* **************************************************************************** */

    // take damage from a bullet
    public void TakeDamage(int damage)
    {
        AudioManager.Instance.PlaySFX("ImpactShot");

        if (canTakeDamage)
        {

            // calculate Enemy Health
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // drop an Item
                if (expOrbSpawn)
                    ObjectPoolManager.SpawnObject(expOrb, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

                // instanstiate explosion
                ObjectPoolManager.SpawnObject(explosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
                
                // update player UI
                if (secondDimensionEnemy == false)
                {
                    gameManager.UpdateEnemyCounter(-1);
                    gameManager.UpdateEnemyToKill(1);
                }

         
                // pool (destroy) enemy object
                if (canPoolObject == true)
                    ObjectPoolManager.ReturnObjectToPool(gameObject);
                else
                    Destroy(gameObject);
               
            }
        }
    }

    // take damage from an explosion
    public void TakeExplosionDamage(int damage)
    {
        if (canTakeDamage)
        {
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // drop an Item
                if (expOrbSpawn)
                    ObjectPoolManager.SpawnObject(expOrb, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

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

                // pool (destroy) enemy object
                if (canPoolObject == true)
                    ObjectPoolManager.ReturnObjectToPool(gameObject);
                else
                    Destroy(gameObject);
                
            }
        }
    }

    // take damage from a laser
    public void TakeLaserDamage(int damage)
    {
        AudioManager.Instance.PlaySFX("PlayerLaserHit");

        if (canTakeDamage)
        {
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // drop an Item
                if (expOrbSpawn)
                    ObjectPoolManager.SpawnObject(expOrb, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

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

                //diesound
                AudioManager.Instance.PlaySFX("PlayerLaserDie");

                // pool (destroy) enemy object
                if (canPoolObject == true)
                    ObjectPoolManager.ReturnObjectToPool(gameObject);
                else
                    Destroy(gameObject);

            }
        }
    }

    /* **************************************************************************** */
    /* SHOOTING CONTROL------------------------------------------------------------ */
    /* **************************************************************************** */
    // start shooting
    public void StartShooting()
    {
        foreach (EnemyParticleBullet particle in enemyWeapons)
        {
            particle.BulletStart_();
            particle.GetComponent<EnemyParticleBullet>().bulletDamage = bulletDamage;
        }
        isShooting = true;
    }

    // stop shooting
    public void StopShooting()
    {
        foreach (EnemyParticleBullet particle in enemyWeapons)
        {
            particle.HardBulletStop();
        }
        isShooting = false;
    }

    public void ShowDamageFromObjects(int damage)
    {
        Vector3 pos = transform.position; // the point of intersection between the particle and the enemy
        gameManager.DoFloatingText(pos, "+" + damage.ToString(), hitColor);
    }
}

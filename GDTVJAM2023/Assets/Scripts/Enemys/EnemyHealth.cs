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

    // gameObjects to find
    private GameManager gameManager;
    private Collider collider;




    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        isShooting = false;
    }

    private void Update()
    {
        if (gameManager.dimensionShift == !secondDimensionEnemy)
        {
            collider.enabled = false;
            if (miniMapIcon != null) miniMapIcon.SetActive(false);
            StopShooting();
        }
        else
        {
            if (isShooting == false)
            {
                collider.enabled = true;
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
            int damage = other.GetComponent<ParticleBullet>().bulletDamage;

            AudioManager.Instance.PlaySFX("ImpactShot");
            TakeDamage(damage);


            int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

            foreach (ParticleCollisionEvent collisionEvent in collisionEvents) //  for each collision, do the following:
            {
                Vector3 pos = collisionEvent.intersection; // the point of intersection between the particle and the enemy
                gameManager.DoFloatingText(pos, "+" + damage.ToString(), hitColor);
            }
        }
    }




    /* **************************************************************************** */
    /* TAKE DAMAGE CONTROLL-------------------------------------------------------- */
    /* **************************************************************************** */
    // damage calculation
    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                if (expOrbSpawn)
                    ObjectPoolManager.SpawnObject(expOrb, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

                ObjectPoolManager.SpawnObject(explosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
                
                if (secondDimensionEnemy == false)
                {
                    gameManager.UpdateEnemyCounter(-1);
                    gameManager.UpdateEnemyToKill(1);
                }

                if (canPoolObject == true)
                    ObjectPoolManager.ReturnObjectToPool(gameObject);
                else
                    Destroy(gameObject);

            }
        }
    }




    /* **************************************************************************** */
    /* Shooting contoll------------------------------------------------------------ */
    /* **************************************************************************** */
    //start shooting
    public void StartShooting()
    {
        foreach (EnemyParticleBullet particle in enemyWeapons)
        {
            particle.BulletStart_();
            particle.GetComponent<EnemyParticleBullet>().bulletDamage = bulletDamage;
        }
        isShooting = true;
    }

    //stop shooting
    public void StopShooting()
    {
        foreach (EnemyParticleBullet particle in enemyWeapons)
        {
            particle.HardBulletStop();
        }
        isShooting = false;
    }
}

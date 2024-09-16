using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NewEnemyMovement;

public class EnemyHealth : MonoBehaviour
{
    [Header("Objects")]
    public GameObject explosionObject;
    public GameObject _AOEreplacement;
    private float collisionMultiplier = 128;
    public GameObject _burnReplacement;
    public GameObject dropObject;
    public GameObject miniMapIcon;
    public GameObject novaExplosionPrefab;

    public ParticleSystem burnParticleSystem;
    public Collider enemyCollider;

    [Header("Enemy Settings")]
    public float enemyHealth = 2.0f;
    [HideInInspector] public float enemyStartHealth;
    public int collisonDamage = 1;
    public float explosionForce = 5.0f;
    public bool secondDimensionEnemy = false;
    [HideInInspector] public bool canTakeDamage = true;
    public bool canPoolObject = true;

    public event EventHandler DieEvent;

    public bool isBoss = false;
    public bool isGround = false;
    [HideInInspector] public int novaOnDieTriggerType = -1; 
    // comes from other Objects - if the Object dies 
    //   -1 => dont trigger anything


    [Header("Collision Control")]
    public List<ParticleCollisionEvent> collisionEvents;
    private bool isdied = false;


    private bool isBurning = false;
    [HideInInspector] public int burnTickCount = 0;
    [HideInInspector] public bool[] canTakeLaserDamage;

    // Color Management
    [HideInInspector] public Color hitColor = new Color();
    [HideInInspector] public Color critColor = new Color();
    [HideInInspector] public Color burningColor = new Color();
    private Color resultColor;


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
        hitColor = gameManager.cCPrefab.classColor[11];
        critColor = gameManager.cCPrefab.classColor[12];
        burningColor = gameManager.cCPrefab.classColor[13];
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();

        collisionMultiplier += UnityEngine.Random.Range(-16, 128);
        enemyStartHealth = enemyHealth;
        canTakeLaserDamage = new bool[7];
    }

    private void OnEnable()
    {
        if (enemyCollider != null) enemyCollider.enabled = true;
        collisionEvents = new List<ParticleCollisionEvent>();
        enemyHealth = enemyStartHealth;
        isBurning = false;
        if (canPoolObject == true)
            canTakeDamage = true;

        burnTickCount = 0;
        CancelInvoke();

        isdied = false;
        canTakeLaserDamage[0] = true; //burning Damage
        canTakeLaserDamage[1] = true; // MW Laser 1
        canTakeLaserDamage[2] = true; // MW Laser 2
        canTakeLaserDamage[3] = true; // front Laser
        canTakeLaserDamage[4] = true; // Orbital Laser
        canTakeLaserDamage[5] = true; // Backfire Beam
        canTakeLaserDamage[6] = true; // Backfire Beam Split

        gameManager.OnDimensionSwap += HandleDimensionSwap;

        novaOnDieTriggerType = -1;
    }

    private void Start()
    {
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
    }

    public void HandleDimensionSwap(bool isSecondDimension = false)
    {
        if (isdied == false)
        {
            if (isSecondDimension == true)
            {
                if (enemyCollider != null) enemyCollider.enabled = false;
                if (miniMapIcon != null) miniMapIcon.SetActive(false);

            }
            else
            {
                if (enemyCollider != null) enemyCollider.enabled = true;
                if (miniMapIcon != null) miniMapIcon.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        gameManager.OnDimensionSwap -= HandleDimensionSwap;
    }



    /* **************************************************************************** */
    /* TAKE DAMAGE CONTROLL ------------------------------------------------------- */
    /* **************************************************************************** */
    #region take damage controll

    // take damage from a bullet -----------------------------------------------------
    private void OnParticleCollision(GameObject other)
    {
        if (canTakeDamage == true)
        {
            ParticleSystem part = other.GetComponent<ParticleSystem>();
            var ps = other.GetComponent<ParticleBullet>();
            int damage = ps.bulletDamage;
            int damagetyp = ps.damageTyp;
            resultColor = hitColor;

            // damage from a bullet
            if (damagetyp == 0)
            {
                AudioManager.Instance.PlaySFX("ImpactShot");

                // calculate crit damage
                int ran = UnityEngine.Random.Range(0, 100);
                if (ran < playerWeaponController.shipData.bulletCritChance)
                {
                    damage = CritDamage(damage);
                    resultColor = critColor;


                    if (upgradeChooseList.upgrades[52].upgradeIndexInstalled == 1)
                        NovaOnDie(1);
                }

                // calculate Enemy Health
                TakeDamage(damage);
            }

            // trigger damage text
            int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);
            foreach (ParticleCollisionEvent collisionEvent in collisionEvents)
            {
                Vector3 pos = collisionEvent.intersection; // the point of intersection between the particle and the enemy
                gameManager.DoFloatingText(pos, damage.ToString(), resultColor);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // calculate Enemy Health
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            // instanstiate explosion
            if (explosionObject != null)
                ObjectPoolManager.SpawnObject(explosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

            Die();
        }
    }

    public int CritDamage(int damage)
    {
        damage = Mathf.CeilToInt(damage * ((float)playerWeaponController.shipData.bulletCritDamage / 100));
        //resultColor = critColor;
        return damage;
    }


    // take damage from a Explosion -----------------------------------------------------
    public void TakeExplosionDamage(int damage)
    {
        if (canTakeDamage == true)
        {
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // create object to die effect
                if (_AOEreplacement != null)
                {
                    var replacement = Instantiate(_AOEreplacement, transform.position, transform.rotation);

                    // make the die effect object explode
                    var rbs = replacement.GetComponentsInChildren<Rigidbody>();
                    foreach (var rb in rbs)
                    {
                        rb.AddExplosionForce(collisionMultiplier, transform.position, 1);
                    }
                }
                Die();
            }
            else
            {
                // reset novaOnDieTriggerType
                novaOnDieTriggerType = -1;
            }
        }
    }



    // take damage from a Laser -----------------------------------------------------
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
            if (ran < playerWeaponController.shipData.burnDamageChance && isBurning == false)
            {
                isBurning = true;
                InvokeBurningDamage();
            }

            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // die sound
                AudioManager.Instance.PlaySFX("PlayerLaserDie");

                // create object to die effect
                if (_burnReplacement != null)
                {
                    Instantiate(_burnReplacement, transform.position, transform.rotation);
                }
                Die();
            }
        }
    }



    // take damage from burning -----------------------------------------------------
    public void InvokeBurningDamage()
    {
        if (burnParticleSystem != null)
            burnParticleSystem.Play();

        burnTickCount = 0;
        CancelInvoke(nameof(TakeBurningDamage));
        InvokeRepeating(nameof(TakeBurningDamage), .2f, 1f);
    }

    public void TakeBurningDamage()
    {
        int burningDamage = Mathf.CeilToInt(playerWeaponController.shipData.baseLaserTickDamage * (playerWeaponController.shipData.laserBurningTickDamangePercent) / 100);

        ShowDamageFromObjectsColor(burningDamage, burningColor);
        TakeLaserDamage(burningDamage, 0);

        burnTickCount++;

        if (burnTickCount > playerWeaponController.shipData.baseLaserTicks)
        {
            CancelInvoke(nameof(TakeBurningDamage));
            burnTickCount = 0;
            isBurning = false;
            if (burnParticleSystem != null)
                burnParticleSystem.Stop();
        }
    }
    #endregion



    /* **************************************************************************** */
    /* DIE STATE ----------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region DIE STATE
    private void Die()
    {
        Debug.Log("dieEvent");
        if (enemyCollider != null) enemyCollider.enabled = false;

        // cancle all Invokes
        CancelInvoke();

        // update UI
        UpdatePlayerUI();

        // calculate chance of explosion
        NovaOnDie(novaOnDieTriggerType);

        canTakeDamage = false;
        isdied = true;

        if (burnParticleSystem != null) burnParticleSystem.Stop();

        if (DieEvent != null)
        {
            DieEvent.Invoke(this, new EventArgs());
            return;
        }

        // drop an Item
        if (dropObject != null)
            ObjectPoolManager.SpawnObject(dropObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

        // pool (destroy) enemy object
        DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        Debug.Log("destroyEvent");
        // pool (destroy) enemy object
        if (canPoolObject == true)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("destroy");
        }
    }
    #endregion



    /* **************************************************************************** */
    /* MISC ----------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Misc
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
        gameManager.DoFloatingText(pos, damage.ToString(), new Color(1f, 0.6f, 0.0f, 1f));
    }

    private void UpdatePlayerUI()
    {
        // update player UI
        if (secondDimensionEnemy == false)
        {
            if (isGround == false) gameManager.UpdateEnemyCounter(-1);
            gameManager.UpdateEnemyToKill(1);
        }
    }


    private void NovaOnDie(int novaTyp) //0=die 1=crit
    {
        if (novaExplosionPrefab != null && novaTyp != -1)
        {
            float explosionRadius = 0;
            int novaDamage = 0;
            int explosionForce = 0;
            int novaOnDieTriggerType = -1;

            switch (novaTyp)
            {
                case 0: // Nova triggert from die (not in Use)
                    if (UnityEngine.Random.Range(0, 100) > 0) return;

                    explosionRadius = 0.5f * (1 + playerWeaponController.shipData.rocketAOERadius / 100);
                    novaDamage = 10;
                    explosionForce = 200;

                    break;

                case 1: // Nova triggert from crit
                    if (UnityEngine.Random.Range(0, 100) > 5) return;

                    AudioManager.Instance.PlaySFX("Playernova");
                    explosionRadius = 0.3f * (1 + playerWeaponController.shipData.rocketAOERadius / 100);
                    novaDamage = 3;
                    explosionForce = 150;

                    break;

                case 2: // Rockets from Cockpit ability
                    if (upgradeChooseList.upgrades[87].upgradeIndexInstalled > 0)
                    {
                        if (UnityEngine.Random.Range(0, 100) > 25) return;
                        explosionRadius = 1.3f * (1 + playerWeaponController.shipData.rocketAOERadius / 100);
                        novaDamage = 15;
                        explosionForce = 150;
                        novaOnDieTriggerType = 2; //can trigger a chainReaction
                    }
                    else
                    {
                        return;
                    }
                    break;
            }

            GameObject go = ObjectPoolManager.SpawnObject(novaExplosionPrefab, transform.position, transform.rotation, ObjectPoolManager.PoolType.Gameobject);
            DamageExplosionController exC = go.GetComponent<DamageExplosionController>();
            exC.damage = novaDamage;
            exC.force = explosionForce;
            exC.radius = explosionRadius;
            exC.novaOnDieTriggerType = novaOnDieTriggerType;
        }
    }
    #endregion
}

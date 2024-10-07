using UnityEngine;
using DG.Tweening;


public class BackfireBeamProjectile : MonoBehaviour
{
    public float initialForce = 20f;
    public float returnThreshold = 1f; // Threshold speed at which the projectile starts returning
    public Rigidbody rb;
    public int damage = 8;
    public int laserDamageChannel = 4;
    public int killProjectileCount = 0;

    public ParticleSystem hitParticle;
    public ParticleSystem dieParticle;
    public ParticleSystem bladeParticle;
    private Transform playerTransform; // Reference to the player's transform

    private bool returning = false;
    private bool isStarted = false;
    private bool destroyFlag = false;
    private float returnSpeed = 1.5f;
    private float returnSpeed_;

    public GameObject splitPrefab;
    public Vector3 projectileSize = new Vector3(1f, 1f, 1f);

    public BackfireBeam backfireBeam = null;
    public Collider backfireCollider;

    void Awake()
    {
        playerTransform = GameObject.Find("NewPlayer").GetComponent<Transform>();
        backfireBeam = playerTransform.gameObject.GetComponentInChildren<BackfireBeam>();
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(projectileSize, 0.1f);
        rb.AddForce(transform.forward * -initialForce, ForceMode.Impulse);
        returning = false;
        isStarted = false;
        destroyFlag = false;
        returnSpeed_ = returnSpeed;
        damage = backfireBeam.damage;
        killProjectileCount = backfireBeam.killProjectileCount;
        backfireCollider.enabled = true;

        Invoke(nameof(CanDoDamage), 0.1f);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void CanDoDamage()
    {
        isStarted = true;
    }

    void FixedUpdate()
    {
        if (!returning)
        {
            // Check if the projectile should start returning
            if (rb.velocity.magnitude < returnThreshold && isStarted == true)
            {
                returning = true;
            }
        }
        else
        {
            // Calculate direction to the player
            returnSpeed_ = returnSpeed_ * 1.01f * 1.01f;
        }

    }
    private void Update()
    {
        if (returning)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, returnSpeed_ * Time.deltaTime);

            if (Vector3.Distance(playerTransform.position, transform.position) <= 1f)
            {
                if (backfireBeam != null && destroyFlag == false)
                {
                    backfireBeam.NovaTrigger();
                }
                DestroyProjectile();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy") || other.CompareTag("secondDimensionEnemy"))
        {
            EnemyHealth eH = other.gameObject.GetComponent<EnemyHealth>();

            if (eH != null)
            {
                if (eH.canTakeLaserDamage[laserDamageChannel] == true && eH.canTakeDamage == true)
                {
                    if (eH.enemyHealth - damage <= 0 && killProjectileCount > 0)
                    {
                        SplitProjectile();
                    }
                    eH.TakeLaserDamage(damage, laserDamageChannel);
                    eH.ShowDamageFromPosition(other.transform.position, damage);
                    hitParticle.transform.position = other.ClosestPointOnBounds(other.transform.position);
                    hitParticle.Emit(20);
                }
            }
            else
            {
                EnemyShield es = other.transform.GetComponentInParent<EnemyShield>();

                if (es != null)
                {
                    es.ShieldGetDamage();
                    es.ShowDamageFromPosition(other.ClosestPointOnBounds(other.transform.position));
                }
            }

        }
    }

    private void SplitProjectile()
    {
        DestroyProjectile();

        for (int i = 0; i < killProjectileCount; i++)
        {
            if (splitPrefab != null)
            {
                float angle = i * (360f / killProjectileCount);
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                GameObject go = ObjectPoolManager.SpawnObject(splitPrefab, transform.position, rotation, ObjectPoolManager.PoolType.Gameobject);
                BackfireBeamProjectile bfB = go.GetComponent<BackfireBeamProjectile>();
                bfB.damage = Mathf.RoundToInt((float)damage / 2);
            }
        }
    }

    private void DestroyProjectile()
    {
        if (destroyFlag == false)
        {
            backfireCollider.enabled = false;
            dieParticle.transform.position = playerTransform.position;
            dieParticle.Emit(10);
            bladeParticle.Stop();

            destroyFlag = true;
            Invoke(nameof(DestroyObject), 1f);
        }
    }

    private void DestroyObject()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}

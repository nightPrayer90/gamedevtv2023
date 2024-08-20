using UnityEngine;

public class GranateController : MonoBehaviour
{
    [Header("Explosion Settings")]
    [HideInInspector] public int damage; // explosion damage
    public float speed = 10f; // explosion speed
    public float iniLifeTime; // initial lifetime
    private float maxLifeTime; // time before the explosion gets destroyed
    [HideInInspector] public Color hitColor;
    public bool isMainWeapon = false;

    [Header("Explosion Control")]
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    private LayerMask layerMask;

    [Header("Game Objects")]
    public GameObject exposionHitObject;
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    private PlayerWeaponController playerWeaponController;

    private Vector3 startPosition;
    private float distanceTraveled;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */

    private void Awake()
    {
        // set Game Objects
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
    }

    private void OnEnable()
    {
        SetDestroyTimer();

        // Layermask
        layerMask = (1 << 6);

        // Initialize the start position
        startPosition = transform.position;
        distanceTraveled = 0f; // Reset the distance traveled

        distanceTraveled = Vector3.Distance(startPosition, transform.position);
    }

    private void FixedUpdate()
    {
        // Explosion movement straight forward
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            layerMask = (1 << 9);
            tagStr = "secondDimensionEnemy";
        }

        // Enemy target tag compare only then destroy the explosion
        if (other.CompareTag(tagStr))
        {
            Explode(other.gameObject);
        }
    }


    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    private void DestroyObject()
    {
        Explode();
    }

    private void Explode(GameObject collisionTarget = null)
    {
        // Position of explosion object
        Vector3 pos = transform.position;

        // Cancel invoke
        CancelInvoke(nameof(DestroyObject));

        // Array of all objects in explosionRadius
        float explosionRadius_ = explosionRadius * (1 + playerWeaponController.shipData.rocketAOERadius / 100);
        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius_, layerMask);

        foreach (var obj in surroundingObjects)
        {
            // Control if object has shield
            EnemyShield eSh = obj.GetComponent<EnemyShield>();
            if (eSh != null)
            {
                int shieldDamage = 1;
                eSh.ShieldGetDamage(shieldDamage);
                gameManager.DoFloatingText(transform.position, shieldDamage.ToString(), eSh.hitColor);
                continue;
            }

            // Get rigidbodys from all objects in range
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            // Calculate distance between explosion center and objects in range
            float distance = Vector3.Distance(pos, rb.transform.position);

            // Get EnemyHealth script
            EnemyHealth eHC = obj.GetComponent<EnemyHealth>();
            Color resultColor = hitColor;
            int adjustedDamage = damage;

            if (eHC.isBoss == true && isMainWeapon == true)
            {
                adjustedDamage = Mathf.CeilToInt((float)damage * (1 + (float)playerWeaponController.shipData.bossBonusDamage / 100));
            }

            if (eHC != null)
            {
                if (obj.gameObject != collisionTarget)
                {
                    if (distance < explosionRadius_)
                    {
                        float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius_), 1f);
                        adjustedDamage = Mathf.CeilToInt(adjustedDamage * scaleFactor);
                    }
                }

                if (upgradeChooseList.upgrades[54].upgradeIndexInstalled > 0)
                {
                    int ran = Random.Range(0, 100);
                    if (ran < playerWeaponController.shipData.bulletCritChance)
                    {
                        adjustedDamage = eHC.CritDamage(adjustedDamage);
                        resultColor = eHC.critColor;
                    }
                }

                // Show floating text
                if (eHC.canTakeDamage == true)
                    gameManager.DoFloatingText(obj.transform.position, adjustedDamage.ToString(), resultColor);

                // Calculate enemy damage
                eHC.TakeExplosionDamage(adjustedDamage);
            }
            rb.AddExplosionForce(explosionForce, pos, explosionRadius_);
        }

        // Spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(exposionHitObject, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius_;

        // Object goes back to the pool
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }


    public void SetDestroyTimer()
    {
        // Destroy time
        maxLifeTime = Random.Range(iniLifeTime - 0.05f, iniLifeTime + 0.05f) + playerWeaponController.shipData.rocketLifeTime;
        Invoke(nameof(DestroyObject), maxLifeTime); // Can not be in enable - because lifetime comes from another object
    }
}

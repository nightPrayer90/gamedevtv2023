using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("Rocket Settings")]
    [HideInInspector] public int damage; // rocked damage
    public float speed = 10f; // rocked Speed
    public float rotationSpeed = 5f; // rotation speed
    public float startTime = 0.5f; // time until the rocked flys to a target
    protected float rotationSpeedtmp = 0;
    public GameObject exposionHitObject;
    public float iniLifeTime;
    private float maxLifeTime;   // time before the rocked get destroyed
    [HideInInspector] public Color hitColor;
    public bool isMainWeapon = false;

    [Header("Explosion Control")]
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    private LayerMask layerMask;


    [Header("Game Objects")]
    public GameObject trail;
    private GameObject target;
    // private Rigidbody rbRocket;
    protected GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    protected PlayerWeaponController playerWeaponController;


    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void OnEnable()
    {
        InitRocket();

        // set Game Objects
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();

        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        // reset Target
        target = null;

        // is main weapon flag=
        isMainWeapon = false; // flag set from mwController

        // Layermask
        layerMask = (1 << 6);
    }

    protected void InitRocket()
    {
        rotationSpeedtmp = rotationSpeed;
        rotationSpeed = 0;
        Invoke(nameof(SetRotateSpeed), startTime);

        if (trail != null)
            Invoke(nameof(ActivateTrail), 0.2f);
    }

    private void FixedUpdate()
    {
        // rocket movement
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += transform.forward * speed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (!target.activeSelf)
            {
                target = null;
            }
        }

        // find a new target
        else
        {
            // reset target
            target = null;

            // find the next target
            FindNextTarget(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            layerMask = (1 << 9);
            tagStr = "secondDimensionEnemy";
        }
        
        // enemy target tag compare only than destroy the rocked
        if (other.CompareTag(tagStr))
        {
            Explode(other.gameObject);
        }
    }

    protected void OnDisable()
    {
        if (trail != null)
            trail.SetActive(false);
    }

    /* **************************************************************************** */
    /* MOVMENT--------------------------------------------------------------------- */
    /* **************************************************************************** */
    // find and set the next target
    private void FindNextTarget()
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tagStr);

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        // find the closest enemy
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        // set target to the closestEnemy
        target = closestEnemy;
    }

    // set the rotatespeed and a new target after start
    private void SetRotateSpeed()
    {
        rotationSpeed = rotationSpeedtmp;

        // Target reset
        target = null;
        FindNextTarget();
    }




    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // destroy function 
    private void DestroyObject()
    {
        // enemy target tag set
        if (gameManager.dimensionShift == true)
        {
            layerMask = (1 << 9);
        }

        Explode();
    }

    private void Explode(GameObject collisionTarget = null)
    {
        // postion of explosion Object
        Vector3 pos = transform.position;

        // cancle invoke
        CancelInvoke("DestroyObject");

        // array of all Objects in explosionRadius
        float explosionRadius_ = explosionRadius * playerWeaponController.rocketAOERadius;
        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius_, layerMask);

        foreach (var obj in surroundingObjects)
        {
            // control is shield;
            EnemyShield eSh = obj.GetComponent<EnemyShield>();
            if (eSh != null)
            {
                int shieldDamage = 1;
                eSh.ShieldGetDamage(shieldDamage);
                gameManager.DoFloatingText(transform.position, shieldDamage.ToString(), eSh.hitColor);
                continue;
            }

            // get rigidbodys from all objects in range
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

           
            // calculate distance between explosioncenter and objects in Range
            float distance = Vector3.Distance(pos, rb.transform.position);


            // get EnemyHealthscript
            EnemyHealth eHC = obj.GetComponent<EnemyHealth>();
            Color resultColor = hitColor;
            int adjustedDamage = damage;

            if (eHC.isBoss == true && isMainWeapon == true)
            {
                adjustedDamage = Mathf.CeilToInt((float)damage * (1+(float)upgradeChooseList.bossBonusDamage / 100));
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
                    int ran = UnityEngine.Random.Range(0, 100);
                    if (ran < playerWeaponController.bulletCritChance)
                    {
                        adjustedDamage = eHC.CritDamage(adjustedDamage);
                        resultColor = eHC.critColor;
                    }
                }

                // show floating text
                if (eHC.canTakeDamage == true)
                    gameManager.DoFloatingText(obj.transform.position, adjustedDamage.ToString(), resultColor);

                // calculate enemy damage
                eHC.TakeExplosionDamage(adjustedDamage);
            }
            rb.AddExplosionForce(explosionForce, pos, explosionRadius);
        }


        // spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(exposionHitObject, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius_;

        // object goes back to the pool
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void ActivateTrail()
    {
        if (trail != null)
            trail.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void SetDestroyTimer()
    {
        // destroytime
        maxLifeTime = Random.Range(iniLifeTime - 0.05f, iniLifeTime + 0.05f) + upgradeChooseList.rocketLifeTime;
        Invoke(nameof(DestroyObject), maxLifeTime); //can not be in enable - because lifetime comes from another object
    }
}


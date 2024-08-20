using UnityEngine;

[RequireComponent(typeof(Explosion))]
public class RocketController : MonoBehaviour
{
    [Header("Rocket Settings")]
    [HideInInspector] public int damage; // rocked damage
    public float speed = 10f; // rocked Speed
    public float rotationSpeed = 5f; // rotation speed
    public float startTime = 0.5f; // time until the rocked flys to a target
    protected float rotationSpeedtmp = 0;
    public float iniLifeTime;
    private float maxLifeTime;   // time before the rocked get destroyed
     
    [Header("Explosion Control")]
    public float explosionRadius = 5f;
    public int explosionForce = 500;
    public int novaOnDieTriggerType = -1;
    private Explosion explosion;
    [HideInInspector]public bool isMainWeapon = false;

    [Header("Game Objects")]
    public GameObject trail;
    private GameObject target;
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    private PlayerWeaponController playerWeaponController;
    

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
        explosion = gameObject.GetComponent<Explosion>();

        target = null; // reset Target
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
        LayerMask layerMask = (1 << 6);
        if (gameManager.dimensionShift == true)
        {
            layerMask = (1 << 9);
            tagStr = "secondDimensionEnemy";
        }
        
        // enemy target tag compare only than destroy the rocked
        if (other.CompareTag(tagStr))
        {
            CancelInvoke(nameof(DestroyObject));
            explosion.InitExplosion(damage,explosionForce,explosionRadius,isMainWeapon,other.gameObject, novaOnDieTriggerType);
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
        CancelInvoke(nameof(DestroyObject));
        explosion.InitExplosion(damage, explosionForce, explosionRadius, isMainWeapon, null, novaOnDieTriggerType);
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
        maxLifeTime = Random.Range(iniLifeTime - 0.05f, iniLifeTime + 0.05f) + playerWeaponController.shipData.rocketLifeTime;
        Invoke(nameof(DestroyObject), maxLifeTime); //can not be in enable - because lifetime comes from another object
    }
}


using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("Rocked Settings")]
    [HideInInspector] public int damage; // rocked damage
    public float speed = 10f; // rocked Speed
    public float rotationSpeed = 5f; // rotation speed
    public GameObject exposionHitObject;
    public GameObject exposionDieObject;
    [HideInInspector] public float maxLifeTime;   // time before the rocked get destroyed
    [HideInInspector] public Color hitColor;


    [Header("Explosion Controll")]
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    private Rigidbody rb;
    private LayerMask layerMask;


    [Header("Game Objects")]
    private GameObject target;
    private Rigidbody rbRocket;
    private GameManager gameManager;

    

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void OnEnable()
    {
        // set Game Objects
        rbRocket = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        
        // reset Target
        target = null;

        // destroytime
        maxLifeTime = Random.Range(maxLifeTime - 0.25f, maxLifeTime + 0.25f);
        Invoke("DestroyObject", maxLifeTime);


        // Layermask
        layerMask = (1 << 6);
    }

    private void FixedUpdate()
    {
        // rocked movement
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rbRocket.velocity = speed * direction;
            RotateTowardsTarget(direction);

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

        // postion of explosion Object
        Vector3 pos = transform.position;
        // enemy target tag compare only than destroy the rocked
        if (other.gameObject.CompareTag(tagStr))
        {
            // cancle invoke
            CancelInvoke("DestroyObject");


            // array of all Objects in the explosionRadius
            var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

            foreach (var obj in surroundingObjects)
            {
                // get rigidbodys from all objects in range
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null) continue;

                // calculate distance between explosioncenter and objects in Range
                float distance = Vector3.Distance(pos, rb.transform.position);
                //Debug.Log(distance);

                if (distance < explosionRadius)
                {
                    float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
                    int adjustedDamage = Mathf.CeilToInt(damage * scaleFactor);

                    // get EnemyHealthscript
                    EnemyHealth eHC = obj.GetComponent<EnemyHealth>();
                    Debug.Log("SF + " + scaleFactor + " | distance " + distance);

                    // calculate enemy damage
                    eHC.TakeExplosionDamage(adjustedDamage);

                    // show floating text
                    gameManager.DoFloatingText(rb.transform.position, "+" + adjustedDamage.ToString(), hitColor);
                }

                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

           
            // spawn the explosion object
            ObjectPoolManager.SpawnObject(exposionHitObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

            // object goes back to the pool
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }


    /* **************************************************************************** */
    /* MOVMENT--------------------------------------------------------------------- */
    /* **************************************************************************** */
    // rotate the rocked in flydirection
    private void RotateTowardsTarget(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

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




    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // destroy function 
    private void DestroyObject()
    {
        // cancle destroy invoke
        CancelInvoke("DestroyObject");

        // object goes back to the pool
        ObjectPoolManager.ReturnObjectToPool(gameObject);

        // spawn the explosion object
        ObjectPoolManager.SpawnObject(exposionDieObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
    }
}

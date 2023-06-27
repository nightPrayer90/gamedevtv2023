using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("Rocked Settings")]
    [HideInInspector] public int damage; // rocked damage
    public float speed = 10f; // rocked Speed
    public float rotationSpeed = 5f; // rotation speed
    public float startTime = 0.5f; // time until the rocked flys to a target
    private float rotationSpeedtmp = 0;
    public GameObject exposionHitObject;
    [HideInInspector] public float maxLifeTime;   // time before the rocked get destroyed
    [HideInInspector] public Color hitColor;


    [Header("Explosion Control")]
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    private LayerMask layerMask;


    [Header("Game Objects")]
    public GameObject trail;
    private GameObject target;
   // private Rigidbody rbRocket;
    private GameManager gameManager;

    

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void OnEnable()
    {
        rotationSpeedtmp = rotationSpeed;
        rotationSpeed = 0;
        Invoke("SetRotateSpeed", startTime);

        if (trail != null)
            Invoke("ActivateTrail", 0.2f);

        // set Game Objects
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

        GameObject go = other.gameObject;
        
        // enemy target tag compare only than destroy the rocked
        if (other.gameObject.CompareTag(tagStr))
        {
            Explode(go);
        }
    }


    private void OnDisable()
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

            if (obj.gameObject != collisionTarget)
            {
                if (distance < explosionRadius)
                {

                    float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
                    int adjustedDamage = Mathf.CeilToInt(damage * scaleFactor);

                    // get EnemyHealthscript
                    EnemyHealth eHC = obj.GetComponent<EnemyHealth>();

                    // calculate enemy damage
                    eHC.TakeExplosionDamage(adjustedDamage);

                    // show floating text
                    gameManager.DoFloatingText(rb.transform.position, "+" + adjustedDamage.ToString(), hitColor);
                }
            }
            else
            {
                // get EnemyHealthscript
                EnemyHealth eHC = obj.GetComponent<EnemyHealth>();

                // calculate enemy damage
                eHC.TakeExplosionDamage(damage);

                // show floating text
                gameManager.DoFloatingText(rb.transform.position, "+" + damage.ToString(), hitColor);
            }

            rb.AddExplosionForce(explosionForce, pos, explosionRadius);
        }


        // spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(exposionHitObject, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;

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
}

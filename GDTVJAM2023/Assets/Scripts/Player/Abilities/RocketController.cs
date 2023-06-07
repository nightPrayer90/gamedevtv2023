using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("Rocked Settings")]
    [HideInInspector] public int damage; // rocked damage
    public float speed = 10f; // rocked Speed
    public float rotationSpeed = 5f; // rotation speed
    public GameObject exposionObject;
    [HideInInspector] public float maxLifeTime;   // time before the rocked get destroyed
    [HideInInspector] public Color hitColor;

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
            tagStr = "secondDimensionEnemy";
        }

        // enemy target tag compare
        if (other.gameObject.CompareTag(tagStr))
        {
            // cancle invoke
            CancelInvoke("DestroyObject");

            // calculate damage
            other.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);

            // show floating text
            gameManager.DoFloatingText(transform.position, "+" + damage.ToString(), hitColor);

            // object goes back to the pool
            ObjectPoolManager.ReturnObjectToPool(gameObject);

            // spawn the explosion object
            ObjectPoolManager.SpawnObject(exposionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        }
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
        ObjectPoolManager.SpawnObject(exposionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
    }
}

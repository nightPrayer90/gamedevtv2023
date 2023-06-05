using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    public float speed = 10f; // Geschwindigkeit der Rakete
    public float rotationSpeed = 5f; // Rotationsgeschwindigkeit der Rakete
    public string enemyTag = "Enemy"; // Tag des Enemy-Objekts
    
    public GameObject exposionObject;

    private GameObject target; // aktuelles Zielobjekt
    private Rigidbody rbRocket;
    private GameManager gameManager;

    public int damage = 5;

    public float maxLifeTime;


    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        rbRocket = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        target = null;
        FindNextTarget(); // Erstes Zielobjekt suchen
        Invoke("DestroyObject", maxLifeTime);
        maxLifeTime = Random.Range(maxLifeTime - 0.25f, maxLifeTime + 0.25f); 
    }

    private void FixedUpdate()
    {
        // Rakete zum Zielobjekt bewegen
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rbRocket.velocity = direction * speed;
            RotateTowardsTarget(direction);
        }
        else
        {
            FindNextTarget(); // Neues Zielobjekt suchen
        }
    }

    private void RotateTowardsTarget(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void DestroyObject()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);

        ObjectPoolManager.SpawnObject(exposionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
    }

    private void FindNextTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                if (gameManager.dimensionShift == false)
                {
                    if (enemy.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
                else
                {
                    if (enemy.layer == LayerMask.NameToLayer( "secondDimensionEnemy"))
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }

            
            }
        }

        target = closestEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);

            ObjectPoolManager.ReturnObjectToPool(gameObject);

            ObjectPoolManager.SpawnObject(exposionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        }
    }
}

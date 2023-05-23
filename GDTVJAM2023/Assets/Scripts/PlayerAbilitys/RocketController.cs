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

    public int damage = 5;

    private void Start()
    {
        FindNextTarget(); // Erstes Zielobjekt suchen
        rbRocket = GetComponent<Rigidbody>();
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
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        target = closestEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);

            Destroy(gameObject);
            Instantiate(exposionObject, transform.position, transform.rotation);
        }
    }
}

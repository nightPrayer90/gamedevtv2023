using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject explosionObject;
    public float enemyHealth = 2.0f;

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            Instantiate(explosionObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}

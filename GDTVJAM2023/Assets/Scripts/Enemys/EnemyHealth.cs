using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject explosionObject;
    public GameObject dieExplosionObject;
    public GameObject expOrb;
    public float enemyHealth = 2.0f;
    public int collisonDamage = 1; //wird alles über den Spieler abgefragt
    public float explosionForce = 5.0f;
    public bool expOrbSpawn = false;

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            if (expOrbSpawn)
                Instantiate(expOrb, transform.position, transform.rotation);
            Instantiate(explosionObject, transform.position, transform.rotation);
            
            Destroy(gameObject);
        }
    }
}

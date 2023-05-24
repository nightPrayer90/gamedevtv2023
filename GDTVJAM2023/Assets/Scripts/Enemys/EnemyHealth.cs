using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject explosionObject;
    public GameObject dieExplosionObject;
    public GameObject expOrb;
    private GameManager gameManager;
    private Collider collider;
    public float enemyHealth = 2.0f;
    public int collisonDamage = 1; //wird alles über den Spieler abgefragt
    public float explosionForce = 5.0f;
    public bool expOrbSpawn = false;

    public List<EnemyParticleBullet> enemyWeapons;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (gameManager.dimensionShift == true)
        {
            collider.enabled = false;
            StopShooting();
        }
        else
        {
            collider.enabled = true;
            StartShooting();
        }
    }

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            if (expOrbSpawn)
                Instantiate(expOrb, transform.position, transform.rotation);
            Instantiate(explosionObject, transform.position, transform.rotation);

            gameManager.UpdateEnemyCounter(-1);
            gameManager.UpdateEnemyToKill(1);

            Destroy(gameObject);
        }
    }

    public void StopShooting()
    {
        int b = 0;
        foreach (EnemyParticleBullet particle in enemyWeapons)
        {
            enemyWeapons[b].HardBulletStop();
            b++;
        }
    }

    public void StartShooting()
    {
        int b = 0;
        foreach (EnemyParticleBullet particle in enemyWeapons)
        {
            enemyWeapons[b].BulletStart_();
            b++;
        }
    }

   
}

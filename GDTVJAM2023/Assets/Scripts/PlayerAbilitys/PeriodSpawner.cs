using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodSpawner : MonoBehaviour
{
    public GameObject rockedToLaunch;
    public float spawnInterval = 4f;
    private float nextSpawnTime = 0f;
    public float detectionRange = 10f;
    public GameObject spawnPoint;
    public GameObject spawnPoint2;
    private bool enemyDetected = false;
    // Update is called once per frame
    void Update()
    {
        if (enemyDetected == false)
        {
            DetectEnemy();

        }
        if (Time.time >= nextSpawnTime && enemyDetected == true)
        {
            Instantiate(rockedToLaunch, spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) *  gameObject.transform.rotation);
            Instantiate(rockedToLaunch, spawnPoint2.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation);
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

   
    private void DetectEnemy()
    {
        // Finde alle Objekte mit dem Tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Überprüfe für jedes gefundene Enemy-Objekt die Distanz
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // Wenn das Enemy-Objekt innerhalb der Reichweite ist
            if (distance <= detectionRange)
            {
                enemyDetected = true;
            }
        }
    }
}

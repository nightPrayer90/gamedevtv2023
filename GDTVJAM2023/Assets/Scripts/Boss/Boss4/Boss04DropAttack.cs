using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss04DropAttack : MonoBehaviour
{
    public GameObject attackPrefab;
    private int numberOfObjects = 15;
    private int spawntObjects = 0;
    public float radius = 10f; // Radius des Kreises
    public float yPosition = 9f; // Y-Position der zu spawnenden Objekte
    public float spawntime = 0.1f;


    public void SpawnFallingObjects(int numberOfObjects_)
    {
        numberOfObjects = numberOfObjects_;
        spawntObjects = 0;
        InvokeRepeating(nameof(SpawnObjectsInCircle), 0, spawntime);
    }

    private void SpawnObjectsInCircle()
    {
        // Zufälligen Winkel in Radiant generieren
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius_ = Random.Range(2, radius);

        // Position auf dem Kreis berechnen
        float x = Mathf.Cos(angle) * radius_;
        float z = Mathf.Sin(angle) * radius_;
        float y = yPosition + Random.Range(0f, 4f);

        // Spawne das Objekt an der berechneten Position
        Vector3 spawnPosition = new Vector3(transform.position.x + x, yPosition, transform.position.z + z);
        Instantiate(attackPrefab, spawnPosition, Quaternion.identity);

        spawntObjects++;

        if (spawntObjects >= numberOfObjects)
        {
            CancelInvoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCanon : MonoBehaviour
{
    public GameObject player; // Referenz auf das Spielerobjekt
    public string enemyTag = "Enemy"; // Der Tag der zu suchenden Objekte
    private GameObject nearestEnemy = null; // Referenz auf das nächstgelegene Objekt

    public ParticleSystem particleSystem;
    public int fireSalveCount;
    public int fireSalveMax;

    public float rotationSpeed = 5f; // Geschwindigkeit der Drehung
    private Quaternion targetRotation; // Zielrotation
    private bool isRotating; // Flag, ob sich das Objekt gerade dreht
    
    public float reloadInterval = 2f;
    private float nextShootTime = 0f;

    private void Update()
    {
        if (isRotating)
        {
            RotateTowardsTarget();
        }
        else
        {
            FindNearestEnemyObject();
        }


        if (fireSalveCount >= fireSalveMax)
        {
            if (Time.time >= nextShootTime)
            {
                fireSalveCount = 0;
                nextShootTime = Time.time + reloadInterval;
            }
        }

    }

    private void FindNearestEnemyObject()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // Alle Objekte mit dem gegebenen Tag finden
        float closestDistance = Mathf.Infinity; // Startwert für die kürzeste Distanz
        nearestEnemy = null; // Reset

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position); // Distanz zum Spielerobjekt berechnen

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        // Hier hast du nun das nächstgelegene Objekt als nearestEnemy
        if (nearestEnemy != null)
        {
            Debug.Log("Nächstes Feindobjekt: " + nearestEnemy.name);
            StartRotation();
        }
    }

    private void StartRotation()
    {
        // Berechnung der Zielrotation
        Vector3 targetDirection = nearestEnemy.transform.position - transform.position;
        targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // Setze das Drehungs-Flag auf true, um die Drehung zu starten
        isRotating = true;
    }

    private void RotateTowardsTarget()
    {
        // Drehe das Objekt in Richtung des Ziels
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Überprüfe, ob die Drehung abgeschlossen ist
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            isRotating = false;
            RotationCompleted(); // Funktion, die ausgelöst wird, wenn die Drehung vollendet ist
        }
    }

    private void RotationCompleted()
    {
        if (fireSalveCount < fireSalveMax)
        {
            particleSystem.Play();
            fireSalveCount ++;
        }
    }
}

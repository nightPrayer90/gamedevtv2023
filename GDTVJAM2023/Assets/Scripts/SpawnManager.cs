using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> objectsToSpawn;
    public List<float> spawnProbabilities;
    public float maxSpawnDistance = 10f;
    public float spawnInterval = 2f;

    private Camera mainCamera;
    private float nextSpawnTime = 0f;

    private GameManager gameManager;

    private void Start()
    {
        mainCamera = Camera.main;
        nextSpawnTime = Time.time + spawnInterval;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (gameManager.gameIsPlayed && !gameManager.gameOver)
        {
            if (Time.time >= nextSpawnTime)
            {
                SpawnObject();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
    }

    private void SpawnObject()
    {
        // Randomly select an object to spawn based on probabilities
        int randomIndex = GetRandomWeightedIndex(spawnProbabilities);
        GameObject objectToSpawn = objectsToSpawn[randomIndex];

        // Generate a random position outside the camera's view
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Spawn the object at the generated position
        Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

        gameManager.UpdateEnemyCounter(1);
    }

    private int GetRandomWeightedIndex(List<float> probabilities)
    {
        // Calculate the total weight of all probabilities
        float totalWeight = 0f;
        foreach (float probability in probabilities)
        {
            totalWeight += probability;
        }

        // Generate a random value between 0 and the total weight
        float randomValue = Random.Range(0f, totalWeight);

        // Iterate through the probabilities and find the corresponding index based on the random value
        float cumulativeWeight = 0f;
        for (int i = 0; i < probabilities.Count; i++)
        {
            cumulativeWeight += probabilities[i];
            if (randomValue <= cumulativeWeight)
            {
                return i;
            }
        }

        // Default to the last index if no valid index is found
        return probabilities.Count - 1;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Get the camera's viewport bounds
        Bounds viewportBounds = GetViewportBounds();

        // Calculate the random spawn position outside the camera's view on the Y-axis 6
        Vector3 spawnPosition = Vector3.zero;
        do
        {
            spawnPosition = RandomPointOutsideViewport(viewportBounds);
            spawnPosition.y = 6f;
        } while (Vector3.Distance(mainCamera.transform.position, spawnPosition) < maxSpawnDistance);

        return spawnPosition;
    }

    private Bounds GetViewportBounds()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        if (mainCamera != null)
        {
            bounds = new Bounds(mainCamera.transform.position, Vector3.zero);
            Vector3 cameraBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
            bounds.Encapsulate(cameraBounds);
        }
        return bounds;
    }

    private Vector3 RandomPointOutsideViewport(Bounds viewportBounds)
    {
        Vector3 randomPoint = Vector3.zero;
        float x, z;

        // Randomly choose a side of the viewport
        int side = Random.Range(0, 4);

        // Generate a random point outside the chosen side of the viewport
        switch (side)
        {
            case 0: // Top side
                x = Random.Range(viewportBounds.min.x, viewportBounds.max.x);
                z = viewportBounds.max.z + maxSpawnDistance;
                randomPoint = new Vector3(x, 6f, z);
                break;
            case 1: // Right side
                x = viewportBounds.max.x + maxSpawnDistance;
                z = Random.Range(viewportBounds.min.z, viewportBounds.max.z);
                randomPoint = new Vector3(x, 6f, z);
                break;
            case 2: // Bottom side
                x = Random.Range(viewportBounds.min.x, viewportBounds.max.x);
                z = viewportBounds.min.z - maxSpawnDistance;
                randomPoint = new Vector3(x, 6f, z);
                break;
            case 3: // Left side
                x = viewportBounds.min.x - maxSpawnDistance;
                z = Random.Range(viewportBounds.min.z, viewportBounds.max.z);
                randomPoint = new Vector3(x, 6f, z);
                break;
        }

        return randomPoint;
    }
}
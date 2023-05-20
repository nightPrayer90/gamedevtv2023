using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject followEnemy;
    public float spawnRadius = 10f;
    public float spawnInterval = 2f;

    private Camera mainCamera;
    private Transform playerTransform;

    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        playerTransform = GameObject.Find("Player").transform;

        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 playerPosition = playerTransform.position;

            Vector3 spawnPosition = GetRandomPosition(cameraPosition, playerPosition);
            SpawnObject(spawnPosition);

            yield return new WaitForSeconds(spawnInterval);
        }
    }



    private Vector3 GetRandomPosition(Vector3 cameraPostion, Vector3 playerPosition)
    {
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = playerPosition + randomDirection * spawnRadius;

        // Überprüfe, ob die Spawn-Position außerhalb des Kamerablickfelds liegt
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        while (GeometryUtility.TestPlanesAABB(frustumPlanes, new Bounds(spawnPosition, Vector3.one)))
        {
            randomDirection = Random.insideUnitCircle.normalized;
            spawnPosition = playerPosition + randomDirection * spawnRadius;
        }

        return spawnPosition;
    }

    private void SpawnObject(Vector3 spawnPosition)
    {
        Instantiate(followEnemy, spawnPosition, Quaternion.identity);
    }
}

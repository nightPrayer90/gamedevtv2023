using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    [Header("Rocket Settings")]
    public int rocketDamage = 5;
    public float salveInterval = 4f;
    public int rocketCount = 5;
    public float spawnInterval;
    public float timeToStart = 1f;

    public GameObject rocketToLaunch;
    public GameObject spawnPoint;
    private GameManager gameManager;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(SpawnRockets), timeToStart, salveInterval);
    }

    // Invoke, spawns a rocked after spawnInterval sec
    private void SpawnRockets()
    {
        for (int i=0; i < rocketCount; i++)
        {
            Invoke(nameof(SpawnSingleRocket), spawnInterval * i);
        }
    }

    private void SpawnSingleRocket()
    {
        // spawn rocket
        GameObject go = ObjectPoolManager.SpawnObject(rocketToLaunch, spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
        DropRocketController rocket = go.GetComponent<DropRocketController>();
        rocket.damage = rocketDamage;
        rocket.hitColor = hitColor;

        AudioManager.Instance.PlaySFX("PlayerRocketStart");
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}

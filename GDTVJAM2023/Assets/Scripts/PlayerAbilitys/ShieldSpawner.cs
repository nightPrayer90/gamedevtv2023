using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    public GameObject shieldObject;
    private PlayerController playerController;

    public float spawnInterval = 10f;
    private float nextSpawnTime = 0f;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (playerController.isFrondshield == false)
        {
            if (Time.time >= nextSpawnTime)
            {
                Instantiate(shieldObject, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f) * gameObject.transform.rotation);
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
    }

}

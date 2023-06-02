using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    public List<GameObject> shieldObjects;
    private PlayerController playerController;

    public float spawnInterval = 10f;
    public float nextSpawnTime = 0f;

    public bool isBackShield;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        nextSpawnTime = Time.time + spawnInterval/4;
    }
    // Update is called once per frame
    void Update()
    {
        if (isBackShield == false)
        {
            // FrontShield
            if (playerController.isFrontShield == false)
            {
                Debug.Log(1);
                if (Time.time >= nextSpawnTime)
                {
                    foreach (GameObject shieldObject in shieldObjects)
                    {
                        Debug.Log(2);
                        Instantiate(shieldObject, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f) * gameObject.transform.rotation);
                    }
                    Debug.Log(3);
                    nextSpawnTime = Time.time + spawnInterval;
                }
            }
        }
        else
        {
            //BackShield
            if (playerController.isBackShieldLeft == false && playerController.isBackShieldRight == false)
            {
                if (Time.time >= nextSpawnTime)
                {
                    foreach (GameObject shieldObject in shieldObjects)
                    {
                        Instantiate(shieldObject, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f) * gameObject.transform.rotation);
                    }
                    nextSpawnTime = Time.time + spawnInterval;
                }
            }
        }
    }

}

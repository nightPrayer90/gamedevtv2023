using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodEmitter : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    public float spawnInterval = 2f;
    private float nextSpawnTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Play();
            }

            

            nextSpawnTime = Time.time + spawnInterval;
        }
    }
}

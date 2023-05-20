using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroy : MonoBehaviour
{
    public ParticleSystem particleSystemToCheck;

    // Update is called once per frame
    void Update()
    {
        if (!particleSystemToCheck.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}

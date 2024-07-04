using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossModulMainengine : MonoBehaviour
{
    public ParticleSystem[] enginePSs;
    private bool controlflag = false;

    void Awake()
    {
        EngineStop();
    }

    private void Update()
    {
        if (controlflag == true)
        {
            foreach (ParticleSystem engine in enginePSs)
            {
                engine.Emit(1);
            }
        }
    }

    public void EngineActivate()
    {
        controlflag = true;
    }

    public void EngineStop()
    {
        foreach (ParticleSystem engine in enginePSs)
        {
            engine.Stop();
        }
        controlflag = false;
    }
}

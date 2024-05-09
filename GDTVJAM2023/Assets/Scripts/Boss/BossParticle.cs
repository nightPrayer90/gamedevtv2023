using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossParticle : MonoBehaviour
{
    public List<ParticleSystem> bossparticles;
    private bool particleSwitch = false;

    public void ParticleStart()
    {
        if (particleSwitch == false)
        {
            foreach (ParticleSystem item in bossparticles)
            {
                item.Play();
            }
            particleSwitch = true;
        }
    }

    public void ParticleStop()
    {
        if (particleSwitch == true)
        {
            foreach (ParticleSystem item in bossparticles)
            {
                item.Stop();
            }
            particleSwitch = false;
        }
    }


}

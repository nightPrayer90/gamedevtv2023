using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossParticle : MonoBehaviour
{
    public List<ParticleSystem> bossparticles;
    public GameObject orbitParticle;
    public GameObject lightningParticle;
    public GameObject pillarParticle;

    private bool particleSwitch = false;

    private void Start()
    {
        gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        if (orbitParticle != null) orbitParticle.transform.position = new Vector3(transform.position.x, 6f, transform.position.z);
        if (lightningParticle != null) lightningParticle.transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
        if (pillarParticle != null) pillarParticle.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

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

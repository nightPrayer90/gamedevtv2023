using UnityEngine;
using System.Collections;
public class EnemyParticleBullet : MonoBehaviour
{
    public int bulletDamage = 0;
    public ParticleSystem bulletParticleSystem;

    public bool canReflect = false;

    //List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    public void Start()
    {
        bulletParticleSystem = GetComponent<ParticleSystem>();
    }


    public void HardBulletStop()
    {
        bulletParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}

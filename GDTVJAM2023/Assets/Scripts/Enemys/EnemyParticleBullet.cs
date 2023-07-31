using UnityEngine;
using System.Collections;
public class EnemyParticleBullet : MonoBehaviour
{
    public int bulletDamage = 0;
    public ParticleSystem bulletParticleSystem;


    //List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    public void Start()
    {
        bulletParticleSystem = GetComponent<ParticleSystem>();
    }


  /*  public void BulletStart_()
    {
        if (bulletParticleSystem.isStopped)
            bulletParticleSystem.Play();
        isEmitting = true;
    }
  */

    public void HardBulletStop()
    {
        bulletParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

}

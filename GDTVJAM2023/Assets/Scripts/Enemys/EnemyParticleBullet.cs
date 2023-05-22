using UnityEngine;

public class EnemyParticleBullet : MonoBehaviour
{
    public int bulletDamage = 1;
    public float baseFireRate = 1f;
    //private GameManager gameManager;
    public ParticleSystem particleSystem;
    public bool fireOnBirth = false;

    //List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    public void Start()
    {
        if (fireOnBirth)
        {
            BulletStart(bulletDamage, baseFireRate);
        }
    }

    public void BulletStart(int bulletDamage_, float fireRate)
    {
        //Set Damage
        bulletDamage = bulletDamage_;
        
        //Set FireRate
        var main = particleSystem.main;
        main.duration = fireRate;

        particleSystem.Play();
    }

    public void BulletStop()
    {
        particleSystem.Stop();
    }

    public void HardBulletStop()
    {
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void BulletSetDamage(int bulletDamage_)
    {
        bulletDamage = bulletDamage_;
    }


    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out PlayerController en))
        {
            en.UpdatePlayerHealth(bulletDamage);
        }
    }



}

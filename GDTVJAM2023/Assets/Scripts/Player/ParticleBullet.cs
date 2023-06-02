using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ParticleBullet : MonoBehaviour
{
    public int bulletDamage = 1;
    //private GameManager gameManager;
    public ParticleSystem particleSystem;
    public float interval = 1f;
    private bool isEmitting = false;

    public bool isEmittingSound = false;
    public string shootSound;
    private Coroutine shootingSound;

    void OnEnable()
    {
        //particleSystem = GetComponent<ParticleSystem>();
    }

    private IEnumerator TriggerParticleSound()
    {
        while (true)
        {
            if (isEmitting)
            {
                // Emitiere ein Partikel
                AudioManager.Instance.PlaySFX(shootSound);
            }

            yield return new WaitForSeconds(interval);
        }
    }

    public void BulletStart(int bulletDamage_, float fireRate)
    {
        if (isEmittingSound == true)
        {
            shootingSound = StartCoroutine(TriggerParticleSound());
            isEmitting = true;
        }

        //Set Damage
        bulletDamage = bulletDamage_;

        //Set FireRate
        var main = particleSystem.main;
        main.duration = fireRate;

        particleSystem.Play();
        
        interval = fireRate;
        
    }

    public void BulletStop()
    {
        particleSystem.Stop();
        isEmitting = false;
        if (shootingSound != null)
        {
            StopCoroutine(shootingSound);
            shootingSound = null;
        }
    }

    public void HardBulletStop()
    {
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        isEmitting = false;
        if (shootingSound != null)
        {
            StopCoroutine(shootingSound);
            shootingSound = null;
        }
    }

    public void BulletSetDamage(int bulletDamage_)
    {
        bulletDamage = bulletDamage_;
    }


    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out EnemyHealth en))
        {
            en.TakeDamage(bulletDamage);
            
        }
        AudioManager.Instance.PlaySFX("ImpactShot");
    }



  

}

using UnityEngine;
using System.Collections;
public class EnemyParticleBullet : MonoBehaviour
{
    public int bulletDamage = 1;
    public float baseFireRate = 1f;
    //private GameManager gameManager;
    public ParticleSystem particleSystem;
    public bool fireOnBirth = false;
    public float interval = 1f;
    private bool isEmitting = false;
    public bool isEmittingSound = true;
    //public string shootSound;

    public AudioSource audioSource;

    //List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    public void Start()
    {
        if (fireOnBirth)
        {
            BulletStart(bulletDamage, baseFireRate);
        }
        if (isEmittingSound == true)
        {
            StartCoroutine(TriggerParticleSound());
            isEmitting = true;
        }
    }

    private IEnumerator TriggerParticleSound()
    {
        while (true)
        {
            if (isEmitting)
            {
                if (audioSource != null)
                {
                    // Emitiere ein Partikel
                    audioSource.volume = AudioManager.Instance.sfxVolume;
                    audioSource.Play();
                }
            }

            yield return new WaitForSeconds(interval);
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

        interval = fireRate;
        isEmitting = true;
    }

    public void BulletStart_()
    {
        if (particleSystem.isStopped)
            particleSystem.Play();
        isEmitting = true;
    }

    public void BulletStop()
    {
        particleSystem.Stop();
        isEmitting = false;
    }

    public void HardBulletStop()
    {
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        isEmitting = false;
    }

    public void BulletSetDamage(int bulletDamage_)
    {
        bulletDamage = bulletDamage_;
    }


    private void OnParticleCollision(GameObject other)
    {
        
        
        if (other.CompareTag("Shield"))
        {
    
            if (other.TryGetComponent(out ShieldController en))
            {
                en.UpdateShieldHealth(bulletDamage);
            }

        }
        else
        {
            if (other.TryGetComponent(out PlayerController en))
            {
                en.UpdatePlayerHealth(bulletDamage);
            }
        }
    }



}

using UnityEngine;
using System.Collections;
public class EnemyParticleBullet : MonoBehaviour
{
    public int bulletDamage = 1;
    public float baseFireRate = 1f;
    //private GameManager gameManager;
    public ParticleSystem bulletParticleSystem;
    public bool fireOnBirth = false;
    public float interval = 1f;
    private bool isEmitting = false;
    public bool isEmittingSound = true;
    //public string shootSound;
    private Coroutine shootSoundCoroutine;

    public AudioSource audioSource;

    //List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    public void Start()
    {
        bulletParticleSystem = gameObject.GetComponent<ParticleSystem>();
        if (fireOnBirth)
        {
            BulletStart(bulletDamage, baseFireRate);
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
        if (isEmittingSound == true)
        {
            shootSoundCoroutine = StartCoroutine(TriggerParticleSound());
            isEmitting = true;
        }

        //Set Damage
        bulletDamage = bulletDamage_;
        
        //Set FireRate
        var main = bulletParticleSystem.main;
        main.duration = fireRate;

        bulletParticleSystem.Play();

        interval = fireRate;
        isEmitting = true;
    }

    public void BulletStart_()
    {
        if (bulletParticleSystem.isStopped)
            bulletParticleSystem.Play();
        isEmitting = true;
    }

    public void BulletStop()
    {
        bulletParticleSystem.Stop();
        isEmitting = false;
    }

    public void HardBulletStop()
    {
        bulletParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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

    void OnDestroy()
    {
        StopAllCoroutines();
        //StopCoroutine(shootSoundCoroutine);
    }

}

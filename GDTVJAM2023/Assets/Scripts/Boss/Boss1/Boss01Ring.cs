using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01Ring : MonoBehaviour
{
    public List<ParticleSystem> particleWeapons1 = new List<ParticleSystem>();
    public List<ParticleSystem> channelWeapons1 = new List<ParticleSystem>();
    public AudioSource shootSound;

    public List<ParticleSystem> particleWeapons2 = new List<ParticleSystem>();
    public List<ParticleSystem> channelWeapons2 = new List<ParticleSystem>();

    public ParticleSystem diePartcle;
    public ParticleSystem windParticle;

    public GameObject dieObject;
    public MeshRenderer mesh;

    public AudioSource explosionSound;

    private void OnEnable()
    {
        foreach (ParticleSystem weapon in channelWeapons1)
        {
            weapon.Stop();
        }
        foreach (ParticleSystem weapon in channelWeapons2)
        {
            weapon.Stop();
        }
    }

    // rotate 
    public void RotateRing(float rotationSpeed)
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime);
    }


    // Attack Phase 1
    public void ShootingAttack01()
    {
        Invoke(nameof(ChannelShooting01FX), 0.5f);
        InvokeRepeating(nameof(Shoot01), 1.5f, 0.5f);
    }

    public void StopShooting01()
    {
        CancelInvoke(nameof(Shoot01));
        windParticle.Stop();
        foreach (ParticleSystem weapon in channelWeapons1)
        {
            weapon.Stop();
        }
    }

    private void ChannelShooting01FX()
    {
        windParticle.Play();
        AudioManager.Instance.PlaySFX("ShieldRegenerate");
        foreach (ParticleSystem weapon in channelWeapons1)
        {
            weapon.Play();
        }
    }

    private void Shoot01()
    {
        shootSound.volume = AudioManager.Instance.sfxVolume;
        shootSound.Play();
        foreach (ParticleSystem weapon in particleWeapons1)
        {
            weapon.Emit(1);
        }
    }

    // Attack Phase 2
    public void ShootingAttack02()
    {
        Invoke(nameof(ChannelShooting02FX), 0.5f);
        InvokeRepeating(nameof(Shoot02), 1.5f, 0.5f);
    }

    public void StopShooting02()
    {
        windParticle.Stop();
        CancelInvoke(nameof(Shoot02));
        foreach (ParticleSystem weapon in channelWeapons2)
        {
            weapon.Stop();
        }
    }

    private void ChannelShooting02FX()
    {
        windParticle.Play();
        AudioManager.Instance.PlaySFX("ShieldRegenerate");
        foreach (ParticleSystem weapon in channelWeapons2)
        {
            weapon.Play();
        }
    }

    private void Shoot02()
    {
        shootSound.volume = AudioManager.Instance.sfxVolume;
        shootSound.Play();
        foreach (ParticleSystem weapon in particleWeapons2)
        {
            weapon.Emit(1);
        }
    }

    //destroy
    public void Explode()
    {
        
        StopShooting01();
        StopShooting02();

        Invoke(nameof(ExplodeRingInvoke), 1f);
        Invoke(nameof(DestroyRing), 10f);
    }

    private void ExplodeRingInvoke() 
    {
        diePartcle.Play();
        explosionSound.volume = AudioManager.Instance.sfxVolume;
        explosionSound.Play();
        mesh.enabled = false;
        Instantiate(dieObject, transform.position, transform.rotation);
    }

    private void DestroyRing()
    {
        Destroy(gameObject);
    }

}

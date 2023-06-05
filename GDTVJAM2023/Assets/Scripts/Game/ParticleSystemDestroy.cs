using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroy : MonoBehaviour
{
    public ParticleSystem particleSystemToCheck;
    public bool isEmittingSound = true;
    //public string shootSound;

    public AudioSource audioSource;


    private void OnEnable()
    {
        //AudioManager.Instance.PlaySFX("Explosion");
        if (isEmittingSound == true && audioSource != null)
        {
            audioSource.volume = AudioManager.Instance.sfxVolume;
            audioSource.Play();
        }
    }

    private void OnParticleSystemStopped()
    {
         ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroy : MonoBehaviour
{
    public ParticleSystem particleSystemToCheck;
    public bool isEmittingSound = true;
    //public string shootSound;

    public AudioSource audioSource;
    // Update is called once per frame
    private void Start()
    {
        //AudioManager.Instance.PlaySFX("Explosion");
        if (isEmittingSound == true && audioSource != null)
        {
            audioSource.volume = AudioManager.Instance.sfxVolume;
            audioSource.Play();
        }
    }
    void Update()
    {
        if (!particleSystemToCheck.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}

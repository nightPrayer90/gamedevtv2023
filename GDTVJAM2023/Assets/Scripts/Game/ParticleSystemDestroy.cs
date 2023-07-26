using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroy : MonoBehaviour
{
    public ParticleSystem particleSystemToCheck;
    public ParticleSystem rippleParticleSystem;
    public bool isEmittingSound = true;
    //public string shootSound;

    public AudioSource audioSource;
    public float rippleParicleSize = 1f;
    private bool isRipplePlayed = false;
    private RipplePostProcessor mainCamera;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<RipplePostProcessor>();
    }

    private void OnEnable()
    {
        //AudioManager.Instance.PlaySFX("Explosion");
        if (isEmittingSound == true && audioSource != null)
        {
            audioSource.volume = AudioManager.Instance.sfxVolume;
            audioSource.Play();
        }
        Invoke("DeactivateSystem", 10f);

        isRipplePlayed = false;
    }

    private void LateUpdate()
    {
        if (rippleParticleSystem != null && isRipplePlayed == false)
        {
            var mainModule = rippleParticleSystem.main;
            mainModule.startSize = rippleParicleSize * 3f; // factor = size rippleParticle/3

            rippleParticleSystem.Play();
            mainCamera.RippleEffect(transform.position.x, transform.position.y, 5f, 0.9f);
            isRipplePlayed = true;
        }
    }


    private void DeactivateSystem()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rippleParicleSize); 
    }
}

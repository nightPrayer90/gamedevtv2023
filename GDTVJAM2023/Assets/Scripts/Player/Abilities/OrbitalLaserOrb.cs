using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalLaserOrb : MonoBehaviour
{
    public ParticleSystem orbParticle;
    public ParticleSystem hitParticle;

    private int damage;
    private float realoadTime;

    public int index;

    // Start is called before the first frame update
    void Start()
    {
        PlayerWeaponController pwc = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();

        damage = pwc.olDamage;
        realoadTime = pwc.bwRealoadTime;

        Invoke("ActivateOrb", index);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("secondDimensionEnemy"))
        {
            EnemyHealth eh = other.GetComponent<EnemyHealth>();
            eh.TakeLaserDamage(damage);
            eh.ShowDamageFromObjects(damage);

            Invoke("ActivateOrb", realoadTime);

            hitParticle.transform.position = gameObject.transform.position;
            hitParticle.Play();

            orbParticle.Stop();
            gameObject.SetActive(false);
        }
    }

    private void ActivateOrb()
    {
        orbParticle.Play();
        gameObject.SetActive(true);
        AudioManager.Instance.PlaySFX("PlayerOrbitalSound");
    }
}

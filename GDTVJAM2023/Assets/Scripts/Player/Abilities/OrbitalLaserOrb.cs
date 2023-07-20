using UnityEngine;

public class OrbitalLaserOrb : MonoBehaviour
{
    public ParticleSystem orbParticle;
    public ParticleSystem hitParticle;
    public OrbitalLaser orbitalLaser;

    public int damage;
    public float realoadTime;

    public int index;

    // Start is called before the first frame update
    void Start()
    {
        damage = orbitalLaser.damage;
        realoadTime = orbitalLaser.realoadTime;

        Invoke("ActivateOrb", index);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("secondDimensionEnemy"))
        {
            EnemyHealth eh = other.GetComponent<EnemyHealth>();

            if (eh.canTakeDamage == true)
            {
                eh.TakeLaserDamage(damage);
                eh.ShowDamageFromObjects(damage);
            }
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

using DG.Tweening.Core.Easing;
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

            if (eh != null)
            {
                if (eh.canTakeDamage == true)
                {
                    eh.TakeLaserDamage(damage, 3);
                    eh.ShowDamageFromObjects(damage);
                }
            }
            else
            {
                
                EnemyShield es = other.transform.GetComponentInParent<EnemyShield>();

                Debug.Log(es);

                if (es != null)
                {
                    es.ShieldGetDamage();
                    es.ShowDamageFromPosition(transform.position);
                }
            }

            DestroyOrb();
        }
    }

    private void OnParticleCollision(GameObject other)
    {

        Debug.Log("Collision");
        /*if (upgradeChooseList.weaponIndexInstalled[57] == false) //todo to true
        {
            ParticleSystem part = other.GetComponent<ParticleSystem>(); // *** important! Making a variable to acess the particle system of the emmiting object, in this case, the lasers from my player ship.


        }*/
    }

    // if the Orb Collide with an Enemy laser2
    public void DestroyOrb()
    {
        Invoke("ActivateOrb", realoadTime);

        hitParticle.transform.position = gameObject.transform.position;
        hitParticle.Play();

        orbParticle.Stop();
        gameObject.SetActive(false);
    }

    private void ActivateOrb()
    {
        orbParticle.Play();
        gameObject.SetActive(true);
        AudioManager.Instance.PlaySFX("PlayerOrbitalSound");
    }
}

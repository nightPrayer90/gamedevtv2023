using UnityEngine;
using DG.Tweening;

public class PlayerBurningGroundController : MonoBehaviour
{
    public float lifetime;
    public ParticleSystem burningFieldParticle;
    public float scaleFactor = 1f;

    private void Start()
    {
        burningFieldParticle.Play();
        Invoke(nameof(InvokeDestroy), lifetime);
        gameObject.transform.DOScale(new Vector3(0.5f * scaleFactor, 0.5f * scaleFactor, 0.5f * scaleFactor),0.3f);
        gameObject.transform.position = new Vector3(transform.position.x, 5.9f, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("secondDimensionEnemy"))
        {
            EnemyHealth EnH = other.GetComponent<EnemyHealth>();

            if (EnH != null)
            {
                EnH.InvokeBurningDamage();
            }
        }
    }


    private void InvokeDestroy()
    {
        gameObject.transform.DOScale(new Vector3(0f, 0f, 0f), 0.3f).OnComplete(()=>
        {
            burningFieldParticle.Stop();
            Destroy(gameObject);
        });
    }

}

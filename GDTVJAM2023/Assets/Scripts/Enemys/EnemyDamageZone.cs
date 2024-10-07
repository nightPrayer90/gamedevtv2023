using UnityEngine;
using DG.Tweening;

public class EnemyDamageZone : MonoBehaviour
{
    public float lifetime;
    public ParticleSystem burningFieldParticle;
    public float scaleFactor = 1f;
    public int damage = 1;
    private bool doDamage = false;
    public ParticleSystem hitParticle;
    public Transform hitParticleTransform;

    private void Start()
    {
        burningFieldParticle.Play();
        Invoke(nameof(InvokeDestroy), lifetime);
        gameObject.transform.DOScale(new Vector3(1f * scaleFactor, 1f * scaleFactor, 1f * scaleFactor),0.5f);
        gameObject.transform.position = new Vector3(transform.position.x, 5.9f, transform.position.z);

        AudioManager.Instance.PlaySFX("DamageFieldSpawn");

        Invoke(nameof(SetDamageBool), 1f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NewPlayerController playerController = other.gameObject.GetComponent<NewPlayerController>();

            if (playerController != null)
            {
                if (doDamage == true)
                {
                    AudioManager.Instance.PlaySFX("DamageFieldHitSound");
                    playerController.GetLaserDamage(damage);
                    Invoke(nameof(SetDamageBool), 1.5f);
                    doDamage = false;

                    hitParticleTransform.position = playerController.gameObject.transform.position;
                    hitParticle.Play();
                }
           
            }
        }
    }

    private void SetDamageBool()
    {
        doDamage = true;
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

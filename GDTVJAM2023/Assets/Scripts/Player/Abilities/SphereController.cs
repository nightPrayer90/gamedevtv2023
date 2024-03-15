using UnityEngine;
using DG.Tweening;

public class SphereController : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public Color hitColor;
    private GameManager gameManager;
    [HideInInspector] public float detectRange;
    [HideInInspector] public float forcePower;
    public ParticleSystem hitParticle;
    public ParticleSystem particleSystem;

    private void OnEnable()
    {
        transform.DOScale(new Vector3(0.065f, 0.065f, 0.065f), 0.5f).SetUpdate(true);
        InvokeRepeating("CheckForEnemy", 1, 0.5f);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }

        if (other.CompareTag(tagStr))
        {
            EnemyHealth eh = other.GetComponent<EnemyHealth>();

            if (eh != null)
            {
                if (eh.canTakeDamage == true)
                {
                    //eh.TakeLaserDamage(damage, 4);
                    AudioManager.Instance.PlaySFX("PlayerLaserHit");
                    hitParticle.Emit(15);
                    //eh.ShowDamageFromObjects(damage);
                    eh.InvokeBurningDamage();
                }
            }
            else
            {

                EnemyShield es = other.transform.GetComponentInParent<EnemyShield>();

                Debug.Log(es);

                if (es != null)
                {
                    es.ShieldGetDamage();
                    AudioManager.Instance.PlaySFX("PlayerLaserHit");
                    es.ShowDamageFromPosition(transform.position);
                }
            }

            DestroyOrb();
        }
    }

    void CheckForEnemy()
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag(tagStr))
            {
                Vector3 direction = (collider.transform.position - transform.position).normalized;
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(direction * forcePower, ForceMode.Impulse);
                }
                return; 
            }
        }
    }



    public void SetDestroyTimer(float lifetime)
    {
        Invoke("DestroyOrb", lifetime);
        Debug.Log(lifetime);
    }

    public void DestroyOrb()
    {
        particleSystem.Stop();
        CancelInvoke();
        transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetUpdate(true).OnComplete(() => { ObjectPoolManager.ReturnObjectToPool(gameObject); }) ;
    }
}

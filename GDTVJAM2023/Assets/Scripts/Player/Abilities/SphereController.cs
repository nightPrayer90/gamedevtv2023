using UnityEngine;
using DG.Tweening;

public class SphereController : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public Color hitColor;
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    public float detectRange;
    public float forcePower;
    public float sphereForceTime = 0.5f;
    private float shereForceTime_;
    public ParticleSystem hitParticle;
    public ParticleSystem spawnParticle;
    public Vector3 initalScale;

    private void OnEnable()
    {
        transform.DOScale(initalScale, 0.5f).SetUpdate(true);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();

        shereForceTime_ = sphereForceTime - (upgradeChooseList.weaponIndexInstalled[73] / 10);

        InvokeRepeating("CheckForEnemy", 0.5f, shereForceTime_);
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
                    AudioManager.Instance.PlaySFX("PlayerLaserHit");
                    hitParticle.Emit(15);

                    if (upgradeChooseList.weaponIndexInstalled[70] == 1)
                    {
                        eh.TakeLaserDamage(damage, 4);
                        eh.ShowDamageFromObjects(damage);
                    }
                    else
                    {
                        eh.InvokeBurningDamage();
                    }
                }
            }
            else
            {
                EnemyShield es = other.transform.GetComponentInParent<EnemyShield>();

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
    }

    public void DestroyOrb()
    {
        spawnParticle.Stop();
        CancelInvoke();
        transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetUpdate(true).OnComplete(() => { ObjectPoolManager.ReturnObjectToPool(gameObject); }) ;
    }
}

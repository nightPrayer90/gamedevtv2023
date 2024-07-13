using UnityEngine;
using DG.Tweening;

public class ab_LaserProjectile : MonoBehaviour
{
    public float initialForce = 25f;
    public Rigidbody rb;
    public int damage = 5;
    public int laserDamageChannel = 4;

    public ParticleSystem hitParticle;
    public ParticleSystem bladeParticle;
    private Transform playerTransform; // Reference to the player's transform

    private bool isStarted = false;
    private bool destroyFlag = false;

    public Vector3 projectileSize = new Vector3(1f, 1f, 1f);

    private float sizefactor;
    public AudioSource audioSource;

    void Awake()
    {
        audioSource.volume = AudioManager.Instance.sfxVolume;
        audioSource.Play();
        AudioManager.Instance.PlaySFX("LaserBeamShoot");
        playerTransform = GameObject.Find("NewPlayer").GetComponent<Transform>();
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(projectileSize, 0.1f);
        rb.AddForce(transform.forward * -initialForce, ForceMode.Impulse);
        isStarted = false;
        destroyFlag = false;

        Invoke(nameof(CanDoDamage), 0.05f);
        Invoke(nameof(DestroyProjectile), 2f);
        sizefactor = 1f;
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void CanDoDamage()
    {
        isStarted = true;
    }
  

    void OnTriggerEnter(Collider other)
    {
        if (isStarted == true && destroyFlag == false)
            if (other.CompareTag("Enemy") || other.CompareTag("secondDimensionEnemy"))
            {
                EnemyHealth eH = other.gameObject.GetComponent<EnemyHealth>();

                if (eH != null)
                {
                    if (eH.canTakeLaserDamage[laserDamageChannel] == true && eH.canTakeDamage == true)
                    {
                        eH.TakeLaserDamage(damage, laserDamageChannel);
                        eH.TakeBurningDamage();
                        eH.ShowDamageFromPosition(other.transform.position, damage);
                        hitParticle.transform.position = other.ClosestPointOnBounds(other.transform.position);
                        hitParticle.Emit(20);
                        sizefactor += 0.3f;

                        transform.DOComplete();
                        transform.DOScale(projectileSize * sizefactor, 0.1f);
                    }
                }
                else
                {
                    EnemyShield es = other.transform.GetComponentInParent<EnemyShield>();

                    if (es != null)
                    {
                        es.ShieldGetDamage();
                        es.ShowDamageFromPosition(other.ClosestPointOnBounds(other.transform.position));
                    }
                }

            }
    }

    private void DestroyProjectile()
    {
        if (destroyFlag == false)
        {
            transform.DOKill();
            AudioManager.Instance.PlaySFX("LaserBeamDestroy");
            transform.DOScale(projectileSize * 0, 0.2f).SetDelay(0.3f);
            bladeParticle.Stop();

            destroyFlag = true;
            Invoke(nameof(DestroyObject), 1f);
        }
    }

    private void DestroyObject()
    {
        bladeParticle.Stop();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}

using UnityEngine;
using DG.Tweening;
using System;

public class ab_LaserProjectile : MonoBehaviour
{
    public float initialForce = 25f;
    public Rigidbody rb;
    public int damage = 7;
    public int hitCounter = 0;
    public int laserDamageChannel = 4;
    public float lifeTime = 2f;

    public ParticleSystem hitParticle;
    public ParticleSystem bladeParticle;
    private Transform playerTransform; // Reference to the player's transform

    private bool isStarted = false;
    private bool destroyFlag = false;

    public Vector3 projectileSize = new Vector3(1f, 1f, 1f);

    private float sizefactor;
    public AudioSource audioSource;
    private UpgradeChooseList upgradeChooseList;
    public GameObject burningFildPrefab;
    public GameObject novaExplosion;

    void Awake()
    {
        audioSource.volume = AudioManager.Instance.sfxVolume;
        audioSource.Play();
        AudioManager.Instance.PlaySFX("LaserBeamShoot");
        playerTransform = GameObject.Find("NewPlayer").GetComponent<Transform>();
        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.DOScale(projectileSize, 0.1f);
        rb.AddForce(transform.forward * -initialForce, ForceMode.Impulse);
        isStarted = false;
        destroyFlag = false;

        Invoke(nameof(CanDoDamage), 0.05f);
        Invoke(nameof(DestroyProjectile), lifeTime);
        sizefactor = 1f;
        hitCounter = 0;
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
                        int resultDamage = damage;

                        if (upgradeChooseList.upgrades[96].upgradeIndexInstalled>0)
                        {
                            resultDamage = damage + hitCounter;
                        }
         
                        eH.TakeLaserDamage(resultDamage, laserDamageChannel);
                        eH.ShowDamageFromPosition(other.transform.position, resultDamage);

                        eH.InvokeBurningDamage();
                        hitParticle.transform.position = other.ClosestPointOnBounds(other.transform.position);
                        hitParticle.Emit(20);
                        sizefactor += (0.15f) * (1 + (0.15f * upgradeChooseList.upgrades[93].upgradeIndexInstalled));

                        transform.DOComplete();
                        transform.DOScale(projectileSize * sizefactor, 0.1f);

                        hitCounter++;
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

            if (upgradeChooseList.upgrades[94].upgradeIndexInstalled > 0)
            {
                // Nova
                if (upgradeChooseList.upgrades[95].upgradeIndexInstalled > 0)
                {
                    GameObject explosionObject = Instantiate(novaExplosion, transform.position, transform.rotation);
                    Explosion explosion = explosionObject.GetComponent<Explosion>();
                    explosion.InitExplosion(damage + hitCounter*2, 80 + hitCounter*2, 1 + hitCounter, false);
                }
                else
                {
                    float lifeTimeField = 8f;
                    GameObject burningShpere = Instantiate(burningFildPrefab, transform.position, transform.rotation);

                    if (upgradeChooseList.upgrades[97].upgradeIndexInstalled > 0)
                    {
                        lifeTimeField += lifeTimeField + hitCounter;
                    }

                    PlayerBurningGroundController bgC = burningShpere.GetComponent<PlayerBurningGroundController>();
                    bgC.scaleFactor = 2.3f + sizefactor;
                    bgC.lifetime = lifeTimeField;
                }
            }
        }
    }

    private void DestroyObject()
    {
        bladeParticle.Stop();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}

using UnityEngine;
using DG.Tweening;

public class ab_ShieldController : MonoBehaviour
{
    //private NewPlayerController playerController;
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    private Rigidbody playerRb;
    private Transform playerTr;
    private PlayerWeaponController playerWeaponController;
    private NewPlayerController playerController;
    private ab_FrontShield frontShieldSpawner;

    public Material targetMaterial;
    public GameObject novaOnDeath;

    private int shieldLife_;

    public ParticleSystem dieEffect;
    public ParticleSystem hitEffect;
    public ParticleSystem burnEffect;
    public GameObject shieldMesh;
    private Vector3 shieldSize;
    public BoxCollider shieldCollider;

    private bool canGetDamage = true;



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        // get player Components
        var player = GameObject.FindWithTag("Player");
        playerRb = player.GetComponent<Rigidbody>();
        playerTr = player.GetComponent<Transform>();
        playerWeaponController = player.GetComponent<PlayerWeaponController>();
        playerController = player.GetComponent<NewPlayerController>();
        frontShieldSpawner = player.GetComponentInChildren<ab_FrontShield>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.gameObject.GetComponent<UpgradeChooseList>();

        shieldSize = shieldMesh.transform.localScale;
        targetMaterial.DOFade(0, 0.01f);
    }

    // set position to player position
    private void LateUpdate()
    {
        if (playerRb != null)
        {
            transform.position = playerTr.position;
            transform.rotation = Quaternion.Euler(0f, playerTr.rotation.eulerAngles.y + 180f, 0f); ;
        }
    }

    // if collision with an Enemy
    private void OnCollisionEnter(Collision collision)
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }

        // shield destroy on collosion and gives the player a force in the backwards direction
        if (collision.gameObject.CompareTag(tagStr))
        {
            if (tagStr == "Enemy")
            {
                // player bounce
                Vector3 explosionDirection = collision.transform.position - transform.position;
                explosionDirection.Normalize();
                playerRb.AddForce(explosionDirection * -1f * .1f, ForceMode.Impulse);

                // enemy bounce
                Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
                enemyRb.AddForce(explosionDirection * 1f * 10f, ForceMode.Impulse);

                // enemy get Damage
                ShieldDamage(collision.gameObject);

                hitEffect.Play();

                // update shield health
                UpdateShieldHealth(0);
            }
            else
            {
                // player bounce
                Vector3 explosionDirection = collision.transform.position - transform.position;
                explosionDirection.Normalize();
                playerRb.AddForce(explosionDirection * -1f * 10f, ForceMode.Impulse);

                // destroy shild
                ShieldTypeDieControl();
            }
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>(); // *** important! Making a variable to acess the particle system of the emmiting object, in this case, the lasers from my player ship.
        var ps = other.GetComponent<EnemyParticleBullet>();

        if (ps != null)
            UpdateShieldHealth(1);
    }

    /* **************************************************************************** */
    /* SHIELD MANAGEMENT----------------------------------------------------------- */
    /* **************************************************************************** */
    // OnEnable Event for the shield object
    public void ShieldEnable( )
    {
        AudioManager.Instance.PlaySFX("ShieldActivate");
        shieldMesh.transform.DOPunchScale(new Vector3(1f, 1f, 1f), 1f, 5, 0.5f).SetUpdate(true);
        shieldLife_ = playerWeaponController.shipData.shieldHealth;

        // reset shild life
        canGetDamage = true;

        // set material Color
        shieldCollider.enabled = true;
        shieldMesh.SetActive(true);

        UpdateShieldColor();
    }

    // the shield get an hit from a bullet!
    public void UpdateShieldHealth(int damageTyp)
    {
        // Update shield life
        if (canGetDamage == true)
        {
            shieldLife_ = Mathf.Max(0, shieldLife_ - 1);
            canGetDamage = false;
            Invoke(nameof(CanDamageControl), 0.4f);

            //CancelInvoke(nameof(Shieldregenerate);


            // destroy shield
            if (shieldLife_ <= 0)
            {
                ShieldTypeDieControl();
                AudioManager.Instance.PlaySFX("ShieldDie");
            }
            else
            {
                // update shield color;
                shieldMesh.transform.DOKill();
                shieldMesh.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.3f, 5, 0.5f).OnComplete(() => { shieldMesh.transform.localScale = shieldSize; });
                UpdateShieldColor();
                //Invoke("Shieldregenerate", 5f);
                if (damageTyp == 1) AudioManager.Instance.PlaySFX("ShieldGetHit");
                else AudioManager.Instance.PlaySFX("ShieldCollision");
            }
        }
    }

    private void CanDamageControl()
    {
        canGetDamage = true;
    }

    // Update the weapon and shieldcontroller if the ShildObject die
    private void ShieldTypeDieControl()
    {
        // update crit damage
        if (upgradeChooseList.upgrades[45].upgradeIndexInstalled > 0)
        {
            playerWeaponController.shipData.critDamage += 2;
            burnEffect.Emit(5);
        }

        // update AOE size
        if (upgradeChooseList.upgrades[44].upgradeIndexInstalled > 0)
        {
            playerWeaponController.shipData.rocketAOERadius += 2;
            burnEffect.Emit(5);
        }

        // trigger Nova
        if (upgradeChooseList.upgrades[62].upgradeIndexInstalled > 0)
        {
            NovaOnDeath(5, 10);
        }

        // set the shild die Effect
        dieEffect.Play();

        // deactivate the shield Object
        targetMaterial.DOFade(0f, 0.01f);
        shieldCollider.enabled = false;
        shieldMesh.SetActive(false);
        frontShieldSpawner.SetShieldFlag();
    }

    // update the alpha value from the shield material 
    private void UpdateShieldColor()
    {
        float newAlpha = 1f - (0.9f / shieldLife_);

        targetMaterial.DOKill();
        targetMaterial.DOFade(newAlpha, 0.5f);
    }

    // shield make damage
    private void ShieldDamage(GameObject collsion)
    {
        EnemyHealth enH = null;

        if (playerWeaponController.shipData.shieldDamage > 0)
        {
            enH = collsion.GetComponent<EnemyHealth>();

            if (enH != null)
            {
                enH.TakeDamage(playerWeaponController.shipData.shieldDamage);
                enH.ShowDamageFromObjects(playerWeaponController.shipData.shieldDamage);
                hitEffect.Emit(30);
            }
        }
        if (upgradeChooseList.upgrades[43].upgradeIndexInstalled > 0)
        {
            if (enH == null)
            {
                enH = collsion.GetComponent<EnemyHealth>();
            }
            if (enH != null)
            {
                enH.InvokeBurningDamage();
                enH.burnTickCount = 0;
                burnEffect.Emit(10);
            }

        }
    }

    // trigger a nova on Death
    public void NovaOnDeath(float explosionRadius, int NovaDamage)
    {
        // Audio
        AudioManager.Instance.PlaySFX("Playernova");

        Vector3 pos = transform.position;
        LayerMask layerMask = (1 << 6);
        explosionRadius = explosionRadius * (1 + playerWeaponController.shipData.rocketAOERadius / 100);

        if (gameManager.dimensionShift == true)
        {
            layerMask = (1 << 9);
        }

        // array of all Objects in the explosionRadius
        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

        foreach (var obj in surroundingObjects)
        {
            // get rigidbodys from all objects in range
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            // calculate distance between explosioncenter and objects in Range
            float distance = Vector3.Distance(pos, rb.transform.position);

            if (distance < explosionRadius)
            {
                float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
                int adjustedDamage = Mathf.CeilToInt(NovaDamage * scaleFactor);

                // get EnemyHealthscript
                EnemyHealth eHC = obj.GetComponent<EnemyHealth>();
                Color resultColor = playerController.enemyHitColor;

                if (eHC != null)
                {
                    if (upgradeChooseList.upgrades[54].upgradeIndexInstalled > 0)
                    {
                        int ran = Random.Range(0, 100);
                        if (ran < playerWeaponController.shipData.bulletCritChance)
                        {
                            adjustedDamage = eHC.CritDamage(adjustedDamage);
                            resultColor = eHC.critColor;
                        }
                    }

                    // show floating text
                    if (eHC.canTakeDamage == true)
                        gameManager.DoFloatingText(rb.transform.position, adjustedDamage.ToString(), resultColor);

                    // calculate enemy damage
                    eHC.TakeExplosionDamage(adjustedDamage);

                }
                rb.AddExplosionForce(400, pos, explosionRadius);
            }
        }

        GameObject go = ObjectPoolManager.SpawnObject(novaOnDeath, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMineController : MonoBehaviour
{
    [Header("Float Control")]
    public float floatHeight = 0.2f;
    public float floatSpeed = 1f;
    public float maxLifeTime = 10f;
    private float iniLifeTime = 0;
    private bool detected = false;

    private Vector3 startPosition;
    public Material originalMaterial;
    public Material detectedMaterial;
    private Renderer mineRenderer;
    private PlayerWeaponController playerWeaponController;
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    private Explosion explosion;

    [Header("Explosion Control")]
    public int damage = 8;
    public float explosionRadius = 2f;
    public int explosionForce;
    [HideInInspector] public bool isMainWeapon = false;


    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        playerWeaponController = GameObject.Find("NewPlayer").GetComponent<PlayerWeaponController>();
        mineRenderer = GetComponentInChildren<Renderer>();
        explosion = gameObject.GetComponent<Explosion>();

        iniLifeTime = maxLifeTime;
    }

    private void OnEnable()
    {
        // Reset
        Material[] materials = mineRenderer.materials;
        materials[1] = originalMaterial;
        mineRenderer.materials = materials;

        detected = false;

        SetDestroyTimer();
    }

    void OnTriggerEnter(Collider other)
    {
        // enemy target tag set
        string tagStr = "Enemy";
        LayerMask layerMask = (1 << 6);
        if (gameManager.dimensionShift == true)
        {
            layerMask = (1 << 9);
            tagStr = "secondDimensionEnemy";
        }

        // enemy target tag compare only than destroy the rocked
        if (other.CompareTag(tagStr))
        {
            Material[] materials = mineRenderer.materials;
            materials[1] = detectedMaterial;
            mineRenderer.materials = materials;

            if (detected == false)
            {
                CancelInvoke();
                AudioManager.Instance.PlaySFX("PlayerMineDetect");
                detected = true;
                Invoke("Explode", 0.5f);
            }
        }
    }

    /*private void Explode()
    {
        // postion of explosion Object
        Vector3 pos = transform.position;

        // cancle invoke
        CancelInvoke("DestroyObject");

        // array of all Objects in explosionRadius
        float explosionRadius_ = explosionRadius * (1+playerWeaponController.shipData.rocketAOERadius/100);
        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius_, layerMask);

        foreach (var obj in surroundingObjects)
        {
            // control is shield;
            EnemyShield eSh = obj.GetComponent<EnemyShield>();
            if (eSh != null)
            {
                int shieldDamage = 1;
                eSh.ShieldGetDamage(shieldDamage);
                gameManager.DoFloatingText(transform.position, shieldDamage.ToString(), eSh.hitColor);
                continue;
            }

            // get rigidbodys from all objects in range
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;


            // calculate distance between explosioncenter and objects in Range
            float distance = Vector3.Distance(pos, rb.transform.position);


            // get EnemyHealthscript
            EnemyHealth eHC = obj.GetComponent<EnemyHealth>();
            Color resultColor = hitColor;
            int adjustedDamage = damage;

            if (eHC.isBoss == true && isMainWeapon == true)
            {
                adjustedDamage = Mathf.CeilToInt((float)damage * (1 + (float)playerWeaponController.shipData.bossBonusDamage / 100));
            }

            if (eHC != null)
            {
                if (distance < explosionRadius_)
                {
                    float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius_), 1f);
                    adjustedDamage = Mathf.CeilToInt(adjustedDamage * scaleFactor);
                }

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
                    gameManager.DoFloatingText(obj.transform.position, adjustedDamage.ToString(), resultColor);

                // calculate enemy damage
                eHC.TakeExplosionDamage(adjustedDamage);
            }
            rb.AddExplosionForce(explosionForce, pos, explosionRadius);
        }


        // spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(exposionHitObject, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius_;

        // object goes back to the pool
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }*/

    public void SetDestroyTimer()
    {
        // destroytime
        maxLifeTime = Random.Range(iniLifeTime - 0.05f, iniLifeTime + 0.05f) ;
        Invoke(nameof(Explode), maxLifeTime); 
    }

    private void Explode()
    {
        explosion.InitExplosion(damage, explosionForce, explosionRadius, isMainWeapon);
    }
}

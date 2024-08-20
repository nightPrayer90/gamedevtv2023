
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private PlayerWeaponController playerWeaponController;
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;

    [Header("Explosion Settings")]
    public GameObject explosionPrefab;
    private int novaOnDieTriggerType = -1;
    private int expDamage = 0;
    private int expForce = 50;
    private float expRadius = 1;
    private bool isMainWeapon = false;

    private void Awake()
    {
        playerWeaponController = GameObject.Find("NewPlayer").GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
    }
    

    public void InitExplosion(int damage, int force, float radius, bool isMainWeapon, GameObject collisionTarget = null, int novaOnDieTriggerType = -1)
    {
        expDamage = damage;
        expForce = force;
        expRadius = radius;
        this.isMainWeapon = isMainWeapon;
        this.novaOnDieTriggerType = novaOnDieTriggerType;

        Explode(collisionTarget);
    }

    private void Explode(GameObject collisionTarget = null)
    {
        // set Layermask
        LayerMask layerMask = gameManager.dimensionShift? layerMask = (1 << 9) : layerMask = (1 << 6);

        // postion of explosion Object
        Vector3 pos = transform.position;

        // array of all Objects in explosionRadius
        float explosionRadius_ = expRadius * (1 + playerWeaponController.shipData.rocketAOERadius / 100);
        var surroundingObjects = Physics.OverlapSphere(pos, explosionRadius_, layerMask);

        foreach (var obj in surroundingObjects)
        {
            // control is shield;
            EnemyShield eSh = obj.GetComponent<EnemyShield>();
            if (eSh != null)
            {
                int shieldDamage = 1;
                eSh.ShieldGetDamage(shieldDamage);
                gameManager.DoFloatingText(pos, shieldDamage.ToString(), eSh.hitColor);
                continue;
            }

            // get rigidbodys from all objects in range
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            // calculate distance between explosioncenter and objects in Range
            float distance = Vector3.Distance(pos, rb.transform.position);

            // get EnemyHealthscript
            EnemyHealth eHC = obj.GetComponent<EnemyHealth>();
            Color resultColor = eHC.hitColor;
            int adjustedDamage = expDamage;


            if (eHC != null)
            {
                eHC.novaOnDieTriggerType = novaOnDieTriggerType;

                // titanSlayer
                if (eHC.isBoss == true && isMainWeapon == true)
                {
                    adjustedDamage = Mathf.CeilToInt((float)expDamage * (1 + (float)playerWeaponController.shipData.bossBonusDamage / 100));
                }

                if (obj.gameObject != collisionTarget)
                {
                    if (distance < explosionRadius_)
                    {
                        float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius_), 1f);
                        adjustedDamage = Mathf.CeilToInt(adjustedDamage * scaleFactor);
                    }
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
            rb.AddExplosionForce(expForce, pos, explosionRadius_);
        }

        // spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(explosionPrefab, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius_;

        // object goes back to the pool
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

}

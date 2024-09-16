
using UnityEngine;

public class DroneNovaAttack : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject explosionFX;
    private DroneController droneController;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        droneController = gameObject.GetComponentInParent<DroneController>();
        droneController.TriggerAttack += OnTriggerAttack;
    }

    private void OnDestroy()
    {
        droneController.TriggerAttack -= OnTriggerAttack;
    }

    private void OnTriggerAttack()
    {
        SpawnNova();
    }

    // Invoke, spawns a rocked after spawnInterval sec
    private void SpawnNova()
    {
        // Audio
        int explosionRadius = 2;
        int novaDamage = 5;
        int explosionForce = 300;
        AudioManager.Instance.PlaySFX("Playernova");

        // postion of explosion Object
        Vector3 pos = transform.position;

        // Layermask
        LayerMask layerMask = (1 << 6);
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
            //Debug.Log(distance);

            if (distance < explosionRadius)
            {
                float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
                int adjustedDamage = Mathf.CeilToInt(novaDamage * scaleFactor);


                // get EnemyHealthscript
                EnemyHealth eHC = obj.GetComponent<EnemyHealth>();


                if (eHC != null)
                {
                    Color resultColor = eHC.hitColor;
                    /*if (upgradeChooseList.upgrades[54].upgradeIndexInstalled > 0)
                    {
                        int ran = Random.Range(0, 100);
                        if (ran < weaponController.shipData.bulletCritChance)
                        {
                            adjustedDamage = eHC.CritDamage(adjustedDamage);
                            resultColor = eHC.critColor;
                        }
                    }*/

                    // show floating text
                    if (eHC.canTakeDamage == true)
                        gameManager.DoFloatingText(rb.transform.position, adjustedDamage.ToString(), resultColor);

                    // calculate enemy damage
                    eHC.TakeExplosionDamage(adjustedDamage);
                }
                rb.AddExplosionForce(explosionForce, pos, explosionRadius);
            }
        }


        // spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(explosionFX, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;
        go.transform.SetParent(gameObject.transform);
    }
}

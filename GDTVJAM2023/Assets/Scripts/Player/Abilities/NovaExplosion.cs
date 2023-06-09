using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaExplosion : MonoBehaviour
{
    private GameManager gameManager;
    
    [Header("Explosion Controll")]
    public int novaDamage = 5;
    public float spawnInterval = 4f;
    public float explosionRadius = 5f;
    public float explosionForce = 400f;
    public GameObject explosionFX;
    private LayerMask layerMask;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);
    private PlayerWeaponController weaponController;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();
        InvokeRepeating("SpawnNova", spawnInterval, spawnInterval);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }




    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController
    private void StartValues()
    {
        weaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        novaDamage = weaponController.neDamage;
        spawnInterval = weaponController.neReloadTime;
        explosionRadius = weaponController.neRadius;
    }


    // Invoke, spawns a rocked after spawnInterval sec
    private void SpawnNova()
    {
        // Audio
        AudioManager.Instance.PlaySFX("Playernova");

        // postion of explosion Object
        Vector3 pos = transform.position;

        // Layermask
        layerMask = (1 << 6);
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

                // calculate enemy damage
                eHC.TakeExplosionDamage(adjustedDamage);

                // show floating text
                gameManager.DoFloatingText(rb.transform.position, "+" + adjustedDamage.ToString(), hitColor);
            }

            rb.AddExplosionForce(explosionForce, pos, explosionRadius);
        }


        // spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(explosionFX, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

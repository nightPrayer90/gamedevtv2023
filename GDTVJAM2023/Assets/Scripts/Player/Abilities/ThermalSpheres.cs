using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalSpheres : MonoBehaviour
{
    [Header("Settings")]
    public int baseDamage = 5;
    public float lifeTime = 3f;
    public float spawnInterval = 1f;
    public float detectRange = 1f;
    public float forcePower = 0.4f;
    public GameObject sphereToLaunch;
    public GameObject spawnPoint;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();
        InvokeRepeating("SpawnSphere", spawnInterval, spawnInterval);
    }




    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController
    private void StartValues()
    {
        PlayerWeaponController weaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        baseDamage = weaponController.tsDamage;
        //lifeTime = weaponController.tsDamage;
        spawnInterval = weaponController.tsReloadTime;
    }


    // Invoke, spawns a rocked after spawnInterval sec
    private void SpawnSphere()
    {
        GameObject go = ObjectPoolManager.SpawnObject(sphereToLaunch, spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        SphereController sphere = go.GetComponent<SphereController>();
        sphere.damage = baseDamage;
        sphere.hitColor = hitColor;
        sphere.SetDestroyTimer(lifeTime);
        sphere.detectRange = detectRange;
        sphere.forcePower = forcePower; ;

        AudioManager.Instance.PlaySFX("PlayerRocketStart");
        if (rb != null)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            rb.AddForce(randomDirection * 0.5f, ForceMode.Impulse);
        }
    }

}



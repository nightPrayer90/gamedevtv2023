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
    public GameObject[] spheresToLaunch;
    public GameObject spawnPoint;
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);
    private UpgradeChooseList upgradeChooseList;


    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();
        StartValues();

    }




    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController
    public void StartValues()
    {
        PlayerWeaponController weaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        baseDamage = weaponController.tsDamage;
        lifeTime = weaponController.tsLifetime;
        spawnInterval = weaponController.tsReloadTime;
        UpdateInvoke();
    }

    public void UpdateInvoke()
    {
        CancelInvoke();
        InvokeRepeating("SpawnSphere", spawnInterval, spawnInterval);
    }


    // Invoke, spawns a rocked after spawnInterval sec
    private void SpawnSphere()
    {
        int sphereIndex = 0;
        if (upgradeChooseList.upgrades[70].upgradeIndexInstalled > 0)
            sphereIndex = 1;
        if (upgradeChooseList.upgrades[75].upgradeIndexInstalled > 0)
            sphereIndex = 2;

        GameObject go = ObjectPoolManager.SpawnObject(spheresToLaunch[sphereIndex], spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        SphereController sphere = go.GetComponent<SphereController>();
        sphere.damage = baseDamage;
        sphere.hitColor = hitColor;
        sphere.SetDestroyTimer(lifeTime);


        for (int i = 0; i <= upgradeChooseList.upgrades[72].upgradeIndexInstalled; i++)
        {
            int random = Random.Range(0, 100);
            if (random < 25)
            {
                go = ObjectPoolManager.SpawnObject(spheresToLaunch[sphereIndex], spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
                rb = go.GetComponent<Rigidbody>();
                sphere = go.GetComponent<SphereController>();
                sphere.damage = baseDamage;
                sphere.hitColor = hitColor;
                sphere.SetDestroyTimer(lifeTime);
            }
        }


        AudioManager.Instance.PlaySFX("PlayerFireFlies");
        if (rb != null)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            rb.AddForce(randomDirection * 0.5f, ForceMode.Impulse);
        }
    }

}



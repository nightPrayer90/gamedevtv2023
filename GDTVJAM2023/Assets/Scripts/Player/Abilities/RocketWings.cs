using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketWings : MonoBehaviour
{
    [Header("Bullet Particle")]
    public List<ParticleSystem> particleSystems;

    [Header("Weapon Settings")]
    public int rocketDamage = 10;
    public float relodInterval = 5f;
    public int salveMaxCount = 10;
    public float spawnInterval = 0.1f;
    public string audioClip = "";
    //public float rocketLifetime = 2f;
    private float nextSpawnTime = 0f;
    private int salveCount = 0;
    public GameObject rockedToLaunch;
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();
        salveCount = salveMaxCount;
       
    }

    // Update is called once per frame
    void Update()
    {
        Shooting();
    }




    /* **************************************************************************** */
    /* RUNTIME METHODEN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController
    private void StartValues()
    {
        PlayerWeaponController weaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        rocketDamage = weaponController.rwDamage;
        salveMaxCount = weaponController.rwSalveCount;
        relodInterval = weaponController.rwReloadTime;
    }

    // shooting controller
    void Shooting()
    {
        if (salveCount == salveMaxCount)
        {
            Invoke("RealodWeapon", relodInterval);
            salveCount++;
        }

        if (salveCount < salveMaxCount)
        {
            if (Time.time >= nextSpawnTime)
            {
                // shooting sound
                AudioManager.Instance.PlaySFX(audioClip);

                // emit 1 rocket
                GameObject go = ObjectPoolManager.SpawnObject(rockedToLaunch, spawnPoint1.position, Quaternion.Euler(0f, 180f, 0f) * spawnPoint1.rotation, ObjectPoolManager.PoolType.Gameobject);
                RocketController rocket = go.GetComponent<RocketController>();
                rocket.damage = rocketDamage;
                //rocket.hitColor = hitColor;
                //rocket.maxLifeTime = rocketLifetime;


                go = ObjectPoolManager.SpawnObject(rockedToLaunch, spawnPoint2.position, Quaternion.Euler(0f, 180f, 0f) * spawnPoint2.rotation, ObjectPoolManager.PoolType.Gameobject);
                rocket = go.GetComponent<RocketController>();
                rocket.damage = rocketDamage;
                //rocket.maxLifeTime = rocketLifetime;


                salveCount++;
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
    }

    // realod a salve of weapons
    void RealodWeapon()
    {
        salveCount = 0;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireflies : MonoBehaviour
{
    [Header("Bullet Particle")]
    public List<ParticleSystem> particleSystems;

    [Header("Weapon Settings")]
    public int bulletDamage = 10;
    public float realodInterval = 5f;
    public int bulletMaxCount = 10;
    public float spawnInterval = 0.1f;
    public string audioClip = "";
    private float nextSpawnTime = 0f;
    private int bulletCount = 0;




    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();
        bulletCount = bulletMaxCount;

        // set damage to particle system
        foreach (ParticleSystem weapon in particleSystems)
        {
            weapon.GetComponent<ParticleBullet>().bulletDamage = bulletDamage;
        }

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
        PlayerWeaponController weaponController = GameObject.Find("Player").GetComponent<PlayerWeaponController>();
        bulletDamage = weaponController.ffDamage;
        bulletMaxCount = weaponController.ffbulletCount;
        realodInterval = weaponController.ffReloadTime;
    }

    // shooting controller
    void Shooting()
    {
        if (bulletCount == bulletMaxCount)
        {
            Invoke("RealodWeapon", realodInterval);
            bulletCount++;
        }

        if (bulletCount < bulletMaxCount)
        {
            if (Time.time >= nextSpawnTime)
            {
                // shooting sound
                AudioManager.Instance.PlaySFX(audioClip);

                // emit 1 particle of each weapon
                foreach (ParticleSystem weapon in particleSystems)
                {
                    if (weapon != null)
                        weapon.Emit(1);
                        bulletCount++;
                }

                nextSpawnTime = Time.time + spawnInterval;
            }
        }
    }

    // realod a salve of weapons
    void RealodWeapon()
    {
        bulletCount = 0;
    }

}

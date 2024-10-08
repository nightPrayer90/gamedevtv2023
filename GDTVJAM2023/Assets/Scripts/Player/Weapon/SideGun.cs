using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideGun : MonoBehaviour
{
    [Header("Bullet Particle")]
    public List<ParticleSystem> particleSystems;

    [Header("Weapon Settings")]
    public int shootingside = 0;
    public int bulletDamage = 10;
    public float realodInterval = 5f;
    public int bulletMaxCount = 10;
    public string audioClip = "";
    private float nextSpawnTime = 0f;
    private int bulletCount = 0;
    public int bulltesPerShoot = 2;



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
        PlayerWeaponController weaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        bulletDamage = weaponController.sdgDamage;
        bulltesPerShoot = weaponController.sdgBulletsPerShoot;
        realodInterval = weaponController.sdgReloadTime;
    }

    // shooting controller
    void Shooting()
    {
        if (bulletCount == bulletMaxCount)
        {
            Invoke(nameof(RealodWeapon), realodInterval);
            bulletCount++;

            if (shootingside == 2) shootingside = 0;

            if (shootingside == 0) shootingside = 1;
            else if (shootingside == 1) shootingside = 0;

            int ran = Random.Range(0, 100);
            if (ran < 25)
            {
                shootingside = 2;
                bulletCount = (bulletMaxCount - 1);
            }
        }

        if (bulletCount < bulletMaxCount)
        {
            if (Time.time >= nextSpawnTime)
            {
                // shooting sound
                AudioManager.Instance.PlaySFX(audioClip);

                Debug.Log(shootingside);

                switch (shootingside)
                {
                    case 0:
                        particleSystems[0].Emit(bulltesPerShoot);
                        
                        break;
                    case 1:
                        particleSystems[1].Emit(bulltesPerShoot);
                        
                        break;
                    case 2:
                        particleSystems[2].Emit(bulltesPerShoot*10);
                        break;
                }

                bulletCount++;
                nextSpawnTime = Time.time + realodInterval;
            }
        }
    }

    // realod a salve of weapons
    void RealodWeapon()
    {
        bulletCount = 0;
    }

}

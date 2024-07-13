using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireflies : MonoBehaviour
{
    [Header("Bullet Particle")]
    public ParticleSystem particleSystem_;
    public ParticleSystem particleSystem1;
    public ParticleSystem particleSystem2;
    private ParticleBullet particleBullet;

    [Header("Weapon Settings")]
    public int bulletDamage = 10;
    public float realodInterval = 5f;
    public int bulletMaxCount = 10;
    public float spawnInterval = 0.1f;
    public string audioClip = "";
    private float nextSpawnTime = 0f;
    private int bulletCount = 0;
    private UpgradeChooseList upgradeChooseList;



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();

        //StartValues();
        bulletCount = bulletMaxCount;

        // set damage to particle system
        ChangeParticleSystem();


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
        bulletDamage = weaponController.ffDamage;
        bulletMaxCount = weaponController.ffbulletCount;
        realodInterval = weaponController.ffReloadTime;
    }

    public void ChangeParticleSystem()
    {
        if (upgradeChooseList == null)
        {
            upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();
        }

        if (upgradeChooseList.upgrades[58].upgradeIndexInstalled == 1)
        {
            particleSystem_ = particleSystem2;
        }
        else
        {
            particleSystem_ = particleSystem1;
        }
        particleBullet = particleSystem_.GetComponent<ParticleBullet>();
    }

    // shooting controller
    void Shooting()
    {
        if (bulletCount == bulletMaxCount)
        {
            particleBullet.bulletDamage = bulletDamage;
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
                particleSystem_.Emit(1);
                bulletCount++;

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

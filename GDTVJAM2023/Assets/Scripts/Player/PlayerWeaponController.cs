using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Main Weapon")]
    public int bulletBaseDamage;
    public float fireRate;
    public List<ParticleSystem> mainWeapons = new List<ParticleSystem>();
    public List<ParticleBullet> particleBullets;
    public AudioSource mainWeaponSound;


    [Header("Passiv abilitys")]
    public bool isHeadCannon = false;
    public bool isRocketLauncher = false;
    public bool isFireFlies = false;
    public bool isBulletWings = false;
    public bool isLifeModul = false;
    public bool isSpreadGun = false;
    public bool isFrontShield = false;
    public bool isBackShield = false;


    [Header("Objects")]
    public GameObject frontShield;
    public GameObject backShield;


    [Header("Head Cannon")]
    public int hcBulletDamage = 4;
    public int hcSalveCount = 6;
    public float hcReloadTime = 2.5f;
    public GameObject headCannon;


    [Header("Rocket Launcher")]
    public int rlDamage = 20;
    public float rlLifeTime;
    public float rlReloadTime = 5f;
    public GameObject rocketLauncher;


    [Header("Fireflies")]
    public int ffDamage = 10;
    public float ffReloadTime = 5.5f;
    public int ffbulletCount = 16;
    public GameObject fireFlys;


    [Header("Bullet Wings")]
    public int bwDamage = 6;
    public float bwRealoadTime = 6f;
    public int bwSalveCount = 6;
    public GameObject bulletWings;


    [Header("Life Modul")]
    public GameObject lifeModul;


    [Header("Spread Gun")]
    public int sgDamage = 8;
    public float sgReloadTime = 3f;
    public int sgBulletCount = 8;
    public GameObject spreadGun;


    //private Objects
    private PlayerController playerController;




    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        fireRate = playerController.playerFireRate;
        UpdateBulletValues();

        //optional
        WeaponChoose();
    }




    /* **************************************************************************** */
    /* Weapon Management----------------------------------------------------------------- */
    /* **************************************************************************** */
    public void WeaponChoose()
    {
        headCannon.SetActive(isHeadCannon);
        rocketLauncher.SetActive(isRocketLauncher);
        fireFlys.SetActive(isFireFlies);
        bulletWings.SetActive(isBulletWings);
        lifeModul.SetActive(isLifeModul);
        spreadGun.SetActive(isSpreadGun);
        frontShield.SetActive(isFrontShield);
        backShield.SetActive(isBackShield);
    }




    /* **************************************************************************** */
    /* Main weapon----------------------------------------------------------------- */
    /* **************************************************************************** */
    // the main weapon start to fire
    public void StartShooting()
    {
        // set the main weapon particle damage
        UpdateBulletValues();

        // start invoke for main weapons
        InvokeRepeating("ShotEmit", 0.5f, fireRate);
    }

    // stop the main waepon fire
    public void StopShooting()
    {
        CancelInvoke("ShotEmit");
    }

    // invoke function - make the main weapon fire
    void ShotEmit()
    {
        // shooting sound
        mainWeaponSound.Play();

        // emit 1 particle of each mainweapon
        foreach (ParticleSystem mainWeapon in mainWeapons)
        {
            if (mainWeapon != null)
                mainWeapon.Emit(1);
        }
    }

    // set the main weapon particle damage
    public void SetBulletDamage()
    {
        foreach (ParticleBullet particle in particleBullets)
        {
            particle.BulletSetDamage(bulletBaseDamage);
        }
    }

    // update main Weapon base values from playerController
    public void UpdateBulletValues()
    {
        bulletBaseDamage = playerController.playerBulletBaseDamage;

        float temp_fireRate = fireRate;
        fireRate = playerController.playerFireRate;

        // restart Invoke
        if (fireRate != temp_fireRate)
        {
            CancelInvoke("ShotEmit");
            InvokeRepeating("ShotEmit", 0.3f, fireRate);
        }

        SetBulletDamage();
    }

}

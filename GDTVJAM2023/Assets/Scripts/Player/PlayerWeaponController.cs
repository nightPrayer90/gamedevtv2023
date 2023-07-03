using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon buffs")]
    public int bulletCritChance = 5;
    public int bulletCritDamage = 125;
    public int burnDamageChance = 1;
    public int burnDamage = 5;
    public int burnTickCount = 5;
    public float rocketAOERadius = 0;

    //[Header("Weapon classes")]
    [HideInInspector] public int mcBulletLvl = 0;
    [HideInInspector] public int mcExplosionLvl = 0;
    [HideInInspector] public int mcLaserLvl = 0;
    [HideInInspector] public int mcSupportLvl = 0;
    [HideInInspector] public int scSwarmLvl = 0;
    [HideInInspector] public int scDefenceLvl = 0;
    [HideInInspector] public int scTargetingLvl = 0;
    [HideInInspector] public int scBackwardsLvl = 0;


    [Header("Passiv abilitys")]
    public bool isHeadCannon = false;
    public bool isRocketLauncher = false;
    public bool isFireFlies = false;
    public bool isBulletWings = false;
    public bool isLifeModul = false;
    public bool isSpreadGun = false;
    public bool isFrontShield = false;
    public bool isBackShield = false;
    public bool isNovaExplosion = false;
    public bool isRockedWings = false;
    public bool isFrontLaser = false;
    public bool isOrbitalLaser = false;

    private GameObject isHeadCannonInstalled;
    private GameObject isRocketLauncherInstalled;
    private GameObject isFireFliesInstalled;
    private GameObject isBulletWingsInstalled;
    private GameObject isLifeModulInstalled;
    private GameObject isSpreadGunInstalled;
    private GameObject isFrontShieldInstalled;
    private GameObject isBackShieldInstalled;
    private GameObject isNovaExplosionInstalled;
    private GameObject isRockedWingsInstalled;
    private GameObject isFrontLaserInstalled;
    private GameObject isOrbitalLaserInstalled;


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


    [Header("Nova Explosion")]
    public int neDamage = 3;
    public float neReloadTime = 4.5f;
    public float neRadius = 3f;
    public GameObject novaExplosion;


    [Header("Rocket Wings")]
    public int rwDamage = 3;
    public float rwReloadTime = 4.5f;
    public int rwSalveCount = 6;
    public GameObject rocketWings;


    [Header("Front Laser")]
    public int flDamage = 4;
    public float flReloadTime = 6f;
    public int flBulletCount = 50;
    public GameObject frontLaser;


    [Header("Orbital Laser")]
    public int olDamage = 10;
    public float olReloadTime = 3f;
    public GameObject orbitalLaser;


    [Header("Container")]
    public Transform passivParentContainer;


    //private Objects
    private PlayerController playerController;
    private PlayerMWController playerMWController;
    



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    void Start()
    {
        // testing
        WeaponChoose();

        playerMWController = gameObject.GetComponent<PlayerMWController>();

        
        switch (playerMWController.weaponType)
        {
            case PlayerMWController.MWeapontyp.bullet: 
                mcBulletLvl++;
                break;
            case PlayerMWController.MWeapontyp.rocket: 
                mcExplosionLvl++;
                break;
            case PlayerMWController.MWeapontyp.laser:
                mcLaserLvl++;
                break;
        }
    }




    /* **************************************************************************** */
    /* Weapon Management----------------------------------------------------------------- */
    /* **************************************************************************** */
    public void UpdateWeaponValues()
    {
        // main Class
        bulletCritChance = 5 + (mcBulletLvl * 5);
        burnDamageChance = 1 + (mcLaserLvl * 3);
        rocketAOERadius = (mcExplosionLvl * 0.5f);
        float reloadTime = 1 + (mcSupportLvl * 0.03f);

        // sub class
        int scSwarmLvl_ = scSwarmLvl * 2;
        float scDefenceLvl_ = scDefenceLvl * 0.05f;
        int scTargetingLvl_ = scTargetingLvl;
        int scBackwardsLvl_ = scBackwardsLvl;
        /*
        [HideInInspector] public int scTargetingLvl = 0;
        [HideInInspector] public int scBackwardsLvl = 0;
        */

        // Head Cannon - bullet - target
        if (isHeadCannonInstalled != null)
        {
            var headCannon = isHeadCannonInstalled.GetComponent<HeadCannon>();
            headCannon.bulletDamage = hcBulletDamage + scTargetingLvl; 
            headCannon.fireSalveMax = hcSalveCount;
            headCannon.reloadSalveInterval = hcReloadTime;
        }

        // Rocket Launcher - rocket - target
        if (isRocketLauncherInstalled != null)
        {
            var rocketLauncher = isRocketLauncherInstalled.GetComponent<PeriodSpawner>();
            rocketLauncher.rocketDamage = rlDamage + scTargetingLvl;
            rocketLauncher.lifeTime = rlLifeTime;
            rocketLauncher.spawnInterval = rlReloadTime;
        }

        // Fireflies - bullet - backwards
        if (isFireFliesInstalled != null)
        {
            var fireflies = isFireFliesInstalled.GetComponent<Fireflies>();
            fireflies.bulletDamage = ffDamage + scBackwardsLvl;
            fireflies.realodInterval = ffReloadTime;
            fireflies.bulletMaxCount = ffbulletCount;
        } 

        /*

        // Bullet Wings
        bwDamage = 6;
        bwRealoadTime = 6f;
        bwSalveCount = 6;


        // Life Modul


        // Spread Gun
        sgDamage = 8;
        sgReloadTime = 3f;
        sgBulletCount = 8;


        // Nova Explosion
        neDamage = 3;
        neReloadTime = 4.5f;
        neRadius = 3f;


        // Rocket Wings
        rwDamage = 3;
        rwReloadTime = 4.5f;
        rwSalveCount = 6;


        // Front Laser
        flDamage = 4;
        flReloadTime = 6f;
        flBulletCount = 50;


        // Orbital Laser
        olDamage = 10;
        olReloadTime = 3f;
        */
}


    public void WeaponChoose()
    {
        if (isHeadCannon == true && isHeadCannonInstalled == null)
        {
            isHeadCannonInstalled = Instantiate(headCannon, passivParentContainer);
        }
        if (isRocketLauncher == true && isRocketLauncherInstalled == null)
        {
            isRocketLauncherInstalled = Instantiate(rocketLauncher, passivParentContainer);
        }
        if (isFireFlies == true && isFireFliesInstalled == null)
        {
            isFireFliesInstalled = Instantiate(fireFlys, passivParentContainer);
        }
        if (isBulletWings == true && isBulletWingsInstalled == null)
        {
            isBulletWingsInstalled = Instantiate(bulletWings, passivParentContainer);
        }
        if (isLifeModul == true && isLifeModulInstalled == null)
        {
            isLifeModulInstalled = Instantiate(lifeModul, passivParentContainer);
        }
        if (isSpreadGun == true && isSpreadGunInstalled == null)
        {
            isSpreadGunInstalled = Instantiate(spreadGun, passivParentContainer);
        }
        if (isFrontShield == true && isFrontShieldInstalled == null)
        {
            var shild = Instantiate(frontShield, passivParentContainer);
            shild.name = frontShield.name;
            isFrontShieldInstalled = shild;
        }
        if (isBackShield == true && isBackShieldInstalled == null)
        {
            var shild = Instantiate(backShield, passivParentContainer);
            shild.name = backShield.name;
            isBackShieldInstalled = shild;
        }
        if (isNovaExplosion == true && isNovaExplosionInstalled == null)
        {
            isNovaExplosionInstalled = Instantiate(novaExplosion, passivParentContainer);
        }
        if (isRockedWings == true && isRockedWingsInstalled == null)
        {
            isRockedWingsInstalled = Instantiate(rocketWings, passivParentContainer);
        }
        if (isFrontLaser == true && isFrontLaserInstalled == null)
        {
            isFrontLaserInstalled = Instantiate(frontLaser, passivParentContainer);
        }
        if (isOrbitalLaser == true && isOrbitalLaserInstalled == null)
        {
            isOrbitalLaserInstalled = Instantiate(orbitalLaser);
        }

        UpdateWeaponValues();
    }
}

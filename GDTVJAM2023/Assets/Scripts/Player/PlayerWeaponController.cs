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

    private HeadCannon isHeadCannonInstalled;
    private PeriodSpawner isRocketLauncherInstalled;
    private Fireflies isFireFliesInstalled;
    private BulletWings isBulletWingsInstalled;
    private LifeModul isLifeModulInstalled;
    private SpreadGun isSpreadGunInstalled;
    private ShieldSpawner isFrontShieldInstalled;
    private ShieldSpawner isBackShieldInstalled;
    private NovaExplosion isNovaExplosionInstalled;
    private RocketWings isRockedWingsInstalled;
    private FrontLaser isFrontLaserInstalled;
    private OrbitalLaser isOrbitalLaserInstalled;


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
        // Start Values
        WeaponChoose();

        playerMWController = gameObject.GetComponent<PlayerMWController>();

        // Upadate Start - Class
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

        // upgrade Weapon Values
        UpdateWeaponValues();
    }




    /* **************************************************************************** */
    /* Weapon Management----------------------------------------------------------------- */
    /* **************************************************************************** */
    public void UpdateWeaponValues()
    {
        // main Class
        bulletCritChance = 5 + (mcBulletLvl * 5);
        burnDamageChance = (mcLaserLvl);
        rocketAOERadius = 1 +(mcExplosionLvl * 0.05f);
        float reloadTime = 1 + (mcSupportLvl * 0.03f);

        // sub class
        int scSwarmLvl_ = scSwarmLvl;
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
            isHeadCannonInstalled.bulletDamage = hcBulletDamage + scTargetingLvl;
            isHeadCannonInstalled.fireSalveMax = hcSalveCount;
            isHeadCannonInstalled.reloadSalveInterval = hcReloadTime;
        }

        // Rocket Launcher - rocket - target
        if (isRocketLauncherInstalled != null)
        {
            isRocketLauncherInstalled.rocketDamage = rlDamage + scTargetingLvl;
            isRocketLauncherInstalled.lifeTime = rlLifeTime;
            isRocketLauncherInstalled.spawnInterval = rlReloadTime;
        }

        // Fireflies - bullet - backwards
        if (isFireFliesInstalled != null)
        {
            isFireFliesInstalled.bulletDamage = ffDamage + scBackwardsLvl;
            isFireFliesInstalled.realodInterval = ffReloadTime;
            isFireFliesInstalled.bulletMaxCount = ffbulletCount;
        }

        // Bullet Wings - bullet - swarm
        if (isBulletWingsInstalled != null)
        {
            isBulletWingsInstalled.bulletDamage = bwDamage;
            isBulletWingsInstalled.realodInterval = bwRealoadTime;
            isBulletWingsInstalled.salveMaxCount = bwSalveCount;
        }

        // Life Modul - support
        if (isLifeModulInstalled != null)
        {
            //isLifeModulInstalled
        }

        // Spread Gun - bullet - swarm
        if (isSpreadGunInstalled != null)
        {
            isSpreadGunInstalled.bulletDamage = sgDamage;
            isSpreadGunInstalled.realodInterval = sgReloadTime;
            isSpreadGunInstalled.bulletMaxCount = sgBulletCount;
        }

        // Front Shield - support
        if (isFrontShieldInstalled != null)
        {
            //isFrontShieldInstalled
        }

        // Back Shield - support
        if (isBackShieldInstalled != null)
        {
            //isBackShieldInstalled
        }

        // Nova Explosion - explosion - defence
        if (isNovaExplosionInstalled != null)
        {
            isNovaExplosionInstalled.novaDamage = neDamage;
            isNovaExplosionInstalled.spawnInterval = neReloadTime;
            isNovaExplosionInstalled.explosionRadius = neRadius;
        }

        // Rocket Wings - explosion - swarm
        if (isRockedWingsInstalled != null)
        {
            isRockedWingsInstalled.rocketDamage = rwDamage;
            isRockedWingsInstalled.relodInterval = rwReloadTime;
            isRockedWingsInstalled.salveMaxCount = rwSalveCount;
        }

        // Front Laser - laser
        if (isFrontLaserInstalled != null)
        {
            isFrontLaserInstalled.bulletDamage = flDamage;
            isFrontLaserInstalled.realodInterval = flReloadTime;
            isFrontLaserInstalled.bulletMaxCount = flBulletCount;
        }

        // Orbital Laser - laser - defence
        if (isOrbitalLaserInstalled != null)
        {
            isOrbitalLaserInstalled.damage = olDamage;
            isOrbitalLaserInstalled.realoadTime = olReloadTime;

            isOrbitalLaserInstalled.UpdateOrbs();
        }
    }


    public void WeaponChoose()
    {
        GameObject go = null;
        if (isHeadCannon == true && isHeadCannonInstalled == null)
        {
            go = Instantiate(headCannon, passivParentContainer);
            isHeadCannonInstalled = go.GetComponent<HeadCannon>();
        }
        if (isRocketLauncher == true && isRocketLauncherInstalled == null)
        {
            go = Instantiate(rocketLauncher, passivParentContainer);
            isRocketLauncherInstalled = go.GetComponent<PeriodSpawner>();
        }
        if (isFireFlies == true && isFireFliesInstalled == null)
        {
            go = Instantiate(fireFlys, passivParentContainer);
            isFireFliesInstalled = go.GetComponent<Fireflies>();
        }
        if (isBulletWings == true && isBulletWingsInstalled == null)
        { 
            go = Instantiate(bulletWings, passivParentContainer);
            isBulletWingsInstalled = go.GetComponent<BulletWings>();
        }
        if (isLifeModul == true && isLifeModulInstalled == null)
        {
            go = Instantiate(lifeModul, passivParentContainer);
            isLifeModulInstalled = go.GetComponent<LifeModul>();
        }
        if (isSpreadGun == true && isSpreadGunInstalled == null)
        {
            go = Instantiate(spreadGun, passivParentContainer);
            isSpreadGunInstalled = go.GetComponent<SpreadGun>();
        }
        if (isFrontShield == true && isFrontShieldInstalled == null)
        {
            var shild = Instantiate(frontShield, passivParentContainer);
            shild.name = frontShield.name;
            isFrontShieldInstalled = shild.GetComponent<ShieldSpawner>();
        }
        if (isBackShield == true && isBackShieldInstalled == null)
        {
            var shild = Instantiate(backShield, passivParentContainer);
            shild.name = backShield.name;
            isBackShieldInstalled = shild.GetComponent<ShieldSpawner>();
        }
        if (isNovaExplosion == true && isNovaExplosionInstalled == null)
        {
            go = Instantiate(novaExplosion, passivParentContainer);
            isNovaExplosionInstalled = go.GetComponent<NovaExplosion>();
        }
        if (isRockedWings == true && isRockedWingsInstalled == null)
        {
            go = Instantiate(rocketWings, passivParentContainer);
            isRockedWingsInstalled = go.GetComponent<RocketWings>();
        }
        if (isFrontLaser == true && isFrontLaserInstalled == null)
        {
            go = Instantiate(frontLaser, passivParentContainer);
            isFrontLaserInstalled = go.GetComponent<FrontLaser>();
        }
        if (isOrbitalLaser == true && isOrbitalLaserInstalled == null)
        {
            go = Instantiate(orbitalLaser);
            isOrbitalLaserInstalled = go.GetComponent<OrbitalLaser>();
        }

        UpdateWeaponValues();
    }
}

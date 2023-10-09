using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon buffs")]
    private ShipData shipData;
    public int bulletCritChance = 0;
    public int bulletCritDamage = 0;
    public int burnDamageChance = 0;
    public float rocketAOERadius = 0;


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
    private FrontShieldSpawner isFrontShieldInstalled;
    private BackShieldSpawner isBackShieldInstalled;
    private NovaExplosion isNovaExplosionInstalled;
    private RocketWings isRockedWingsInstalled;
    private FrontLaser isFrontLaserInstalled;
    private OrbitalLaser isOrbitalLaserInstalled;



    [Header("Objects")]
    public GameObject frontShield;
    public GameObject backShield;
    [HideInInspector] public GameObject frontShield_;
    [HideInInspector] public GameObject backShieldLeft_;
    [HideInInspector] public GameObject backShieldRight_;


    [Header("Head Cannon")]
    public int hcBulletDamage = 4;
    public int hcSalveCount = 6;
    public float hcReloadTime = 2.5f;
    public GameObject headCannon;
    [HideInInspector] public int hcBulletDamage_;
    [HideInInspector] public int hcSalveCount_;
    [HideInInspector] public float hcReloadTime_;


    [Header("Rocket Launcher")]
    public int rlDamage = 20;
    public float rlLifeTime;
    public float rlReloadTime = 5f;
    public GameObject rocketLauncher;
    [HideInInspector] public int rlDamage_;
    [HideInInspector] public float rlLifeTime_;
    [HideInInspector] public float rlReloadTime_;

    [Header("Fireflies")]
    public int ffDamage = 10;
    public float ffReloadTime = 5.5f;
    public int ffbulletCount = 16;
    public GameObject fireFlys;
    [HideInInspector] public int ffDamage_;
    [HideInInspector] public float ffReloadTime_;
    [HideInInspector] public int ffbulletCount_;


    [Header("Bullet Wings")]
    public int bwDamage = 6;
    public float bwRealoadTime = 6f;
    public int bwSalveCount = 6;
    public GameObject bulletWings;
    [HideInInspector] public int bwDamage_;
    [HideInInspector] public float bwRealoadTime_;
    [HideInInspector] public int bwSalveCount_;

    [Header("Life Modul")]
    public int lmLifePerTick = 1;
    public float lmReloadTime = 10;
    public GameObject lifeModul;
    [HideInInspector] public int lmLifePerTick_;
    [HideInInspector] public float lmReloadTime_;


    [Header("Spread Gun")]
    public int sgDamage = 8;
    public float sgReloadTime = 3f;
    public int sgBulletCount = 8;
    public GameObject spreadGun;
    [HideInInspector] public int sgDamage_;
    [HideInInspector] public float sgReloadTime_;
    [HideInInspector] public int sgBulletCount_;

    [Header("Nova Explosion")]
    public int neDamage = 3;
    public float neReloadTime = 4.5f;
    public float neRadius = 3f;
    public GameObject novaExplosion;
    [HideInInspector] public int neDamage_;
    [HideInInspector] public float neReloadTime_;
    [HideInInspector] public float neRadius_;


    [Header("Rocket Wings")]
    public int rwDamage = 3;
    public float rwReloadTime = 4.5f;
    public int rwSalveCount = 6;
    public GameObject rocketWings;
    [HideInInspector] public int rwDamage_;
    [HideInInspector] public float rwReloadTime_;
    [HideInInspector] public int rwSalveCount_;


    [Header("Front Laser")]
    public int flDamage = 4;
    public float flReloadTime = 6f;
    public float flShootingTime = 3f;
    public GameObject frontLaser;
    [HideInInspector] public int flDamage_;
    [HideInInspector] public float flReloadTime_;
    [HideInInspector] public float flShootingTime_;

    [Header("Orbital Laser")]
    public int olDamage = 10;
    public float olReloadTime = 3f;
    public GameObject orbitalLaser;
    [HideInInspector] public int olDamage_;
    [HideInInspector] public float olReloadTime_;


    [Header("Container")]
    public Transform passivParentContainer;


    [Header("Shield Controll")]
    public float fsSpawnTime = 10f;
    public int fsShieldLife = 3;
    public float bsSpawnTime = 6f;
    public int bsShildLife = 2; 
    public bool isFrontShieldEnabled = false;
    public bool isBackShieldLeft = false;
    public bool isBackShieldRight = false;
    [HideInInspector] public float fsSpawnTime_;
    [HideInInspector] public int fsShieldLife_;
    [HideInInspector] public float bsSpawnTime_;
    [HideInInspector] public int bsShildLife_;


    //private Objects
    private PlayerController playerController;
    private PlayerMWController playerMWController;
    private UpgradeChooseList upgradeChooseList;



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle
    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        playerMWController = gameObject.GetComponent<PlayerMWController>();
        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();


        // copy ship Data
        shipData = playerController.shipData;


        // set upgradeListData from shipData
        upgradeChooseList.mcBulletLvl = shipData.bulletClass;
        upgradeChooseList.mcExplosionLvl = shipData.explosionClass;
        upgradeChooseList.mcLaserLvl = shipData.laserClass;
        upgradeChooseList.mcSupportLvl = shipData.supportClass;

        upgradeChooseList.baseBulletCritChance += upgradeChooseList.critChance * shipData.bulletClass + shipData.pecCritChance;
        upgradeChooseList.baseBulletCritDamage += upgradeChooseList.critDamage * shipData.bulletClass + shipData.pecBulletCritDamage;
        upgradeChooseList.baseRocketAOERadius += upgradeChooseList.aoeRange * shipData.explosionClass + shipData.pecAOE;
        upgradeChooseList.baseLaserBurnDamageChance += upgradeChooseList.burningChance * shipData.laserClass + shipData.pecBurningChance;
        upgradeChooseList.baseSupportRealoadTime += upgradeChooseList.supportRealodTime * shipData.supportClass;

        upgradeChooseList.percBulletDamage = shipData.pecBulletDamage;
        upgradeChooseList.percRocketDamage = shipData.pecExplosionDamage;
        upgradeChooseList.percLaserDamage = shipData.pecLaserDamage;

        upgradeChooseList.rocketLifeTime = shipData.rocketLifeTime;
        upgradeChooseList.baseLaserTickDamage = shipData.burningDamagePerTick;

        upgradeChooseList.bossBonusDamage = shipData.pecMainWeaponDamageBoss;

        // Start Values
        WeaponChoose();
    }
    #endregion


    /* **************************************************************************** */
    /* WEAPON MANAGEMENT----------------------------------------------------------- */
    /* **************************************************************************** */
    #region weapon management
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
            isFrontShieldInstalled = shild.GetComponent<FrontShieldSpawner>();
        }
        if (isBackShield == true && isBackShieldInstalled == null)
        {
            var shild = Instantiate(backShield, passivParentContainer);
            shild.name = backShield.name;
            isBackShieldInstalled = shild.GetComponent<BackShieldSpawner>();
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

        Invoke("UpdateWeaponValues", 0.1f);
    }


    public void UpdateWeaponValues()
    {
        //Debug.Log("weapon Upgrade");
        // main weapon - value in playercontroller = constant -> update only mainWeapon controller
        if (playerMWController.weaponType == PlayerMWController.MWeapontyp.bullet)  
        {
            playerMWController.bulletBaseDamage = Mathf.CeilToInt((playerController.playerBulletBaseDamage) * (1 + upgradeChooseList.percBulletDamage / 100));
            playerMWController.SetBulletDamage();
            //Debug.Log("bulletUpgrade");
            //Debug.Log(playerMWController.bulletBaseDamage);
        }
        else if (playerMWController.weaponType == PlayerMWController.MWeapontyp.rocket)
        {
            playerMWController.bulletBaseDamage = Mathf.CeilToInt((playerController.playerBulletBaseDamage) * (1 + upgradeChooseList.percRocketDamage / 100));
        }
        else if (playerMWController.weaponType == PlayerMWController.MWeapontyp.laser)
        {
            playerMWController.bulletBaseDamage = Mathf.CeilToInt((playerController.playerBulletBaseDamage) * (1 + upgradeChooseList.percLaserDamage / 100));
        }

        // main Class
        bulletCritChance = upgradeChooseList.baseBulletCritChance;
        bulletCritDamage = upgradeChooseList.baseBulletCritDamage;

        burnDamageChance = upgradeChooseList.baseLaserBurnDamageChance;
        rocketAOERadius = 1 + upgradeChooseList.baseRocketAOERadius * 0.01f;
        float suReloadTime = 1 - (upgradeChooseList.baseSupportRealoadTime * 0.01f);

        // sub class
        int scSwarmLvl_ = 0; //upgradeChooseList.scSwarmLvl;
        //float scDefenceLvl_ = 0; // upgradeChooseList.scDefenceLvl * 0.1f;
        int scTargetingLvl_ = 0; //upgradeChooseList.scTargetingLvl;
        int scDirectionLvl_ = 0; // upgradeChooseList.scDirectionLvl;



        // Head Cannon - bullet - target
        hcBulletDamage_ = Mathf.CeilToInt((hcBulletDamage + scTargetingLvl_) * (1 + upgradeChooseList.percBulletDamage / 100));

        //Debug.Log(hcBulletDamage_ + " upgradeChooseList.percBulletDamage " + upgradeChooseList.baseBulletCritDamage + "-" + Mathf.CeilToInt(((float)upgradeChooseList.baseBulletCritDamage/100)* hcBulletDamage_));

        hcSalveCount_ = hcSalveCount;
        hcReloadTime_ = Mathf.Max(0.1f, (hcReloadTime * suReloadTime));
        if (isHeadCannonInstalled != null)
        {
            isHeadCannonInstalled.bulletDamage = hcBulletDamage_;
            isHeadCannonInstalled.fireSalveMax = hcSalveCount_;
            isHeadCannonInstalled.reloadSalveInterval = hcReloadTime_;
        }


        // Rocket Launcher - rocket - target
        rlDamage_ = Mathf.CeilToInt((rlDamage + scTargetingLvl_) * (1 + upgradeChooseList.percRocketDamage / 100));
        rlLifeTime_ = rlLifeTime;
        rlReloadTime_ = Mathf.Max(0.1f, (rlReloadTime * suReloadTime));
        if (isRocketLauncherInstalled != null)
        {
            isRocketLauncherInstalled.rocketDamage = rlDamage_;
            isRocketLauncherInstalled.lifeTime = rlLifeTime_;
            isRocketLauncherInstalled.spawnInterval = rlReloadTime_;
        }


        // Fireflies - bullet - backwards
        ffDamage_ = Mathf.CeilToInt((ffDamage + scDirectionLvl_) * (1 + upgradeChooseList.percBulletDamage / 100));
        ffReloadTime_ = Mathf.Max(0.1f, (ffReloadTime * suReloadTime));
        ffbulletCount_ = ffbulletCount;
        if (isFireFliesInstalled != null)
        {
            isFireFliesInstalled.bulletDamage = ffDamage_;
            isFireFliesInstalled.realodInterval = ffReloadTime_;
            isFireFliesInstalled.bulletMaxCount = ffbulletCount_;
        }


        // Bullet Wings - bullet - swarm
        bwDamage_ = Mathf.CeilToInt((bwDamage) * (1 + upgradeChooseList.percBulletDamage / 100));
        bwRealoadTime_ = Mathf.Max(0.1f, (bwRealoadTime * suReloadTime));
        bwSalveCount_ = bwSalveCount + scSwarmLvl_;
        if (isBulletWingsInstalled != null)
        {
            isBulletWingsInstalled.bulletDamage = bwDamage_;
            isBulletWingsInstalled.realodInterval = bwRealoadTime_;
            isBulletWingsInstalled.salveMaxCount = bwSalveCount_;
        }


        // Life Modul - support
        lmReloadTime_ = Mathf.Max(0.1f, (lmReloadTime * suReloadTime));
        lmLifePerTick_ = lmLifePerTick;
        if (isLifeModulInstalled != null)
        {
            isLifeModulInstalled.nextHealTick = lmReloadTime_;
            isLifeModulInstalled.healthPerTick = lmLifePerTick_;
        }


        // Spread Gun - bullet - swarm
        sgDamage_ = Mathf.CeilToInt((sgDamage) * (1 + upgradeChooseList.percBulletDamage / 100));
        sgReloadTime_ = Mathf.Max(0.1f, (sgReloadTime * suReloadTime));
        sgBulletCount_ = sgBulletCount + scSwarmLvl_;
        if (isSpreadGunInstalled != null)
        {
            isSpreadGunInstalled.bulletDamage = sgDamage_;
            isSpreadGunInstalled.realodInterval = sgReloadTime_;
            isSpreadGunInstalled.bulletMaxCount = sgBulletCount_;
        }

        // Front Shield - support - defence
        fsSpawnTime_ = Mathf.Max(0.1f, (fsSpawnTime * suReloadTime));
        fsShieldLife_ = fsShieldLife + upgradeChooseList.shieldHealth;
        if (isFrontShieldInstalled != null)
        {
            isFrontShieldInstalled.spawnInterval = fsSpawnTime_;
            fsShieldLife = fsShieldLife_;
        }


        // Back Shield - support - defence
        bsSpawnTime_ = Mathf.Max(0.1f, (bsSpawnTime * suReloadTime));
        bsShildLife_ = bsShildLife + upgradeChooseList.shieldHealth;
        if (isBackShieldInstalled != null)
        {
            isBackShieldInstalled.spawnInterval = bsSpawnTime_;
            bsShildLife = bsShildLife_;
        }


        // Nova Explosion - explosion - defence
        neDamage_ = Mathf.CeilToInt((neDamage) * (1 + upgradeChooseList.percRocketDamage / 100));
        neReloadTime_ = Mathf.Max(0.1f, (neReloadTime * (suReloadTime)));
        neRadius_ = neRadius * rocketAOERadius;
        if (isNovaExplosionInstalled != null)
        {
            isNovaExplosionInstalled.novaDamage = neDamage_;
            isNovaExplosionInstalled.spawnInterval = neReloadTime_;
            isNovaExplosionInstalled.explosionRadius = neRadius_;
        }


        // Rocket Wings - explosion - swarm
        rwDamage_ = Mathf.CeilToInt((rwDamage) * (1 + upgradeChooseList.percRocketDamage / 100));
        rwReloadTime_ = Mathf.Max(0.1f, (rwReloadTime * suReloadTime));
        rwSalveCount_ = rwSalveCount + scSwarmLvl_;
        if (isRockedWingsInstalled != null)
        {
            isRockedWingsInstalled.rocketDamage = rwDamage_;
            isRockedWingsInstalled.relodInterval = rwReloadTime_;
            isRockedWingsInstalled.salveMaxCount = rwSalveCount_;
        }

        // Front Laser - laser
        flDamage_ = Mathf.CeilToInt((flDamage) * (1 + upgradeChooseList.percLaserDamage / 100));
        flReloadTime_ = Mathf.Max(1f, (flReloadTime * suReloadTime));
        flShootingTime_ = flShootingTime;
        if (isFrontLaserInstalled != null)
        {
            isFrontLaserInstalled.bulletDamage = flDamage_;
            isFrontLaserInstalled.realodInterval = flReloadTime_;
            isFrontLaserInstalled.laserShootTime = flShootingTime_;
        }

        // Orbital Laser - laser - defence
        olDamage_ = Mathf.CeilToInt((olDamage) * (1 + upgradeChooseList.percLaserDamage / 100));
        olReloadTime_ = Mathf.Max(0.1f, (olReloadTime * (suReloadTime)));
        if (isOrbitalLaserInstalled != null)
        {
            isOrbitalLaserInstalled.damage = olDamage_;
            isOrbitalLaserInstalled.realoadTime = olReloadTime_;

            isOrbitalLaserInstalled.UpdateOrbs();
        }
    }
    #endregion

}

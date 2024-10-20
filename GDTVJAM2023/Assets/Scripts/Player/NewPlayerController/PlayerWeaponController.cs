using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Ship Data")]
    public ShipData shipData;
    public UpgradePanelController upgradePanelController;

    [Header("Passiv abilitys")]
    public bool isHeadCannon = false;
    public bool isRocketLauncher = false;
    public bool isFireFlies = false;
    public bool isBulletWings = false;
    public bool isLifeModul = false;
    public bool isSpreadGun = false;
    public bool isNovaExplosion = false;
    public bool isRockedWings = false;
    public bool isFrontLaser = false;
    public bool isOrbitalLaser = false;
    public bool isThermalSpheres = false;
    public bool isMineLayer = false;
    public bool isBackfireBeam = false;
    public bool isSidegun = false;

    private HeadCannon isHeadCannonInstalled;
    private PeriodSpawner isRocketLauncherInstalled;
    private Fireflies isFireFliesInstalled;
    private BulletWings isBulletWingsInstalled;
    private LifeModul isLifeModulInstalled;
    private SpreadGun isSpreadGunInstalled;
    private NovaExplosion isNovaExplosionInstalled;
    private RocketWings isRockedWingsInstalled;
    private FrontLaser isFrontLaserInstalled;
    private OrbitalLaser isOrbitalLaserInstalled;
    private ThermalSpheres isThermalSpheresInstalled;
    private MineLayer isMineLayerInstalled;
    private BackfireBeam isBackfireBeamInstalled;
    private SideGun isSideGunInstalled;


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
    public float rlAOERange = 1.1f;
    public float rlReloadTime = 5f;
    public GameObject rocketLauncher;
    [HideInInspector] public int rlDamage_;
    [HideInInspector] public float rlAOERange_;
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
    public int olCount = 3;
    public float olRotationSpeed = 300;
    public GameObject orbitalLaser;
    [HideInInspector] public int olDamage_;
    [HideInInspector] public float olReloadTime_;

    [Header("Thermal Sphere")]
    public int tsDamage = 4;
    public float tsLifetime = 3;
    public float tsReloadTime = 2.5f;
    public GameObject thermalSphere;
    [HideInInspector] public int tsDamage_;
    [HideInInspector] public float tsLifetime_;
    [HideInInspector] public float tsRelaodTime_;

    [Header("Mine Layer")]
    public int mlDamage = 6;
    public float mlLifetime = 12;
    public float mlReloadTime = 3f;
    public GameObject mineLayer;
    [HideInInspector] public int mlDamage_;
    [HideInInspector] public float mlLifetime_;
    [HideInInspector] public float mlRelaodTime_;

    [Header("Backfire Beam")]
    public int bbDamage = 10;
    public float bbReloadTime = 3f;
    public int bbMainBeams = 1;
    public int bbKillBeams = 2;
    public GameObject backfireBeam;
    [HideInInspector] public int bbDamage_;
    [HideInInspector] public float bbReloadTime_;


    [Header("Container")]
    public Transform passivParentContainer;


    [Header("Shield Control")]
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


    [Header("Side Gun")]
    public int sdgDamage = 8;
    public float sdgReloadTime = 3f;
    public int sdgBulletsPerShoot = 8;
    public GameObject sideGun;
    [HideInInspector] public int sdgDamage_;
    [HideInInspector] public float sdgReloadTime_;
    [HideInInspector] public int sdgBulletsPerShoot_;


    // Events
    public event Action<int> OnMWDamage;
    public event Action<float> ONUpdateLaserReloadTime;
    public event Action<float> ONUpdateBulletRealodTime;

    // private Objects
    //private NewPlayerController playerController;
    private UpgradeChooseList upgradeChooseList;
    private bool canPlayBulletShootSfx = true;


    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle
    void Start()
    {
        //playerController = gameObject.GetComponent<NewPlayerController>();
        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();

        // Start Values - Update Weaponcontroller after loading all modules
        Invoke(nameof(UpdatePlayerWeaponController), 1f);
    }


    private void OnDestroy()
    {
        ResetShipData();
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
        if (isSpreadGun == true && isSpreadGunInstalled == null)
        {
            go = Instantiate(spreadGun, passivParentContainer);
            isSpreadGunInstalled = go.GetComponent<SpreadGun>();
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
        if (isThermalSpheres == true && isThermalSpheresInstalled == null)
        {
            go = Instantiate(thermalSphere, passivParentContainer);
            isThermalSpheresInstalled = go.GetComponent<ThermalSpheres>();
        }
        if (isMineLayer == true && isMineLayerInstalled == null)
        {
            go = Instantiate(mineLayer, passivParentContainer);
            isMineLayerInstalled = go.GetComponent<MineLayer>();
        }
        if (isBackfireBeam == true && isBackfireBeamInstalled == null)
        {
            go = Instantiate(backfireBeam, passivParentContainer);
            isBackfireBeamInstalled = go.GetComponent<BackfireBeam>();
        }
        if (isSidegun == true && isSideGunInstalled == null)
        {
            go = Instantiate(sideGun, passivParentContainer);
            isSideGunInstalled = go.GetComponent<SideGun>();
        }

        Invoke("UpdateWeaponValues", 0.1f);
    }


    public void UpdateWeaponValues()
    {
        float supportReloadTime_ = 1 - (shipData.supportReloadTime / 100);

        // Head Cannon - bullet - target
        hcBulletDamage_ = Mathf.CeilToInt((hcBulletDamage) * (1 + shipData.percBulletDamage / 100));

        //Debug.Log(hcBulletDamage_ + " upgradeChooseList.percBulletDamage " + upgradeChooseList.baseBulletCritDamage + "-" + Mathf.CeilToInt(((float)upgradeChooseList.baseBulletCritDamage/100)* hcBulletDamage_));

        hcSalveCount_ = hcSalveCount;
        hcReloadTime_ = Mathf.Max(0.1f, hcReloadTime * supportReloadTime_);
        if (isHeadCannonInstalled != null)
        {
            isHeadCannonInstalled.bulletDamage = hcBulletDamage_;
            isHeadCannonInstalled.fireSalveMax = hcSalveCount_;
            isHeadCannonInstalled.reloadSalveInterval = hcReloadTime_;
        }

        // Rocket Launcher - rocket - target
        rlDamage_ = Mathf.CeilToInt((rlDamage) * (1 + shipData.percRocketDamage / 100));
        rlAOERange_ = rlAOERange;
        rlReloadTime_ = Mathf.Max(0.1f, rlReloadTime * supportReloadTime_);
        if (isRocketLauncherInstalled != null)
        {
            isRocketLauncherInstalled.rocketDamage = rlDamage_;
            isRocketLauncherInstalled.explosionRadius = rlAOERange_;
            isRocketLauncherInstalled.spawnInterval = rlReloadTime_;
        }

        // Fireflies - bullet - backwards
        ffDamage_ = Mathf.CeilToInt((ffDamage) * (1 + shipData.percBulletDamage / 100));
        ffReloadTime_ = Mathf.Max(0.1f, (ffReloadTime * supportReloadTime_));
        ffbulletCount_ = ffbulletCount;
        if (isFireFliesInstalled != null)
        {
            isFireFliesInstalled.bulletDamage = ffDamage_;
            isFireFliesInstalled.realodInterval = ffReloadTime_;
            isFireFliesInstalled.bulletMaxCount = ffbulletCount_;
            isFireFliesInstalled.ChangeParticleSystem();
        }

        // Bullet Wings - bullet - swarm
        bwDamage_ = Mathf.CeilToInt((bwDamage) * (1 + shipData.percBulletDamage / 100));
        bwRealoadTime_ = Mathf.Max(0.1f, (bwRealoadTime * supportReloadTime_));
        bwSalveCount_ = bwSalveCount;
        if (isBulletWingsInstalled != null)
        {
            isBulletWingsInstalled.bulletDamage = bwDamage_;
            isBulletWingsInstalled.realodInterval = bwRealoadTime_;
            isBulletWingsInstalled.salveMaxCount = bwSalveCount_;
        }

        // Spread Gun - bullet - swarm
        sgDamage_ = Mathf.CeilToInt((sgDamage) * (1 + shipData.percBulletDamage / 100));
        sgReloadTime_ = Mathf.Max(0.1f, (sgReloadTime * supportReloadTime_));
        sgBulletCount_ = sgBulletCount;
        if (isSpreadGunInstalled != null)
        {
            isSpreadGunInstalled.bulletDamage = sgDamage_;
            isSpreadGunInstalled.realodInterval = sgReloadTime_;
            isSpreadGunInstalled.bulletMaxCount = sgBulletCount_;
        }

        // Nova Explosion - explosion - defence
        neDamage_ = Mathf.CeilToInt((neDamage) * (1 + shipData.percRocketDamage / 100));
        neReloadTime_ = Mathf.Max(0.1f, (neReloadTime * supportReloadTime_));
        neRadius_ = neRadius * (1 + shipData.rocketAOERadius / 100);
        if (isNovaExplosionInstalled != null)
        {
            isNovaExplosionInstalled.novaDamage = neDamage_;
            isNovaExplosionInstalled.spawnInterval = neReloadTime_;
            isNovaExplosionInstalled.explosionRadius = neRadius_;
        }


        // Rocket Wings - explosion - swarm
        rwDamage_ = Mathf.CeilToInt((rwDamage) * (1 + shipData.percRocketDamage / 100));
        rwReloadTime_ = Mathf.Max(0.1f, (rwReloadTime * supportReloadTime_));
        rwSalveCount_ = rwSalveCount;
        if (isRockedWingsInstalled != null)
        {
            isRockedWingsInstalled.rocketDamage = rwDamage_;
            isRockedWingsInstalled.relodInterval = rwReloadTime_;
            isRockedWingsInstalled.salveMaxCount = rwSalveCount_;
        }

        // Front Laser - laser
        flDamage_ = Mathf.CeilToInt((flDamage) * (1 + shipData.percLaserDamage / 100));
        flReloadTime_ = Mathf.Max(1f, (flReloadTime * supportReloadTime_));
        flShootingTime_ = flShootingTime;
        if (isFrontLaserInstalled != null)
        {
            isFrontLaserInstalled.bulletDamage = flDamage_;
            isFrontLaserInstalled.realodInterval = flReloadTime_;
            isFrontLaserInstalled.laserShootTime = flShootingTime_;
        }

        // Orbital Laser - laser - defence
        olDamage_ = Mathf.CeilToInt((olDamage) * (1 + shipData.percLaserDamage / 100));
        //olRotationSpeed = olRotationSpeed;
        if (isOrbitalLaserInstalled != null)
        {
            isOrbitalLaserInstalled.damage = olDamage_;
            isOrbitalLaserInstalled.orbCount = olCount;
            isOrbitalLaserInstalled.rotationSpeed = olRotationSpeed;

            isOrbitalLaserInstalled.UpdateOrbs();
        }

        // Thermal Spheres - laser
        tsDamage_ = Mathf.CeilToInt((tsDamage) * (1 + shipData.percLaserDamage / 100));
        tsRelaodTime_ = Mathf.Max(0.1f, (tsReloadTime * supportReloadTime_));
        tsLifetime_ = tsLifetime;
        if (isThermalSpheresInstalled != null)
        {
            isThermalSpheresInstalled.baseDamage = tsDamage_;
            isThermalSpheresInstalled.spawnInterval = tsRelaodTime_;
            isThermalSpheresInstalled.lifeTime = tsLifetime_;
            isThermalSpheresInstalled.UpdateInvoke();
        }

        // Mine Layer - rocket
        mlDamage_ = Mathf.CeilToInt((mlDamage) * (1 + shipData.percRocketDamage / 100));
        mlRelaodTime_ = Mathf.Max(0.1f, (mlReloadTime * supportReloadTime_));
        mlLifetime_ = mlLifetime;
        if (isMineLayerInstalled != null)
        {
            isMineLayerInstalled.baseDamage = mlDamage_;
            isMineLayerInstalled.spawnInterval = mlRelaodTime_;
            isMineLayerInstalled.lifeTime = mlLifetime_;
            isMineLayerInstalled.UpdateInvoke();
        }

        // Backfire Beam - laser
        bbDamage_ = Mathf.CeilToInt((bbDamage) * (1 + shipData.percLaserDamage / 100));
        bbReloadTime_ = Mathf.Max(0.1f, (bbReloadTime * supportReloadTime_));
        if (isBackfireBeamInstalled != null)
        {
            isBackfireBeamInstalled.damage = bbDamage_;
            isBackfireBeamInstalled.StartFire(bbReloadTime_);
            isBackfireBeamInstalled.projectileCount = bbMainBeams;
            isBackfireBeamInstalled.killProjectileCount = bbKillBeams;
        }

        // Spread Gun - bullet - swarm
        sdgDamage_ = Mathf.CeilToInt((sdgDamage) * (1 + shipData.percBulletDamage / 100));
        sdgReloadTime_ = Mathf.Max(0.1f, (sdgReloadTime * supportReloadTime_));
        sdgBulletsPerShoot_ = sdgBulletsPerShoot;
        if (isSideGunInstalled != null)
        {
            isSideGunInstalled.bulletDamage = sdgDamage_;
            isSideGunInstalled.realodInterval = sdgReloadTime_;
            isSideGunInstalled.bulltesPerShoot = sdgBulletsPerShoot_;
        }
    }
    #endregion

    /* **************************************************************************** */
    /* WEAPON EVENTS--------------------------------------------------------------- */
    /* **************************************************************************** */
    public void UpdateMWDamage(int damage)
    {
        OnMWDamage?.Invoke(damage);
    }

    public void UpdateMWLaserReloadTime(float reloadTimeUpdate)
    {
        ONUpdateLaserReloadTime?.Invoke(reloadTimeUpdate);
    }
    public void UpdateMWBulletFireRate(float reloadTimeUpdate)
    {
        ONUpdateBulletRealodTime?.Invoke(reloadTimeUpdate);
    }

    private void UpdatePlayerWeaponController()
    {
        shipData.bulletCritChance = shipData.critChance * shipData.mcBulletLvl;
        shipData.bulletCritDamage = 100 + shipData.critDamage * shipData.mcBulletLvl;

        shipData.rocketAOERadius = shipData.aoeRange * shipData.mcExplosionLvl;

        shipData.burnDamageChance = 1 + shipData.burningChance * shipData.mcLaserLvl;

        shipData.supportReloadTime = shipData.mcSupportLvl * shipData.realodTime;

        for (int i = 0; i < shipData.mcBulletLvl; i++)
        {
            upgradePanelController.classUpgradeOrder.Add(0);
        }
        for (int i = 0; i < shipData.mcExplosionLvl; i++)
        {
            upgradePanelController.classUpgradeOrder.Add(1);
        }
        for (int i = 0; i < shipData.mcLaserLvl; i++)
        {
            upgradePanelController.classUpgradeOrder.Add(2);
        }
        for (int i = 0; i < shipData.mcSupportLvl; i++)
        {
            upgradePanelController.classUpgradeOrder.Add(3);
        }

        WeaponChoose();
    }

    private void ResetShipData()
    {
        shipData.mcBulletLvl = 0;
        shipData.mcExplosionLvl = 0;
        shipData.mcLaserLvl = 0;
        shipData.mcSupportLvl = 0;

        shipData.baseLaserTicks = 4;
        shipData.baseLaserTickDamage = 5;
        shipData.laserBurningTickDamangePercent = 100;
        shipData.chanceToGetScrap = 5;

        shipData.percBulletDamage = 0;
        shipData.percRocketDamage = 0;
        shipData.percLaserDamage = 0;
        shipData.chanceToGetTwoExp = 0;
        shipData.chanceToGetHealth = 0;
        shipData.chanceToGetFullEnergy = 0;
        shipData.rocketLifeTime = 0;
        shipData.percAgility = 0;

        shipData.bossBonusDamage = 0;
        shipData.supportReloadTime = 0;
        shipData.percMainAttackSpeedBullet = 0;
        shipData.percMainAttackSpeedRocket = 0;
        shipData.percMainDownTimeLaser = 0;

        shipData.bulletCritChance = 0;
        shipData.bulletCritDamage = 0;
        shipData.burnDamageChance = 0;
        shipData.rocketAOERadius = 0;

        shipData.shieldHealth = 4;
        shipData.shieldDamage = 0;

        shipData.boostInvulnerability = 0.5f;
        shipData.boostSize = 1.2f;
        shipData.boostDamage = 20;

        shipData.extraRockets = 0;
        shipData.extraDamage = 0;
    }

    /* **************************************************************************** */
    /* Shootsound ----------------------------------------------------------------- */
    /* **************************************************************************** */
    public void BulletShootingSoundEngine()
    {
        if (canPlayBulletShootSfx == true)
        {
            AudioManager.Instance.PlaySFX("PlayerShot");
            canPlayBulletShootSfx = false;
            Invoke("InvokeBulletSoundReset", 0.1f);
        }
    }
    private void InvokeBulletSoundReset()
    {
        canPlayBulletShootSfx = true;
    }
}

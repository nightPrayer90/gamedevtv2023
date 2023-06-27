using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
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

    private bool isHeadCannonInstalled = false;
    private bool isRocketLauncherInstalled = false;
    private bool isFireFliesInstalled = false;
    private bool isBulletWingsInstalled = false;
    private bool isLifeModulInstalled = false;
    private bool isSpreadGunInstalled = false;
    private bool isFrontShieldInstalled = false;
    private bool isBackShieldInstalled = false;
    private bool isNovaExplosionInstalled = false;
    private bool isRockedWingsInstalled = false;
    private bool isFrontLaserInstalled = false;


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


    [Header("Container")]
    public Transform passivParentContainer;


    //private Objects
    private PlayerController playerController;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    void Start()
    {
        // testing
        WeaponChoose();
    }




    /* **************************************************************************** */
    /* Weapon Management----------------------------------------------------------------- */
    /* **************************************************************************** */
    
    public void WeaponChoose()
    {
        if (isHeadCannon == true && isHeadCannonInstalled == false)
        {
            Instantiate(headCannon, passivParentContainer);
            isHeadCannonInstalled = true;
        }
        if (isRocketLauncher == true && isRocketLauncherInstalled == false)
        {
            Instantiate(rocketLauncher, passivParentContainer);
            isRocketLauncherInstalled = true;
        }
        if (isFireFlies == true && isFireFliesInstalled == false)
        {
            Instantiate(fireFlys, passivParentContainer);
            isFireFliesInstalled = true;
        }
        if (isBulletWings == true && isBulletWingsInstalled == false)
        {
            Instantiate(bulletWings, passivParentContainer);
            isBulletWingsInstalled = true;
        }
        if (isLifeModul == true && isLifeModulInstalled == false)
        {
            Instantiate(lifeModul, passivParentContainer);
            isLifeModulInstalled = true;
        }
        if (isSpreadGun == true && isSpreadGunInstalled == false)
        {
            Instantiate(spreadGun, passivParentContainer);
            isSpreadGunInstalled = true;
        }
        if (isFrontShield == true && isFrontShieldInstalled == false)
        {
            var shild = Instantiate(frontShield, passivParentContainer);
            shild.name = frontShield.name;
            isFrontShieldInstalled = true;
        }
        if (isBackShield == true && isBackShieldInstalled == false)
        {
            var shild = Instantiate(backShield, passivParentContainer);
            shild.name = backShield.name;
            isBackShieldInstalled = true;
        }
        if (isNovaExplosion == true && isNovaExplosionInstalled == false)
        {
            Instantiate(novaExplosion, passivParentContainer);
            isNovaExplosionInstalled = true;
        }
        if (isRockedWings == true && isRockedWingsInstalled == false)
        {
            Instantiate(rocketWings, passivParentContainer);
            isRockedWingsInstalled = true;
        }
        if (isFrontLaser == true && isFrontLaserInstalled == false)
        {
            Instantiate(frontLaser, passivParentContainer);
            isFrontLaserInstalled = true;
        }
    }
}

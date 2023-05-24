using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Passiv abilitys")]
    public bool isHeadCannon = false;
    public bool isRocketLauncher = false;
    public bool isFireFlys = false;
    public bool isBulletWings = false;
    public bool isLifeModul = false;
    public bool isMineModul = false;
    public bool isSpreadGun = false;

    private GameObject headCannon;
    private GameObject rocketLauncher;
    private GameObject fireFlys;
    private GameObject bulletWings;
    private GameObject lifeModul;
    private GameObject mineModul;
    private GameObject spreadGun;

    // Start is called before the first frame update
    void Start()
    {
        headCannon = GameObject.Find("Head Cannon");
        rocketLauncher = GameObject.Find("Rocket Launcher");
        fireFlys = GameObject.Find("Fire Flys");
        bulletWings = GameObject.Find("Bullet Wings");
        lifeModul = GameObject.Find("Life Modul");
        mineModul = GameObject.Find("Mine Modul");
        mineModul = GameObject.Find("Spread Gun");
    }



}

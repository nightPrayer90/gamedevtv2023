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
    public bool isSpreadGun = false;
    public bool isFrontShield = false;
    public bool isBackShield = false;

    [Header("Objects")]
    public GameObject headCannon;
    public GameObject rocketLauncher;
    public GameObject fireFlys;
    public GameObject bulletWings;
    public GameObject lifeModul;
    public GameObject spreadGun;
    public GameObject frontShield;
    public GameObject backShield;

    // Start is called before the first frame update
    void Start()
    {
        /*headCannon = GameObject.Find("Head Cannon");
        rocketLauncher = GameObject.Find("Rocket Launcher");
        fireFlys = GameObject.Find("Fire Flys");
        bulletWings = GameObject.Find("Bullet Wings");
        lifeModul = GameObject.Find("Life Modul");
        spreadGun = GameObject.Find("Spread Gun");
        frontShield = GameObject.Find("front Shield");
        backShield = GameObject.Find("back Shield");*/

        //optional
        WeaponChoose();
    }

    public void WeaponChoose()
    {
        headCannon.SetActive(isHeadCannon);
        rocketLauncher.SetActive(isRocketLauncher);
        fireFlys.SetActive(isFireFlys);
        bulletWings.SetActive(isBulletWings);
        lifeModul.SetActive(isLifeModul);
        spreadGun.SetActive(isSpreadGun);
        frontShield.SetActive(isFrontShield);
        backShield.SetActive(isBackShield);
    }

}

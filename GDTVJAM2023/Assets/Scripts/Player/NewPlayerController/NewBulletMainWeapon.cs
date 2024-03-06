using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMWController;

public class NewBulletMainWeapon : MonoBehaviour
{
    //private Objects
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    private NewPlayerController playerController;

    public int bulletBaseDamage;
    public float fireRate;
    public AudioSource WeaponSound;
    public ParticleSystem bulletParticalSystem;
    public ParticleBullet particelBullet;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.gameObject.GetComponent<UpgradeChooseList>();
        playerController = GetComponentInParent<NewPlayerController>();

        // bullet
        UpdateBulletValues();

        // To do
        Invoke("StartShooting", 2.5f);
    }
    #endregion


    /* **************************************************************************** */
    /* Main weapon----------------------------------------------------------------- */
    /* **************************************************************************** */
    // update main Weapon base values from playerController
    #region Mainweapon
    public void UpdateBulletValues()
    {
        // update bullet values
        float temp_fireRate = fireRate;

        // restart Invoke
        if (fireRate != temp_fireRate)
        {
            CancelInvoke("BulletShotEmit");
            InvokeRepeating("BulletShotEmit", 0.3f, fireRate);
        }
        SetBulletDamage();
    }

    // the main weapon start to fire
    public void StartShooting()
    {
        // set the main weapon particle damage
        UpdateBulletValues();

        if (!IsInvoking("BulletShotEmit"))
            InvokeRepeating("BulletShotEmit", 0.5f, fireRate);
    }

    // stop the main waepon fire
    public void StopShooting()
    {
        CancelInvoke("BulletShotEmit");
    }
    #endregion


    /* **************************************************************************** */
    /* BULLET---------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region bullet
    // invoke function - make the main weapon fire
    void BulletShotEmit()
    {
        // shooting sound
        //WeaponSound.Play();

        // emit 1 particle of each mainweapon
        bulletParticalSystem.Emit(1);
    }

    // set the main weapon particle damage
    public void SetBulletDamage()
    {
        particelBullet.BulletSetDamage(bulletBaseDamage);
    }
    #endregion
}

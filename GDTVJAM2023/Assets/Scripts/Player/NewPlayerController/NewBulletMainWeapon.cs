using UnityEngine;

public class NewBulletMainWeapon : BaseModule
{
    //private Objects
    private GameManager gameManager;

    public int bulletBaseDamage;
    private int bulletResouldDamage;
    public float fireRate;
    public AudioSource WeaponSound;
    public ParticleSystem bulletParticalSystem;
    public ParticleBullet particelBullet;
    private NewPlayerController playerController;
    private PlayerWeaponController playerWeaponController;
    public ParticleSystem flashParticle;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle
    void Start()
    {
        // Update Module Values to Player Controller - Function comes from BaseModule
        UpdateModuleValues();

        GameObject go = GameObject.Find("Game Manager");
        if (go != null) {
            gameManager = go.GetComponent<GameManager>();
            playerController = GetComponentInParent<NewPlayerController>();
            playerController.OnIntroOver += HandleStartShooting;
            playerWeaponController = GetComponentInParent<PlayerWeaponController>();
            playerWeaponController.OnMWDamage += HandleDamageUpdate;
            playerWeaponController.ONUpdateBulletRealodTime += HandleFireRateUpdate;

            // bullet
            UpdateBulletValues();
        }
    }

    public void HandleDamageUpdate(int damageToUpdate)
    {
        bulletBaseDamage = (bulletBaseDamage + damageToUpdate );
        SetBulletDamage();
    }

    private void HandleStartShooting()
    {
        Invoke("StartShooting", 0.2f);
    }

    private void HandleFireRateUpdate(float fireRateUpdate)
    {
        fireRate = fireRate * fireRateUpdate;
        CancelInvoke("BulletShotEmit");
        InvokeRepeating("BulletShotEmit", 0.3f, fireRate);
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
        if (fireRate != temp_fireRate) {
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
        EnergieDamageDebuff();

        // emit 1 particle of each mainweapon
        bulletParticalSystem.Emit(1);
        if (flashParticle != null) flashParticle.Emit(1);

        // trigger sound
        playerWeaponController.BulletShootingSoundEngine();
    }

    // set the main weapon particle damage
    public void SetBulletDamage()
    {
        bulletResouldDamage = bulletBaseDamage + Mathf.CeilToInt((float)bulletBaseDamage * (playerWeaponController.shipData.percBulletDamage / 100));
        particelBullet.BulletSetDamage(bulletResouldDamage);
    }
    #endregion

    // Debuff is EnergieProduction < 0
    private void EnergieDamageDebuff()
    {
        if (playerController.energieProduction < 0) {
            bulletResouldDamage = bulletBaseDamage + Mathf.CeilToInt((float)bulletBaseDamage * (playerWeaponController.shipData.percBulletDamage / 100) * 0.5f);
            particelBullet.BulletSetDamage(bulletResouldDamage);

        }
    }
}

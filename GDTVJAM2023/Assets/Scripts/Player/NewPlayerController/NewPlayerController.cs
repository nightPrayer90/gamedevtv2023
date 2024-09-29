using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NewPlayerController : MonoBehaviour
{
    [Header("Player Intro")]
    //public float startImpulse = 100;
    private bool isIntro = true;
    //private bool isStartSound = false;
    private float introTargetY = 6f;

    // engine stuff
    [HideInInspector] public bool hasMainEngine = false;
    private bool isEnginePlayed = false;
    [HideInInspector] public bool useBoost = false;

    [Header("Player Movement")]
    [HideInInspector] public float rotateInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public float strafeInput;
    [HideInInspector] public bool boostInput;
    [HideInInspector] public bool abilityInput;
    [HideInInspector] public bool isboostSoundFlag = false;
    

    [Header("Player Stats")]
    [SerializeField] private bool canTakeDamge = true;
    public int playerMaxHealth = 10;
    public int playerCurrentHealth = 10;
    public float pickupRange = 5;
    public float energieProduction = 1f;
    public float energieMax = 10f;
    public float energieCurrent = 0f;
    /*[HideInInspector]*/
    public float protectionPerc = 0;
    /*[HideInInspector]*/
    public int protectionLvl = 0;
    private bool canGetLaserDamage = true;

    [Header("Movement Input")]
    public PlayerInputHandler intputHandler;

    [Header("Player Level Properties")]
    public int playerLevel = 1;
    private int playerCurrentExperience;
    private int playerExperienceToLevelUp = 6;
    private float playerLevelUpFactor = 1.2f;

    [Header("Game Objects")]
    public GameObject novaOnHit;
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private NavigationController navigationController;
    [SerializeField] private GameObject centerOfMass;
    public Rigidbody playerRigidbody;
    [SerializeField] private GameManager gameManager;
    //[SerializeField] private GameObject powerBoostMarker;
    public List<ParticleCollisionEvent> collisionEvents;
    public UpgradeChooseList upgradeChooseList;
    public PlayerWeaponController playerWeaponController;
    public ParticleSystem damageParticle;


    [Header("Outside Border")]
    public float damageInterval = 1f;
    public int damageTaken = 2;
    public bool isOutsideBorder = false;

    [Header("Floating Text")]
    public Color hitColor = new Color(1f, 0.0f, 0.0f, 1f);
    private Color hitColorTemp;
    public Color enemyHitColor = new Color(1f, 0.0f, 0.0f, 1f);
    public Color enemyHitColorProtected = new Color(1f, 0.0f, 0.0f, 1f);

    [Header("Debug")]
    [SerializeField] private float flySpeed;

    // Events
    public event Action OnIntroOver;
    public event Action<float> OnUpdateRotateSpeed;

    // Debuffs
    private int moveControlChange = 1;
    public bool isDebuff = false;

    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle
    void Awake()
    {
        playerRigidbody.centerOfMass = new Vector3(0f, 0f, 0f);
        hitColorTemp = hitColor;

        // intro starting sound
        AudioManager.Instance.PlaySFX("LiftUPBoss");

        // starting Energie System
        InvokeRepeating(nameof(EnergieRegen), 2f, 0.1f);

        // TODO Update protection after loading
        Invoke(nameof(UpdateProtection), 1f);
        Invoke(nameof(SetModuleIndex), 1f);
    }

    private void UpdateProtection()
    {
        // Invoke from Awake !TODO
        float normalizedLvl = Mathf.InverseLerp(0, 10, protectionLvl);
        float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);
        protectionPerc = targetPercentage;
    }

    private void SetModuleIndex()
    {
        NewLaserMainWeapon[] foundLasers;
        NewBulletMainWeapon[] foundBullets;
        NewRocketMainWeapon[] foundRockets;
        NewSphereThrower[] foundSphereThrowers;

        foundLasers = gameObject.GetComponentsInChildren<NewLaserMainWeapon>();
        foundBullets = gameObject.GetComponentsInChildren<NewBulletMainWeapon>();
        foundRockets = gameObject.GetComponentsInChildren<NewRocketMainWeapon>();
        foundSphereThrowers = gameObject.GetComponentsInChildren<NewSphereThrower>();

        for (int i = 0; i < foundLasers.Length; i++)
        { foundLasers[i].moduleIndex = i; }

        for (int i = 0; i < foundBullets.Length; i++)
        { foundBullets[i].moduleIndex = i; }

        for (int i = 0; i < foundRockets.Length; i++)
        {
            foundRockets[i].moduleIndex = i; }

        for (int i = 0; i < foundSphereThrowers.Length; i++)
        { foundSphereThrowers[i].moduleIndex = i; }

        if (foundLasers.Length > 0) upgradeChooseList.upgrades[49].upgradeStartCount = 3;
        if (foundBullets.Length > 0) upgradeChooseList.upgrades[51].upgradeStartCount = 999;
        if (foundRockets.Length > 0) upgradeChooseList.upgrades[55].upgradeStartCount = 1;
        if (foundSphereThrowers.Length > 0) upgradeChooseList.upgrades[82].upgradeStartCount = 3;
    }

    void Update()
    {
        // Debug
        //CalculateFlySpeed();
        if (isIntro == true)
        {
            if (transform.position.y < introTargetY)
            {
                // Intro
                Vector3 movement = new Vector3(0f, 1.5f, 0f) * Time.deltaTime;
                transform.Translate(movement);
            }
            else
            {
                // Startboost in allen Engines auslösen
                AudioManager.Instance.PlaySFX("ShortAlert");
                OnIntroOver?.Invoke();
                if (hasMainEngine == true)
                {
                    playerRigidbody.AddForce(transform.forward * 1200, ForceMode.Force);
                }
                isIntro = false;
            }
        }
        else
        {
            //Gameplay Loop
            HandleInput();
        }
        //debug
        //masseanzeiger
        //centerOfMass.transform.localPosition = playerRigidbody.centerOfMass + new Vector3(0f, 1f, 0f);
    }

    private void FixedUpdate()
    {
        // Überprüfen, ob keine horizontale Richtungsänderung erfolgt
        if (Mathf.Abs(rotateInput) < 0.1f)
        {
            // Aktuellen X-Rotationswert abrufen
            float currentRotationX = transform.rotation.eulerAngles.x;

            // Ziel-X-Rotationswert auf 0 setzen
            Quaternion targetRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            // Lerpen der X-Rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f * Time.fixedDeltaTime);
        }
    }

    #endregion

    /* **************************************************************************** */
    /* Collision Stuff------------------------------------------------------------- */
    /* **************************************************************************** */
    #region collision stuff

    // activate if trigger enter - some Pickup stuff
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        switch (tag)
        {
            case "Exp":
                
                /*int expValue = other.GetComponent<EnemyExp>().expValue;
                UpdatePlayerExperience(expValue);*/
                break;

            case "UpgradePickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                PlayerWeaponUpdatePickup();
                break;

            case "DimensionPickUp":
                other.gameObject.SetActive(false);
                navigationController.DeactivateNavigatorMesh();
                gameManager.GoToDimension();
                AudioManager.Instance.PlaySFX("DimensionSwap");
                break;

            case "VictoryPickup":
                other.gameObject.SetActive(false);
                gameManager.Victory();
                AudioManager.Instance.PlaySFX("VictorySound");
                break;

            case "BorderCollider":
                if (isOutsideBorder == true)
                {
                    gameManager.outsideBorderText.text = "";
                    CancelInvoke(nameof(PlayerIsOutsideBorder));
                    CancelInvoke(nameof(PlayerIsOutsideBorderWarning));
                    isOutsideBorder = false;
                }
                break;
        }
    }

    public void PlayerWeaponUpdatePickup()
    {
        Time.timeScale = 0;
        AudioManager.Instance.PlaySFX("LevelUp");
        //engineAudioSource.Stop();

        int temphealth = playerMaxHealth - playerCurrentHealth;
        UpdatePlayerHealth(-temphealth);

        // refresh player UI
        gameManager.PlayerWeaponUpdatePickup();
    }

    // activate if trigger stay - border control
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BorderCollider"))
        {
            if (isOutsideBorder == true)
            {
                CancelInvoke(nameof(PlayerIsOutsideBorder));
                CancelInvoke(nameof(PlayerIsOutsideBorderWarning));
                gameManager.outsideBorderText.text = "";
                isOutsideBorder = false;
            }
        }
    }

    // activate if trigger exit - border control
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BorderCollider"))
        {
            if (isOutsideBorder == false && isIntro == false)
            {
                InvokeRepeating(nameof(PlayerIsOutsideBorder), 2.2f, 1f);// damageInterval);
                Invoke(nameof(PlayerIsOutsideBorderWarning), 0.1f);

                gameManager.outsideBorderText.text = "outside border!";
                gameManager.outsideBorderTextTweenTarget.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.8f, 10, 0.5f);
                isOutsideBorder = true;

            }
        }
    }

    // activate if collide with an Enemy
    private void OnCollisionEnter(Collision collision)
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }

        if (collision.gameObject.CompareTag(tagStr))
        {
            // get enemyHealth component
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {

                // find the right explosion direction
                Vector3 explosionDirection = collision.transform.position - transform.position;
                explosionDirection.Normalize();

                // trigger a Explosion on the Enemy
                if (enemyHealth.isBoss == false) ObjectPoolManager.SpawnObject(enemyHealth.explosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

                // if the player is not invulnerability 
                if (canTakeDamge == true || enemyHealth.canPoolObject == false)
                {
                    // add a force after the collision to the player
                    playerRigidbody.AddForce(explosionDirection * -1f * enemyHealth.explosionForce, ForceMode.Impulse);

                    // calculate player health
                    int damage = Protection(enemyHealth.collisonDamage);
                    UpdatePlayerHealth(damage);

                    // trigger the damage floating text
                    gameManager.DoFloatingText(transform.position, damage.ToString(), hitColor);

                    if (upgradeChooseList.upgrades[35].upgradeIndexInstalled > 0) NovaOnHit(2f, 8);
                }
                else
                {
                    // add a force after the collision to the player
                    if (upgradeChooseList.upgrades[36].upgradeIndexInstalled > 0)
                    {
                        NovaOnHit(1.2f, 6);
                        playerRigidbody.AddForce(explosionDirection * 1.4f * enemyHealth.explosionForce, ForceMode.Impulse);
                    }
                    else
                    {
                        gameManager.DoFloatingText(collision.transform.position, enemyHealth.enemyHealth.ToString(), enemyHealth.hitColor);
                        playerRigidbody.AddForce(transform.forward * 2.5f, ForceMode.Impulse);
                    }
                }

                // refresh the UI
                if (enemyHealth.secondDimensionEnemy == false)
                {
                    gameManager.UpdateEnemyCounter(-1);
                    gameManager.UpdateEnemyToKill(1);
                }

                if (enemyHealth.isBoss == false)
                {
                    // destroy the enemy
                    enemyHealth.DestroyEnemy();
                }
            }
        }
    }

    // player get bullet damage
    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>(); // *** important! Making a variable to acess the particle system of the emmiting object, in this case, the lasers from my player ship.

        if (canTakeDamge == true)
        {
            var ps = other.GetComponent<EnemyParticleBullet>();
            int damage = ps.bulletDamage;

            damage = Protection(damage);
            UpdatePlayerHealth(damage);

            gameManager.DoFloatingText(transform.position, damage.ToString(), hitColor);
        }
    }

    // player get damage vom a laserpointer
    public void GetLaserDamage(int damage)
    {
        if (canGetLaserDamage == true && canTakeDamge == true)
        {
            damage = Protection(damage);
            UpdatePlayerHealth(damage);
            gameManager.DoFloatingText(transform.position, damage.ToString(), hitColor);

            Invoke(nameof(InvokeLaserDamage), 1f);

            canGetLaserDamage = false;
        }
    }

    private void InvokeLaserDamage()
    {
        canGetLaserDamage = true;
    }
    #endregion


    /* **************************************************************************** */
    /* Movement Stuff-------------------------------------------------------------- */
    /* **************************************************************************** */
    #region movement stuff
    
    void HandleInput()
    {
        strafeInput = intputHandler.MoveInput.x * moveControlChange;
        verticalInput = intputHandler.MoveInput.y * moveControlChange;
        rotateInput = intputHandler.RotateInput * moveControlChange;
        boostInput = intputHandler.BoostInput;
        abilityInput = intputHandler.AbilityInput;


        ShipEngineSound();
    }

    // Boss 4
    public void SetMoveControlDebuff()
    {
        moveControlChange = moveControlChange * (-1);
    }

    void CalculateFlySpeed()
    {
        flySpeed = playerRigidbody.velocity.magnitude;
        Debug.Log("Speed: " + flySpeed);
    }
    #endregion


    /* **************************************************************************** */
    /* Energy System--------------------------------------------------------------- */
    /* **************************************************************************** */
    #region energie System
    //InvokeRepeating - Awake
    private void EnergieRegen()
    {
        if (energieCurrent < energieMax)
        {
            energieCurrent = Mathf.Min(energieMax, energieCurrent + energieProduction / 10);
            gameManager.UpdateEnergieSlider(Mathf.Max(0, energieCurrent));
        }
    }


    #endregion



    /* **************************************************************************** */
    /* Health and Experience------------------------------------------------------- */
    /* **************************************************************************** */
    #region health and experience

    // update the player experience
    public void UpdatePlayerExperience(int expValue)
    {
        int exp = expValue;

        // chance to get double exp
        if (playerWeaponController.shipData.chanceToGetTwoExp > 0)
        {
            if (UnityEngine.Random.Range(0, 100) <= playerWeaponController.shipData.chanceToGetTwoExp)
            {
                exp *= 2;
                gameManager.DoFloatingText(transform.position, $"+{exp}", Color.white);
            }
        }

        // chance to get one health
        if (playerWeaponController.shipData.chanceToGetHealth > 0)
        {
            if (UnityEngine.Random.Range(0, 100) <= playerWeaponController.shipData.chanceToGetHealth)
            {
                gameManager.DoFloatingText(transform.position, "+1", Color.green);
                UpdatePlayerHealth(-1);
            }
        }

        // chance to get full energie
        if (playerWeaponController.shipData.chanceToGetFullEnergy > 0 && energieCurrent < energieMax)
        {
            if (UnityEngine.Random.Range(0, 100) <= playerWeaponController.shipData.chanceToGetFullEnergy)
            {
                gameManager.DoFloatingText(transform.position, "E", Color.magenta);
                energieCurrent = energieMax;
            }
        }


        playerCurrentExperience += exp;
        bool isLevelUp;

        // if level up
        if (playerCurrentExperience >= playerExperienceToLevelUp)
        {
            Time.timeScale = 0;
            AudioManager.Instance.PlaySFX("LevelUp");
            //engineAudioSource.Stop();

            // recalculate new experience values
            playerLevel++;
            playerExperienceToLevelUp = Mathf.RoundToInt(playerExperienceToLevelUp * playerLevelUpFactor);
            playerCurrentExperience = 0;

            // player get +1 health und heal 25 % of his life
            playerMaxHealth++;

            int temphealth = Mathf.RoundToInt(playerMaxHealth * 0.25f);
            UpdatePlayerHealth(-temphealth);

            isLevelUp = true;
        }
        else
        {
            // play collect sound
            AudioManager.Instance.PlaySFX("ExperienceOrb");
            isLevelUp = false;
        }

        // refresh player UI
        gameManager.UpdateUIPlayerExperience(isLevelUp, playerLevel, playerExperienceToLevelUp, playerCurrentExperience, exp);
    }


    // update player life after get damage, heal or level up
    public void UpdatePlayerHealth(int decHealth)
    {
        
        if (decHealth > 0)
        {
            if (!damageParticle.isPlaying)
            {
                damageParticle.Play();
            }
        }

        // calculate the player health value
        playerCurrentHealth = Mathf.Min(Mathf.Max(0, playerCurrentHealth - decHealth), playerMaxHealth);

        // update playerUI
        gameManager.UpdateUIPlayerHealth(playerCurrentHealth, playerMaxHealth);

        // get damage but dont die
        if (decHealth > 0)
        {
            gameManager.ScreenShake(1);
            AudioManager.Instance.PlaySFX("PlayerGetDamage");
        }

        // player die
        if (playerCurrentHealth <= 0)
        {
            gameObject.SetActive(false);
            gameManager.GameIsOver();
        }
    }

    // recalculated incoming damage with the protection value - Player get 50% damage
    private int Protection(int damage)
    {
        int adjustedDamge = damage;

        if (damage > 1)
        {
            int ran = UnityEngine.Random.Range(0, 99);

            if (ran <= protectionPerc)
            {
                adjustedDamge = Mathf.RoundToInt((float)damage * 0.5f);
                hitColor = enemyHitColorProtected;
            }
            else
            {
                hitColor = hitColorTemp;
            }
        }

        return adjustedDamge;
    }
    #endregion



    /* **************************************************************************** */
    /* Fly Controll---------------------------------------------------------------- */
    /* **************************************************************************** */
    #region fly controll
    // the player take damage after he is outside the border
    private void PlayerIsOutsideBorder()
    {
        UpdatePlayerHealth(damageTaken);
        gameManager.outsideBorderTextTweenTarget.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.6f, 15, 0.5f);
    }
    // the player goes outside border
    private void PlayerIsOutsideBorderWarning()
    {
        AudioManager.Instance.PlaySFX("WarningBoss");
    }

    // activate the navigation controller
    public void SetNavigationController()
    {
        AudioManager.Instance.PlaySFX("ShortAlert");
        navigationController.SetTargetPosition();
    }
    #endregion




    /* **************************************************************************** */
    /* MISC------------------------------------------------------------------------ */
    /* **************************************************************************** */
    #region MISC
    // trigger a nova on Hit
    public void NovaOnHit(float explosionRadius, int NovaDamage)
    {
        // Audio
        AudioManager.Instance.PlaySFX("Playernova");

        Vector3 pos = transform.position;
        LayerMask layerMask = (1 << 6);
        explosionRadius = explosionRadius * (1+ playerWeaponController.shipData.rocketAOERadius/100);
        NovaDamage = 6;

        if (gameManager.dimensionShift == true)
        {
            layerMask = (1 << 9);
        }

        // array of all Objects in the explosionRadius
        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

        foreach (var obj in surroundingObjects)
        {
            // get rigidbodys from all objects in range
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            // calculate distance between explosioncenter and objects in Range
            float distance = Vector3.Distance(pos, rb.transform.position);

            if (distance < explosionRadius)
            {
                float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
                int adjustedDamage = Mathf.CeilToInt(NovaDamage * scaleFactor);

                // get EnemyHealthscript
                EnemyHealth eHC = obj.GetComponent<EnemyHealth>();
                Color resultColor = enemyHitColor;

                if (eHC != null)
                {
                    if (upgradeChooseList.upgrades[54].upgradeIndexInstalled > 0)
                    {
                        int ran = UnityEngine.Random.Range(0, 100);
                        if (ran < playerWeaponController.shipData.bulletCritChance)
                        {
                            adjustedDamage = eHC.CritDamage(adjustedDamage);
                            resultColor = eHC.critColor;
                        }
                    }

                    // show floating text
                    if (eHC.canTakeDamage == true)
                        gameManager.DoFloatingText(rb.transform.position, adjustedDamage.ToString(), resultColor);

                    // calculate enemy damage
                    eHC.TakeExplosionDamage(adjustedDamage);

                }
                rb.AddExplosionForce(400, pos, explosionRadius);
            }
        }

        GameObject go = ObjectPoolManager.SpawnObject(novaOnHit, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;

    }

    // player can take damage
    private void Invulnerability()
    {
        canTakeDamge = true;
        //powerBoostMarker.SetActive(false);
    }

    public void GetInvulnerabilityAfterUpdate()
    {
        canTakeDamge = false;
        Invoke(nameof(Invulnerability), 0.5f);
    }

    // player can take damage
    public void GetInvulnerability()
    {
        canTakeDamge = false;
        //powerBoostMarker.SetActive(true);
        Invoke(nameof(Invulnerability), playerWeaponController.shipData.boostInvulnerability);
    }

    public void UpdateAgility(float updateSpeed)
    {
        OnUpdateRotateSpeed?.Invoke(updateSpeed);
    }

    private void ShipEngineSound()
    {
        if ((rotateInput != 0 || verticalInput != 0 || strafeInput != 0))
        {
            if (isEnginePlayed == false)
            {
                engineSound.DOComplete();
                engineSound.Play();
                engineSound.DOFade(AudioManager.Instance.sfxVolume, 0.2f);
                isEnginePlayed = true;
            }
            else
            {
                if (useBoost == true && isboostSoundFlag == false)
                {
                    AudioManager.Instance.PlaySFX("PlayerBoost");
                    isboostSoundFlag = true;
                }
            }

        }
        else
        {
            if (isEnginePlayed == true)
            {
                engineSound.Stop();
                isEnginePlayed = false;
            }
        }

        if (useBoost == false && boostInput == false)
        {
            isboostSoundFlag = false;
        }
    }

    #endregion
}

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

    [Header("Player Movement")]
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public float horizontalInput2;

    [Header("Player Stats")]
    [SerializeField] private bool canTakeDamge = true;
    public int playerMaxHealth = 10;
    public int playerCurrentHealth = 10;
    public float pickupRange = 5;
    public float energieProduction = 1f;
    public float energieMax = 10f;
    public float energieCurrent = 0f;
    [HideInInspector] public float protectionPerc = 0;
    [HideInInspector] public int protectionLvl = 0;
    private bool canGetLaserDamage = true;

    [Header("Player Level Properties")]
    private int playerCurrentExperience;
    private int playerExperienceToLevelUp = 6;
    private int playerLevel = 1;
    private float playerLevelUpFactor = 1.2f;

    [Header("Game Objects")]
    public GameObject novaOnHit;
    [SerializeField] private NavigationController navigationController;
    [SerializeField] private GameObject centerOfMass;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject powerBoostMarker;
    public List<ParticleCollisionEvent> collisionEvents;
    private UpgradeChooseList upgradeChooseList;
    private PlayerWeaponController playerWeaponController;
    

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

    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle
    // Start is called before the first frame update
    void Awake()
    {
        playerRigidbody.centerOfMass = new Vector3(0f, 0f, 0f);
        hitColorTemp = hitColor;
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        playerWeaponController = GetComponent<PlayerWeaponController>();

        // intro starting sound
        AudioManager.Instance.PlaySFX("LiftUPBoss");

        // starting Energie System
        InvokeRepeating("EnergieRegen", 2f, 0.1f);
        
    }

  

    // Update is called once per frame
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
        centerOfMass.transform.localPosition = playerRigidbody.centerOfMass + new Vector3(0f,1f,0f);
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
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdatePlayerExperience();
                break;

            case "BulletPickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdateClassLevel(0);
                break;

            case "ExplosionPickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdateClassLevel(1);
                break;

            case "LaserPickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdateClassLevel(2);
                break;

            case "SupportPickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdateClassLevel(3);
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
                    CancelInvoke("PlayerIsOutsideBorder");
                    CancelInvoke("PlayerIsOutsideBorderWarning");
                    isOutsideBorder = false;
                }
                break;
        }
    }

    private void UpdateClassLevel(int input)
    {
        AudioManager.Instance.PlaySFX("WindowOpen");
        string floatingText = "";
        switch (input)
        {
            case 0:
                upgradeChooseList.mcBulletLvl++;
                upgradeChooseList.baseBulletCritChance += upgradeChooseList.critChance;
                upgradeChooseList.baseBulletCritDamage += upgradeChooseList.critDamage;
                floatingText = "+1 bullet class";
                break;
            case 1:
                upgradeChooseList.mcExplosionLvl++;
                upgradeChooseList.baseRocketAOERadius += upgradeChooseList.aoeRange;
                floatingText = "+1 explosion class";
                break;
            case 2:
                upgradeChooseList.mcLaserLvl++;
                upgradeChooseList.baseLaserBurnDamageChance += upgradeChooseList.burningChance;
                floatingText = "+1 laser class";
                break;
            case 3:
                upgradeChooseList.mcSupportLvl++;
                floatingText = "+1 support class";
                upgradeChooseList.baseSupportRealoadTime += upgradeChooseList.supportRealodTime;
                break;
        }

        gameManager.DoFloatingText(transform.position, floatingText, gameManager.globalClassColor[input]);
        playerWeaponController.UpdateWeaponValues();
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
                CancelInvoke("PlayerIsOutsideBorder");
                CancelInvoke("PlayerIsOutsideBorderWarning");
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
                InvokeRepeating("PlayerIsOutsideBorder", 2.2f, 1f);// damageInterval);
                Invoke("PlayerIsOutsideBorderWarning", 0.1f);

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
                ObjectPoolManager.SpawnObject(enemyHealth.collisionExplosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

                // if the player is not invulnerability 
                if (canTakeDamge == true || enemyHealth.canPoolObject == false)
                {
                    // add a force after the collision to the player
                    playerRigidbody.AddForce(explosionDirection * -1f * enemyHealth.explosionForce, ForceMode.Impulse);

                    // calculate player health
                    int damage = Protection(enemyHealth.collisonDamage);
                    UpdatePlayerHealth(damage);

                    // trigger the damage floating text
                    gameManager.DoFloatingText(transform.position, "+" + damage.ToString(), hitColor);
                    
                    if (upgradeChooseList.weaponIndexInstalled[35] == 1) NovaOnHit(2f, 8);
                }
                else
                {
                    // add a force after the collision to the player
                    if (upgradeChooseList.weaponIndexInstalled[36] == 1)
                    {
                        NovaOnHit(1.2f, 6);
                        playerRigidbody.AddForce(explosionDirection * 1.4f * enemyHealth.explosionForce, ForceMode.Impulse);
                    }
                    else
                    {
                        gameManager.DoFloatingText(collision.transform.position, "+" + enemyHealth.enemyHealth, enemyHitColor);
                        playerRigidbody.AddForce(transform.forward * 2.5f , ForceMode.Impulse);
                    }
                }

                // refresh the UI
                if (enemyHealth.secondDimensionEnemy == false)
                {
                    gameManager.UpdateEnemyCounter(-1);
                    gameManager.UpdateEnemyToKill(1);
                }

                // destroy the enemy
                Destroy(collision.gameObject);
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

            gameManager.DoFloatingText(transform.position, "+" + damage.ToString(), hitColor);
        }
    }

    // player get damage vom a laserpointer
    public void GetLaserDamage(int damage)
    {
        if (canGetLaserDamage == true && canTakeDamge == true)
        {
            damage = Protection(damage);
            UpdatePlayerHealth(damage);
            gameManager.DoFloatingText(transform.position, "+" + damage.ToString(), hitColor);

            Invoke("InvokeLaserDamage", 1f);

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
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        horizontalInput2 = Input.GetAxis("Horizontal2");
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
        if(energieCurrent < energieMax)
        {
            energieCurrent = Mathf.Min(energieMax, energieCurrent + energieProduction / 10);
            gameManager.UpdateEnergieSlider(Mathf.Max(0,energieCurrent));
        }
    }


    #endregion



    /* **************************************************************************** */
    /* Health and Experience------------------------------------------------------- */
    /* **************************************************************************** */
    #region health and experience

    // update the player experience
    private void UpdatePlayerExperience()
    {
        int exp = 1;
        // get + 1 experience

        // chance to get double exp
        if (upgradeChooseList.chanceToGetTwoExp > 0)
        {
            if (UnityEngine.Random.Range(0, 100) <= upgradeChooseList.chanceToGetTwoExp)
            {
                gameManager.DoFloatingText(transform.position, "+", Color.white);
                exp = 2;
            }
        }

        // chance to get one health
        if (upgradeChooseList.chanceToGet1Health > 0)
        {
            if (UnityEngine.Random.Range(0, 100) <= upgradeChooseList.chanceToGet1Health)
            {
                gameManager.DoFloatingText(transform.position, "+1", Color.green);
                UpdatePlayerHealth(-1);
            }
        }

        // chance to get full energie
        if (upgradeChooseList.chanceToGetfullEnergy > 0 && energieCurrent < energieMax)
        {
            if (UnityEngine.Random.Range(0, 100) <= upgradeChooseList.chanceToGetfullEnergy)
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
        Debug.Log("invoke");
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
        explosionRadius = explosionRadius + playerWeaponController.rocketAOERadius;
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
                    if (upgradeChooseList.weaponIndexInstalled[54] == 1)
                    {
                        int ran = UnityEngine.Random.Range(0, 100);
                        if (ran < playerWeaponController.bulletCritChance)
                        {
                            adjustedDamage = eHC.CritDamage(adjustedDamage);
                            resultColor = eHC.critColor;
                        }
                    }

                    // show floating text
                    if (eHC.canTakeDamage == true)
                        gameManager.DoFloatingText(rb.transform.position, "+" + adjustedDamage.ToString(), resultColor);

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
        powerBoostMarker.SetActive(false);
    }

    // player can take damage
    public void GetInvulnerability()
    {
        canTakeDamge = false;
        powerBoostMarker.SetActive(true);
        Invoke("Invulnerability", upgradeChooseList.baseBoostInvulnerability);
    }

    public void UpdateAgility(float updateSpeed)
    {
        OnUpdateRotateSpeed?.Invoke(updateSpeed);
    }
    #endregion
}

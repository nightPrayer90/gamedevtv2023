using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    [Header("Player Intro")]
    public float startImpulse = 100;
    private bool isIntro = true;
    private bool isStartSound = false;
    private float introTargetY = 6f;


    [Header("Player Stats")]
    public ShipData shipData;
    [HideInInspector] public float speed;
    [HideInInspector] public float rotateSpeed = 0.25f;
    [HideInInspector] public int playerMaxHealth = 10;
    public int playerBulletBaseDamage = 2;
    [HideInInspector] public float playerFireRate = 0.5f;
    [HideInInspector] public float pickupRange = 2f;
    [HideInInspector] public float boostValue = 1f;
    [HideInInspector] public float boostPower = 3f;
    [HideInInspector] public float protectionPerc = 0;
    [HideInInspector] public int protectionLvl = 0;


    [Header("Properties")]
    public int playerCurrentHealth = 10;
    private int playerCurrentExperience;
    private int playerExperienceToLevelUp = 6;
    private int playerLevel = 1;
    private float playerLevelUpFactor = 1.2f;
    private bool isBoost = false;
    private bool setPosition = false;
    private float originalRotationX;
    private float forwardInput;
    private float horizontalInput;
    private float horizontalInput2;
    [HideInInspector] public float currentRotationX;
    private float targetRotation2 = 0;
    public List<ParticleSystem> engineParticles;
    public ParticleSystem boostEngine;
    private bool canGetLaserDamage = true;
    public bool playerMouseMode = false;
    [SerializeField] private bool canTakeDamge = true;

    [Header("Outside Border")]
    public float damageInterval = 1f;
    public int damageTaken = 2;
    public bool isOutsideBorder = false;


    [Header("Floating Text")]
    public List<ParticleCollisionEvent> collisionEvents; // creating a list to store the collision events
    public Color hitColor = new Color(1f, 0.0f, 0.0f, 1f);
    private Color hitColorTemp;
    public Color enemyHitColor = new Color(1f, 0.0f, 0.0f, 1f);
    public Color enemyHitColorProtected = new Color(1f, 0.0f, 0.0f, 1f);

    [Header("Game Objects")]
    public NavigationController navigationController;
    public AudioSource engineAudioSource;
    public ParticleSystem boostParticle;
    public Transform playerMesh;
    public GameObject novaOnHit;
    public GameObject powerBoostMarker;
    private Rigidbody playerRb;
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    private PlayerMWController playerMWController;
    private PlayerWeaponController playerWeaponController;

    private Quaternion previousRotation;
    private CameraController cameraController;

    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle
    void Start()
    {
        // set game objects
        playerRb = GetComponent<Rigidbody>();
        playerMWController = GetComponent<PlayerMWController>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        previousRotation = transform.rotation;
        cameraController = GameObject.Find("Camera Controller").GetComponent<CameraController>();
        hitColorTemp = hitColor;
        powerBoostMarker.SetActive(false);
        

        // get Data from shipDataObject
        speed = shipData.speed;
        rotateSpeed = shipData.rotateSpeed;
        playerMaxHealth = shipData.health;
        playerCurrentHealth = playerMaxHealth;
        playerBulletBaseDamage = shipData.baseDamage;
        playerFireRate = shipData.fireRate;
        pickupRange = shipData.pickupRange;
        boostValue = shipData.boostDuration;
        boostPower = shipData.boostPower;
        protectionLvl = shipData.protectionLevel;

        // calculate Protection perc
        float normalizedLvl = Mathf.InverseLerp(0, 10, protectionLvl);
        float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);
        protectionPerc = targetPercentage;

        // intro starting sound
        AudioManager.Instance.PlaySFX("LiftUPBoss");

        originalRotationX = playerMesh.localRotation.x - 90;
        currentRotationX = originalRotationX;
    }

    private void FixedUpdate()
    {
        if (isIntro = true && transform.position.y < introTargetY)
        {
            // Intro
            Vector3 movement = new Vector3(0f, 2f, 0f) * Time.deltaTime;
            transform.Translate(movement);
        }
        else
        {
            if (isStartSound == false)
            {
               
                isStartSound = true;
                foreach (ParticleSystem ep in engineParticles)
                {
                    ep.Play();
                }
                playerRb.AddForce(transform.forward * -speed * startImpulse, ForceMode.Force);
                // set .y to 6f
                playerRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                // start Shooting
                playerMWController.StartShooting();
            }
            isIntro = false;
        }

        // Game Control
        if (isIntro == false)
        {
            PlayerMovement();
        }
    }

    private void Update()
    {
        if (gameManager.gameIsPlayed && !gameManager.gameOver && !isIntro)
        {
            // get Input values from the user
            forwardInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
            horizontalInput2 = Input.GetAxis("Horizontal2");

            if (Input.GetMouseButton(1))
            {
                playerMouseMode = true;
                if (Input.GetMouseButton(0)) //?
                {
                    forwardInput = 1;
                }
            }
            else
            {
                playerMouseMode = false;
            }

            if (Input.GetButtonUp("Boost"))
            {
                CancelInvoke("BoostReload");
                Invoke("BoostReload", 0.5f);
                isBoost = false;
                setPosition = true;
            }

            // set PlayerMesh position back to zero
            if (setPosition == true)
            {

                if (Vector3.Distance(playerMesh.localPosition, Vector3.zero) > 0.005f)
                {
                    playerMesh.localPosition = Vector3.Lerp(playerMesh.localPosition, Vector3.zero, Time.deltaTime * 3f);
                }
                else
                {
                    setPosition = false;
                }
            }

            float targetRotationX = 0;
            // player use mouse to control
            if (playerMouseMode == true)
            {
                Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);
                float rotationAngle = Mathf.Abs(Quaternion.Angle(Quaternion.identity, deltaRotation));
                float rotationSpeedNormalized = Mathf.Clamp(deltaRotation.eulerAngles.y > 180 ? -rotationAngle : rotationAngle, -1f, 1f);

                previousRotation = transform.rotation;
                targetRotationX = originalRotationX - (rotationSpeedNormalized * 20f) - targetRotation2;
            }

            // WASD - mode
            else
            {
                targetRotationX = originalRotationX - (horizontalInput * 20f) - targetRotation2;
            }

            currentRotationX = Mathf.Lerp(currentRotationX, targetRotationX, Time.deltaTime * 15f);
            playerMesh.localRotation = Quaternion.Euler(currentRotationX, transform.rotation.y + 90f, transform.rotation.z);

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

            case "DimensionPickUpGoBack":
                other.gameObject.SetActive(false);
                gameManager.GoBackDimension();
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

        gameManager.DoFloatingText(transform.position, floatingText, gameManager.cCPrefab.classColor[input]);
        playerWeaponController.UpdateWeaponValues();
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
                InvokeRepeating("PlayerIsOutsideBorder", 2.2f, damageInterval);
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

                // if the player is invulnerability 
                if (canTakeDamge == true || enemyHealth.canPoolObject == false)
                {
                    // add a force after the collision to the player
                    playerRb.AddForce(explosionDirection * -1f * enemyHealth.explosionForce, ForceMode.Impulse);

                    // calculate player health
                    int damage = Protection(enemyHealth.collisonDamage);
                    UpdatePlayerHealth(damage);

                    // trigger the damage floating text
                    gameManager.DoFloatingText(transform.position, "+" + damage.ToString(), hitColor);

                    if (upgradeChooseList.upgrades[35].upgradeIndexInstalled > 0)  NovaOnHit(2f, 8);
                }
                else
                {
                    // add a force after the collision to the player
                    if (upgradeChooseList.upgrades[36].upgradeIndexInstalled > 0)
                    {
                        NovaOnHit(1.2f, 6);
                        playerRb.AddForce(explosionDirection * 1.4f * enemyHealth.explosionForce, ForceMode.Impulse);
                    }
                    else
                    {
                        gameManager.DoFloatingText(collision.transform.position, "+" + enemyHealth.enemyHealth, enemyHitColor);
                        playerRb.AddForce(explosionDirection * 1f * enemyHealth.explosionForce, ForceMode.Impulse);
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

    // recalculated incoming damage with the protection value - Player get 50% damage
    private int Protection(int damage)
    {
        int adjustedDamge = damage;
        int ran = Random.Range(0, 99);

        if (ran <= protectionPerc)
        {
            adjustedDamge = Mathf.RoundToInt((float)damage*0.5f);
            hitColor = enemyHitColorProtected;
        }
        else
        {
            hitColor = hitColorTemp;
        }

        return adjustedDamge;
    }

    /* **************************************************************************** */
    /* Movement Stuff-------------------------------------------------------------- */
    /* **************************************************************************** */
    #region movement stuff

    // control the basic player movement
    private void PlayerMovement()
    {
        if (gameManager.gameIsPlayed && !gameManager.gameOver)
        {
            // engine sound
            if (forwardInput != 0)
            {
                if (!engineAudioSource.isPlaying)
                    engineAudioSource.Play();

                // engine trail
                if (forwardInput > 0.1)
                {
                    foreach (ParticleSystem ep in engineParticles)
                    {
                        ep.Emit(1);
                    }
                }
            }
            else
            {
                engineAudioSource.Stop();
            }

            // fly backwards
            if (forwardInput < 0)
            {
                forwardInput *= 0.4f;
            }
            // boost
            else if (Input.GetButton("Boost"))
            {
               // boostValue = gameManager.UpdateEnergieSlider(boostValue);

                if (boostValue > 0)
                {
                    if (isBoost == false)
                    {
                        // power Boost
                        if (boostValue >= gameManager.energieSlider.maxValue * 0.90f)
                        {
                            powerBoostMarker.SetActive(true);
                            AudioManager.Instance.PlaySFX("PlayerBoostKick");
                       //     gameManager.boostFillArea.color = gameManager.boostColor;
                            playerMesh.DOLocalMoveZ(-0.1f, 0.1f);
                            boostParticle.Emit(80);
                            playerRb.AddForce(transform.forward * -speed * 30, ForceMode.Force);
                            gameManager.energieSlider.value = boostValue * 0.75f;
                            canTakeDamge = false;
                            Invoke("Invulnerability", upgradeChooseList.baseBoostInvulnerability);
                        }
                        isBoost = true;
                    }

                    float boostSpeed = boostPower;
                    forwardInput = boostSpeed;
                    boostParticle.Emit(1);
                    boostEngine.Emit(1);

                    if (canTakeDamge == false)
                    {
                        boostParticle.Emit(10);
                    }

                }
                else
                {
                    // set player Mesh positon to zero
                    setPosition = true;
                    canTakeDamge = true;
                }
            }


            // calculate movement
            playerRb.AddForce(forwardInput * (-speed) * transform.forward, ForceMode.Force);

            // calculate rotation
            PlayerRotation();



            // side step
            if (forwardInput < 0.3f && forwardInput > -0.3f)
            {
                Vector3 rightAngle = Quaternion.Euler(0, 90f, 0f) * transform.forward;
                playerRb.AddForce(rightAngle * horizontalInput2 * -speed * 0.75f);
                targetRotation2 = (horizontalInput2 * 10f);
            }
            else
            {
                targetRotation2 = 0;
            }
        }

    }

    private void Invulnerability()
    {
        canTakeDamge = true;
        powerBoostMarker.SetActive(false);
    }
    // Invoke to aktivate the BoostReaload()
    private void BoostReload()
    {
        //gameManager.BoostReload();
        boostParticle.Emit(20);
    }

    //calculate Player Rotation
    private void PlayerRotation()
    {
        if (playerMouseMode == true)
        {
            if (cameraController.flyModeToggle == true)
            {

                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 6));
                Vector3 directionToMouse = transform.position - mousePosition;
                directionToMouse.y = 0;

                if (directionToMouse != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * 60 * Time.deltaTime);
                }
            }
            else
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));
                Vector3 directionToMouse = (transform.position - mousePosition).normalized * 0.001f;
                directionToMouse.y = 0;

                if (directionToMouse != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * 25 * Time.deltaTime);
                }
            }
        }
        else
        {
            transform.Rotate(0f, horizontalInput * rotateSpeed, 0f);
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
            if (Random.Range(0, 100) <= upgradeChooseList.chanceToGetTwoExp)
            {
                gameManager.DoFloatingText(transform.position, "+", Color.white);
                exp = 2;
            }
        }

        //chance to get one health
        if (upgradeChooseList.chanceToGet1Health > 0)
        {
            if (Random.Range(0, 100) <= upgradeChooseList.chanceToGet1Health)
            {
                gameManager.DoFloatingText(transform.position, "+1", Color.green);
                UpdatePlayerHealth(-1);
            }
        }

        playerCurrentExperience += exp;
        bool isLevelUp;

        // if level up
        if (playerCurrentExperience >= playerExperienceToLevelUp)
        {
            Time.timeScale = 0;
            AudioManager.Instance.PlaySFX("LevelUp");
            engineAudioSource.Stop();

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

    public void PlayerWeaponUpdatePickup()
    {

        Time.timeScale = 0;
        AudioManager.Instance.PlaySFX("LevelUp");
        engineAudioSource.Stop();

        int temphealth = playerMaxHealth - playerCurrentHealth;
        UpdatePlayerHealth(-temphealth);

        // refresh player UI
        gameManager.PlayerWeaponUpdatePickup();
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

    #endregion


    /* **************************************************************************** */
    /* Weapon Stuff---------------------------------------------------------------- */
    /* **************************************************************************** */
    #region weapon stuff
    // set the main weapon particle damage
    public void SetBulletDamage()
    {
        playerWeaponController.WeaponChoose();
        playerMWController.UpdateBulletValues();
    }

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
                        if (upgradeChooseList.upgrades[54].upgradeIndexInstalled > 0)
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
        navigationController.SetTargetPosition();
    }
    #endregion

}

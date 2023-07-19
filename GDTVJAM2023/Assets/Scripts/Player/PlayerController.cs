using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Player Intro")]
    public float startImpulse = 100;
    private bool isIntro = true;
    private bool isStartSound = false;
    private float introTargetY = 6f;


    [Header("Player Stats")]
    public float speed;
    public float rotateSpeed = 0.25f;
    public int playerMaxHealth = 10;
    public int playerBulletBaseDamage = 2;
    public float playerFireRate = 0.5f;
    public float pickupRange = 2f;
    public float boostValue = 1f;
    public float boostPower = 3f;
    public float protectionPerc = 0;
    public int protectionLvl = 0;


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
    private float currentRotationX;
    private float targetRotation2 = 0;


    [Header("Outside Border")]
    public float damageInterval = 1f;
    public int damageTaken = 2;
    public bool isOutsideBorder = false;


    [Header("Floating Text")]
    public List<ParticleCollisionEvent> collisionEvents; // creating a list to store the collision events
    public Color hitColor = new Color(1f, 0.0f, 0.0f, 1f);


    [Header("Game Objects")]
    public NavigationController navigationController;
    public AudioSource engineAudioSource;
    public ParticleSystem boostParticle;
    public Transform playerMesh;
    private Rigidbody playerRb;
    private GameManager gameManager;
    private PlayerMWController playerMWController;
    private PlayerWeaponController playerWeaponController;




    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    void Start()
    {
        // set game objects
        playerRb = GetComponent<Rigidbody>();
        playerMWController = GetComponent<PlayerMWController>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

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
            isIntro = true;
        }
        else
        {
            if (isStartSound == false)
            {
                AudioManager.Instance.PlaySFX("ShortAlert");
                isStartSound = true;
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


            if (Input.GetButtonUp("Boost"))
            {
                CancelInvoke("BoostReload");
                Invoke("BoostReload", 1f);
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

            // rotate Playermesh
            float targetRotationX = originalRotationX - (horizontalInput * 20f) - targetRotation2;
            currentRotationX = Mathf.Lerp(currentRotationX, targetRotationX, Time.deltaTime * 15f);
            playerMesh.localRotation = Quaternion.Euler(currentRotationX, transform.rotation.y + 90f, transform.rotation.z);

        }


    }


    /* **************************************************************************** */
    /* Collision Stuff------------------------------------------------------------- */
    /* **************************************************************************** */
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

            case "SwarmPickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdateClassLevel(4);
                break;

            case "DefensePickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdateClassLevel(5);
                break;

            case "TargetingPickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdateClassLevel(6);
                break;

            case "BackwardsPickup":
                ObjectPoolManager.ReturnObjectToPool(other.gameObject);
                UpdateClassLevel(7);
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
                playerWeaponController.mcBulletLvl++;
                floatingText = "+1 bullet class";
                break;
            case 1:
                playerWeaponController.mcExplosionLvl++;
                floatingText = "+1 explosion class";
                break;
            case 2:
                playerWeaponController.mcLaserLvl++;
                floatingText = "+1 laser class";
                break;
            case 3:
                playerWeaponController.mcSupportLvl++;
                floatingText = "+1 support class";
                break;
            case 4:
                playerWeaponController.scSwarmLvl++;
                floatingText = "+1 swarm class";
                break;
            case 5:
                playerWeaponController.scDefenceLvl++;
                floatingText = "+1 defense class";
                break;
            case 6:
                playerWeaponController.scTargetingLvl++;
                floatingText = "+1 targeting class";
                break;
            case 7:
                playerWeaponController.scBackwardsLvl++;
                floatingText = "+1 backwards class";
                break;
        }

        gameManager.DoFloatingText(transform.position, floatingText, gameManager.globalClassColor[input]);
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
                InvokeRepeating("PlayerIsOutsideBorder", 1f, damageInterval);
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

            // find the right explosion direction
            Vector3 explosionDirection = collision.transform.position - transform.position;
            explosionDirection.Normalize();

            // trigger the damage floating text
            gameManager.DoFloatingText(transform.position, "+" + enemyHealth.collisonDamage.ToString(), hitColor);

            // add a force after the collision to the player
            playerRb.AddForce(explosionDirection * -1f * enemyHealth.explosionForce, ForceMode.Impulse);

            // trigger a Explosion on the Enemy
            ObjectPoolManager.SpawnObject(enemyHealth.dieExplosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

            // calculate player health
            int damage = Mathf.Max(enemyHealth.collisonDamage - Mathf.RoundToInt(enemyHealth.collisonDamage * protectionPerc / 100), 1);
            UpdatePlayerHealth(damage);

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




    /* **************************************************************************** */
    /* Movement Stuff-------------------------------------------------------------- */
    /* **************************************************************************** */
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
            }
            else
            {
                engineAudioSource.Stop();
            }

            // fly backwards
            if (forwardInput < 0)
            {
                forwardInput *= 0.25f;
            }
            // boost
            else if (Input.GetButton("Boost"))
            {
                boostValue = gameManager.UpdateBoostSlider(boostValue);

                if (boostValue > 0)
                {
                    if (isBoost == false)
                    {
                        //gameManager.ScreenShake(5);
                        if (boostValue >= gameManager.boostSlider.maxValue * 0.9)
                        {
                            AudioManager.Instance.PlaySFX("PlayerBoostKick");
                            //playerMesh.localPosition = new Vector3(0, 0, -0.1f);
                            playerMesh.DOLocalMoveZ(-0.1f, 0.1f);
                            boostParticle.Emit(80);
                            playerRb.AddForce(transform.forward * -speed * 30, ForceMode.Force);
                            gameManager.boostSlider.value = boostValue * 0.75f;
                        }
                        isBoost = true;
                    }

                    float boostSpeed = boostPower;
                    forwardInput = boostSpeed;
                    boostParticle.Emit(1);

                }
                else
                {
                    // set player Mesh positon to zero
                    setPosition = true;
                }
            }

            // calculate movement
            playerRb.AddForce(forwardInput * (-speed) * transform.forward, ForceMode.Force);
            transform.Rotate(0f, horizontalInput * rotateSpeed, 0f);

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

    // Invoke to aktivate the BoostReaload()
    private void BoostReload()
    {
        gameManager.BoostReload();
        boostParticle.Emit(20);
    }




    /* **************************************************************************** */
    /* Health and Experience------------------------------------------------------- */
    /* **************************************************************************** */
    // update the player experience
    private void UpdatePlayerExperience()
    {
        // get + 1 experience
        playerCurrentExperience += 1;
        bool isLevelUp;

        // if level up
        if (playerCurrentExperience == playerExperienceToLevelUp)
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
        gameManager.UpdateUIPlayerExperience(isLevelUp, playerLevel, playerExperienceToLevelUp, playerCurrentExperience);
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




    /* **************************************************************************** */
    /* Main weapon----------------------------------------------------------------- */
    /* **************************************************************************** */
    // set the main weapon particle damage
    public void SetBulletDamage()
    {
        playerMWController.UpdateBulletValues();
    }




    /* **************************************************************************** */
    /* Fly Control----------------------------------------------------------------- */
    /* **************************************************************************** */
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


}

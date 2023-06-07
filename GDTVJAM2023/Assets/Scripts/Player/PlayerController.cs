using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

   
    [Header("Properties")]
    public List <ParticleSystem> mainWeapons = new List<ParticleSystem>();
    public List <ParticleBullet> particleBullets;
    public int playerCurrentHealth= 10;
    private int playerCurrentExperience;
    private int playerExperienceToLevelUp = 6;
    private int playerLevel = 1;
    private float playerLevelUpFactor = 1.2f;
    public string shootSound;


    [Header("Outside Border")]
    public float damageInterval = 1f;
    public int damageTaken = 2;
    public bool isOutsideBorder = false;


    [Header("Shield Controll")]
    public bool isFrontShield = false;
    public bool isBackShieldLeft = false;
    public bool isBackShieldRight = false;

    [Header("Floating Text")]
    public List<ParticleCollisionEvent> collisionEvents; // creating a list to store the collision events
    public Color hitColor = new Color(1f, 0.0f, 0.0f, 1f);


    [Header("Game Objects")]
    public NavigationController navigationController;
    public AudioSource mainWeaponSound;
    public AudioSource engineAudioSource;
    private Rigidbody playerRb;
    private GameManager gameManager;




    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    void Start()
    {
        // set game objects
        playerRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // intro starting sound
        AudioManager.Instance.PlaySFX("LiftUPBoss");
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
        {   if (isStartSound == false)
            {
                AudioManager.Instance.PlaySFX("ShortAlert");
                isStartSound = true;
                playerRb.AddForce(gameObject.transform.forward * -speed * startImpulse * Time.deltaTime, ForceMode.Force);
                // set .y to 6f
                playerRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                // start Shooting
                StartShooting();
            }
            isIntro = false;
        }

        // Game Control
        if (isIntro == false)
        {
            PlayerMovement();
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
                    isOutsideBorder = false;
                }
                break;
        }
    }

    // activate if trigger stay - border control
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BorderCollider"))
        {
            if (isOutsideBorder == true)
            {
                CancelInvoke("PlayerIsOutsideBorder");
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
                gameManager.outsideBorderText.text = "outside border!";
                isOutsideBorder = true;
                Debug.Log(isOutsideBorder);
            }
        }
    }

    // activate if collide with an Enemy
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
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
            UpdatePlayerHealth(enemyHealth.collisonDamage);
            
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
            // get Input values from the user
            float forwardInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

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
            
            // calculate movement
            playerRb.AddForce(forwardInput * (-speed) * Time.deltaTime* gameObject.transform.forward, ForceMode.Force);
            gameObject.transform.Rotate(0f, horizontalInput * rotateSpeed,  0f);
        }
        
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
            playerLevel += 1;
            playerExperienceToLevelUp = Mathf.RoundToInt(playerExperienceToLevelUp * playerLevelUpFactor);
            playerCurrentExperience = 0;

            // player get +1 health und heal 25 % of his life
            playerMaxHealth = playerMaxHealth + 1;
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
    // the main weapon start to fire
    public void StartShooting()
    {
        // set the main weapon particle damage
        SetBulletDamage();

        // start invoke for main weapons
        InvokeRepeating("ShotEmit", 0.5f, playerFireRate);
    }

    // stop the main waepon fire
    public void StopShooting()
    {
        CancelInvoke("ShotEmit");
    }

    // invoke function - make the main weapon fire
    void ShotEmit()
    {
        // shooting sound
        mainWeaponSound.Play();
 
        // emit 1 particle of each mainweapon
        foreach (ParticleSystem mainWeapon in mainWeapons)
        {
            if (mainWeapon != null)
                mainWeapon.Emit(1);
        }
    }

    // set the main weapon particle damage
    public void SetBulletDamage()
    {
        foreach (ParticleBullet particle in particleBullets)
        {
            particle.BulletSetDamage(playerBulletBaseDamage);
        }
    }



    /* **************************************************************************** */
    /* Fly Control----------------------------------------------------------------- */
    /* **************************************************************************** */
    // the player take damage after he is outside the border
    private void PlayerIsOutsideBorder()
    {
        UpdatePlayerHealth(damageTaken);
    }

    // activate the navigation controller
    public void SetNavigationController()
    {
        navigationController.SetTargetPosition();
    }

}

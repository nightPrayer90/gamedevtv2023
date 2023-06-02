using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    public float speed;
    public float rotateSpeed = 0.25f;
    public int playerMaxHealth = 10;
    public int playerBulletBaseDamage = 2;
    public float playerFireRate = 0.5f;
    public float pickupRange = 2f;


    private Rigidbody playerRb;
    private GameManager gameManager;
    private float introTargetY = 6f;
    private bool isIntro = true;
    private bool isStartSound = false;
    [Header("Player Intro")]
    public float startImpulse = 100;
    public AudioSource engineAudioSource;

    [Header("Properties")]
    public List <ParticleBullet> particleBullets;
    public CameraController cameraController;
    public int playerCurrentHealth= 10;
    public int playerCurrentExperience;
    public int playerExperienceToLevelUp = 15;
    public int playerLevel = 1;
    public float playerLevelUpFactor = 1.2f;
    private bool isShooting = false;
    
    [Header("Outside Border")]
    public float damageInterval = 1f;
    public int damageTaken = 2;
    private float nextDamageTime = 0f;
    public bool isOutsideBorder = false;

    [Header("Shield Controll")]
    public bool isFrontShield = false;
    public bool isBackShieldLeft = false;
    public bool isBackShieldRight = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //StartShooting();
        AudioManager.Instance.PlaySFX("LiftUPBoss");
        engineAudioSource = GetComponent<AudioSource>();
    }

    // Aktualisiere die Y-Position des Spielers auf 6
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
            }
            isIntro = false;
        }

        // Game Control
        if (isIntro == false)
        {
            PlayerMovement();
            PlayerIsOutsideBorder();

            if (gameManager.gameIsPlayed && !isShooting)
            {
                StartShooting();
            }
        }
    }

    private void UpdatePlayerExperience()
    {
        playerCurrentExperience += 1;
        bool isLevelUp;

        if (playerCurrentExperience == playerExperienceToLevelUp)
        {
            Time.timeScale = 0;
            AudioManager.Instance.PlaySFX("LevelUp");

            playerLevel += 1;
            playerExperienceToLevelUp = Mathf.RoundToInt(playerExperienceToLevelUp * playerLevelUpFactor);
            playerCurrentExperience = 0;

            playerMaxHealth = playerMaxHealth + 1;
            int temphealth = Mathf.RoundToInt(playerMaxHealth * 0.25f);

            UpdatePlayerHealth(-temphealth);

            StopShooting();
            engineAudioSource.Stop();
            isLevelUp = true;
        }
        else
        {
            // play collect sound
            AudioManager.Instance.PlaySFX("ExperienceOrb");
            isLevelUp = false;
        }

        gameManager.UpdateUIPlayerExperience(isLevelUp, playerLevel, playerExperienceToLevelUp, playerCurrentExperience);
    }





    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Exp"))
        {
            Destroy(other.gameObject);
            UpdatePlayerExperience();
            
        }
        if (other.gameObject.CompareTag("DimensionPickUp"))
        {
            //Destroy(other.gameObject);
            other.gameObject.SetActive(false);
            gameManager.GoToDimension();
            AudioManager.Instance.PlaySFX("DimensionSwap");
        }
        if (other.gameObject.CompareTag("DimensionPickUpGoBack"))
        {
            //Destroy(other.gameObject);
            other.gameObject.SetActive(false);
            gameManager.GoBackDimension();
            AudioManager.Instance.PlaySFX("DimensionSwap");
        }
        if (other.gameObject.CompareTag("VictoryPickup"))
        {
            //Destroy(other.gameObject);
            other.gameObject.SetActive(false);
            gameManager.Victory();
            AudioManager.Instance.PlaySFX("VictorySound");
        }
        if (other.gameObject.CompareTag("BorderCollider"))
        {
            gameManager.outsideBorderText.text = "";
            isOutsideBorder = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BorderCollider") && isIntro == false)
        {
            isOutsideBorder = true;
            gameManager.outsideBorderText.text = "outside border!";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {

            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            Vector3 explosionDirection = collision.transform.position - transform.position;
            explosionDirection.Normalize();

            playerRb.AddForce(explosionDirection * -1f * enemyHealth.explosionForce, ForceMode.Impulse);
            UpdatePlayerHealth(enemyHealth.collisonDamage);
            
            Instantiate(enemyHealth.dieExplosionObject, transform.position, transform.rotation);

            if (enemyHealth.secondDimensionEnemy == false)
            {
                gameManager.UpdateEnemyCounter(-1);
                gameManager.UpdateEnemyToKill(1);
            }
            Destroy(collision.gameObject);
        }
    }

    private void PlayerMovement()
    {
        if (gameManager.gameIsPlayed && !gameManager.gameOver)
        {
            float forwardInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            if (forwardInput > 0)
            {
                engineAudioSource.Play();
            }
            else
            {
                engineAudioSource.Stop();
            }

            if (forwardInput < 0)
            {
                forwardInput *= 0.25f;
            }

            playerRb.AddForce(gameObject.transform.forward * -speed * forwardInput * Time.deltaTime, ForceMode.Force);
            gameObject.transform.Rotate(0f, horizontalInput * rotateSpeed,  0f);
            
            //Korrektur der y-Achse
            Vector3 newPosition = playerRb.position;
            newPosition.y = 6f;
            playerRb.position = newPosition;
        }
        
    }

    public void UpdatePlayerHealth(int decHealth)
    {
        if (decHealth > 0) // man verliert leben
        {
            cameraController.ShakeScreen();
            AudioManager.Instance.PlaySFX("PlayerGetDamage");
        }
        playerCurrentHealth = Mathf.Min(Mathf.Max(0, playerCurrentHealth - decHealth), playerMaxHealth);
        
        gameManager.UpdateUIPlayerHealth(playerCurrentHealth, playerMaxHealth);

        // player die
        if (playerCurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void StopShooting()
    {
        isShooting = false;

        int b = 0;
        foreach (ParticleBullet particle in particleBullets)
        {
            particleBullets[b].HardBulletStop();
            b++;
        }
    }

    public void StartShooting()
    {
        isShooting = true;

        int b = 0;
        foreach (ParticleBullet particle in particleBullets)
        {
            particleBullets[b].BulletStart(playerBulletBaseDamage, playerFireRate);
            b++;
        }
    }

    private void PlayerIsOutsideBorder()
    {
        if (isOutsideBorder)
        { 
            if (Time.time >= nextDamageTime)
            {
                UpdatePlayerHealth(damageTaken);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

}

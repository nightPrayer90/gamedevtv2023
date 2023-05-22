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

    [Header("Properties")]
    public List <ParticleBullet> particleBullets;
    public int playerCurrentHealth= 10;
    public int playerCurrentExperience;
    public int playerExperienceToLevelUp = 15;
    public int playerLevel = 1;
    public float playerLevelUpFactor = 1.2f;
    private bool isShooting = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        StartShooting();


    }

    // Aktualisiere die Y-Position des Spielers auf 6
    private void FixedUpdate()
    {
        playerMovement();
        Vector3 newPosition = playerRb.position;
        newPosition.y = 6f;
        playerRb.position = newPosition;

        if (gameManager.gameIsPlayed && !isShooting)
        {
            StartShooting();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Exp"))
        {
            Destroy(other.gameObject);
            playerCurrentExperience += 1;
            gameManager.UpdatePlayerExperience();
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

            Destroy(collision.gameObject);
        }
    }

    private void playerMovement()
    {
        if (gameManager.gameIsPlayed && !gameManager.gameOver)
        {
            float forwardInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            if (forwardInput < 0)
            {
                forwardInput *= 0.2f;
            }

            playerRb.AddForce(gameObject.transform.forward * -speed * forwardInput * Time.deltaTime, ForceMode.Force);
            gameObject.transform.Rotate(0f, horizontalInput * rotateSpeed,  0f);
        }
    }

    public void UpdatePlayerHealth(int decHealth)
    {
        
        playerCurrentHealth = Mathf.Min(Mathf.Max(0, playerCurrentHealth - decHealth), playerMaxHealth);
        
        gameManager.UpdatePlayerHealth();

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

}

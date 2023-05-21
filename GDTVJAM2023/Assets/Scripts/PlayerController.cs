using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float rotateSpeed = 0.25f;
    private Rigidbody playerRb;
    private GameManager gameManager;
    public int playerMaxHealth = 10;
    public int playerCurrentHealth= 10;
    public int playerCurrentExperience;
    public int playerExperienceToLevelUp = 15;
    public int playerLevel = 1;
    public float playerLevelUpFactor = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
    }
    // Aktualisiere die Y-Position des Spielers auf 6
    private void FixedUpdate()
    {
        Vector3 newPosition = playerRb.position;
        newPosition.y = 6f;
        playerRb.position = newPosition;
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
            UpdatePlayerHealth(enemyHealth.explosionDamage);

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

            playerRb.AddForce(gameObject.transform.forward * -speed * forwardInput, ForceMode.Force);
            gameObject.transform.Rotate(0f, horizontalInput * rotateSpeed,  0f);
        }
    }

    public void UpdatePlayerHealth(int decHealth)
    {
        playerCurrentHealth -= decHealth;
        gameManager.UpdatePlayerHealth();

        // player die
        if (playerCurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

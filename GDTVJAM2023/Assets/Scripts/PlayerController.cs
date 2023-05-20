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
    public int playerCurrentHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        
        playerCurrentHealth = playerMaxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
       
    }

    void playerMovement()
    {
        if (gameManager.gameIsPlayed)
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
    }

    // Aktualisiere die Y-Position des Spielers auf 6
    private void FixedUpdate()
    {
        Vector3 newPosition = playerRb.position;
        newPosition.y = 6f;
        playerRb.position = newPosition;
    }
}

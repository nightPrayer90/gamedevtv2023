using UnityEngine;
using System.Collections.Generic;

public class SimpleEnemy : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    

    //public GameObject explosionObject;

    private Rigidbody playerRb;
    private PlayerController playerController;
    private GameManager gameManager;
    

    // Start is called before the first frame update
    void Start()
    {
        //enemyRb = GetComponent<Rigidbody>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();

    }

    void EnemyMovement()
    {
        if (gameManager.gameIsPlayed && !gameManager.gameOver && !gameManager.dimensionShift)
        {

            Vector3 playerPosition = playerRb.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);

            Vector3 targetDirection = playerPosition - transform.position;
            targetDirection.y = 0f; // Nur in der horizontalen Ebene drehen
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            targetRotation *= Quaternion.Euler(0f, 180f, 0f);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
  
}

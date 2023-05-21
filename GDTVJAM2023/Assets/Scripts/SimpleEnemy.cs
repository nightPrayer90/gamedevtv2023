using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public int explosionDamage = 1;
    public float explosionForce = 5.0f;

    public GameObject explosionObject;
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
        if (gameManager.gameIsPlayed && !gameManager.gameOver)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            Vector3 explosionDirection = collision.transform.position - transform.position;
            explosionDirection.Normalize();
            playerRb.AddForce(explosionDirection * explosionForce, ForceMode.Impulse);
            playerController.UpdatePlayerHealth(explosionDamage);

            Instantiate(explosionObject, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

    
}

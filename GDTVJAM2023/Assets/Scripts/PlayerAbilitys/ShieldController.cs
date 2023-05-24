using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private Transform playerTr;
    private PlayerController playerController;
    public Vector3 rotationOffset;
    private GameManager gameManager;
    private Rigidbody playerRb;

    public Material targetMaterial;

    public int shieldLife = 10;
    private Color albedoColor;

    //private Vector3 initialOffset;
    private void Start()
    {
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.isFrondshield = true;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerRb = playerController.GetComponent<Rigidbody>();

        if (playerTr != null)
        {
            //initialOffset = transform.position - playerTr.position;
        }

        albedoColor = targetMaterial.GetColor("_Color");
        
        float newAlpha = 0.5f; 
        albedoColor.a = newAlpha;
        targetMaterial.SetColor("_Color", albedoColor);
    }

    private void LateUpdate()
    {
        if (playerTr != null)
        {
            transform.position = playerTr.position;// + initialOffset;

            Quaternion targetRotation = Quaternion.Euler(rotationOffset);
            transform.rotation = playerTr.rotation * targetRotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {

            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            Vector3 explosionDirection = collision.transform.position - transform.position;
            explosionDirection.Normalize();

            playerRb.AddForce(explosionDirection * -1f * (enemyHealth.explosionForce/4), ForceMode.Impulse);
            playerController.isFrondshield = false;

            Instantiate(enemyHealth.dieExplosionObject, transform.position, transform.rotation);

            gameManager.UpdateEnemyCounter(-1);

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    public void UpdateShieldHealth(int decHealth)
    {
        shieldLife = Mathf.Max(0, shieldLife - decHealth);

        float newAlpha = 0.5f - (0.5f/ shieldLife);
        albedoColor.a = newAlpha;
        targetMaterial.SetColor("_Color", albedoColor);


        if (shieldLife <= 0)
        {
            Destroy(gameObject);
        }
    }
}
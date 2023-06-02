using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private Transform playerTr;
    private PlayerController playerController;
    private GameManager gameManager;
    private Rigidbody playerRb;
    private ShieldSpawner shieldSpawner;

    public Material targetMaterial;
    private Color albedoColor;

    public int shieldLife = 10;
    public bool isBackShield = false;
    public bool isBackShieldLeft = false;
    public bool isBackShieldRight = false;

    

    //private Vector3 initialOffset;
    private void Start()
    {
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerRb = playerController.GetComponent<Rigidbody>();

        if (isBackShield == false)
        {
            playerController.isFrontShield = true;
            shieldSpawner = GameObject.Find("front Shield").GetComponent<ShieldSpawner>();
        }
        else
        {
            if (isBackShieldLeft == true)
                playerController.isBackShieldLeft = isBackShieldLeft;
            else
                playerController.isBackShieldRight = isBackShieldRight;
            shieldSpawner = GameObject.Find("back Shield").GetComponent<ShieldSpawner>();
        }


        Color albedoColor = targetMaterial.GetColor("_Color");
        float newAlpha = 0.5f; 
        albedoColor.a = newAlpha;
        targetMaterial.SetColor("_Color", albedoColor);
    }

    private void LateUpdate()
    {
        if (playerTr != null)
        {
            transform.position = playerTr.position;
            transform.rotation = playerTr.rotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {

            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            Transform eneymTransform = collision.gameObject.GetComponent<Transform>();

            Vector3 explosionDirection = collision.transform.position - transform.position;
            explosionDirection.Normalize();

            playerRb.AddForce(explosionDirection * -1f * (enemyHealth.explosionForce/4), ForceMode.Impulse);

            if (isBackShield == false)
                playerController.isFrontShield = false;
            else
            {
                if (isBackShieldLeft == true)
                    playerController.isBackShieldLeft = false;
                else
                    playerController.isBackShieldRight = false;
            }
            shieldSpawner.nextSpawnTime = Time.time + shieldSpawner.spawnInterval;

            Instantiate(enemyHealth.dieExplosionObject, eneymTransform.position, eneymTransform.rotation);

            gameManager.UpdateEnemyCounter(-1);

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    public void UpdateShieldHealth(int decHealth)
    {
        shieldLife = Mathf.Max(0, shieldLife - decHealth);

        Color albedoColor = targetMaterial.GetColor("_Color");
        float newAlpha = 0.5f - (0.5f/shieldLife);
        albedoColor.a = newAlpha;
        targetMaterial.SetColor("_Color", albedoColor);


        if (shieldLife <= 0)
        {
            Destroy(gameObject);
        }

        if (isBackShield == false)
            playerController.isFrontShield = false;
        else
        {
            if (isBackShieldLeft == true)
                playerController.isBackShieldLeft = false;
            else
                playerController.isBackShieldRight = false;
        }
        shieldSpawner.nextSpawnTime = Time.time + shieldSpawner.spawnInterval;

    }
}
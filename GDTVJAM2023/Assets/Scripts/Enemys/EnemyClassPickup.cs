using UnityEngine;

public class EnemyClassPickup : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float detectionRange = 0.5f;
    public ParticleSystem main_Part;
    public ParticleSystem sub_Part;

    private PlayerController playercontroller;
    private Rigidbody playerRb;
    private GameManager gameManager;
    private bool ifcollect = false;
    private Color classColor; 


    void OnEnable()
    {
        var player = GameObject.FindWithTag("Player");
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        ParticleSystem.MainModule mainModuleMP = main_Part.main;
        ParticleSystem.MainModule mainModuleSP = sub_Part.main;

        int randomNumber = Random.Range(0, 8);
        classColor = gameManager.globalClassColor[randomNumber];
        mainModuleMP.startColor = classColor;
        mainModuleSP.startColor = classColor;

        switch (randomNumber)
        {
            case 0:
                gameObject.tag = "BulletPickup";
                break;
            case 1:
                gameObject.tag = "ExplosionPickup";
                break;
            case 2:
                gameObject.tag = "LaserPickup";
                break;
            case 3:
                gameObject.tag = "SupportPickup";
                break;
            case 4:
                gameObject.tag = "SwarmPickup";
                break;
            case 5:
                gameObject.tag = "DefensePickup";
                break;
            case 6:
                gameObject.tag = "TargetingPickup";
                break;
            case 7:
                gameObject.tag = "BackwardsPickup";
                break;
        }



        if (player != null)
        {
            playercontroller = player.GetComponent<PlayerController>();
            playerRb = player.GetComponent<Rigidbody>();
            detectionRange = playercontroller.pickupRange;
        }
        else 
        {
            detectionRange = 5;
        }
        ifcollect = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerRb.transform.position);

        if (distanceToPlayer < detectionRange)
            ifcollect = true;

        if (ifcollect == true)
        {
            Vector3 directionToPlayer = (playerRb.transform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToPlayer * moveSpeed * Time.deltaTime;
            transform.position = newPosition;
        }
    }
}

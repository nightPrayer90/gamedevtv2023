using UnityEngine;

public class EnemyExp : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float detectionRange = 0.5f;

    private PlayerController playercontroller;
    private Rigidbody playerRb;
    private bool ifcollect = false;

    // Start is called before the first frame update
   
    void OnEnable()
    {
        var player = GameObject.FindWithTag("Player");

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

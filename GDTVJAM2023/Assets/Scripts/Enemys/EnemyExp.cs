using UnityEngine;

public class EnemyExp : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float detectionRange = 0.5f;
    public int expValue = 1;

    private NewPlayerController playercontroller;
    private Transform playerTransform;
    private bool ifcollect = false;

    // Start is called before the first frame update
   
    void OnEnable()
    {
        var player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playercontroller = player.GetComponent<NewPlayerController>();
            playerTransform = player.GetComponent<Transform>();
            
        }
        else 
        {
            detectionRange = 5;
        }

        InvokeRepeating("DistanceToPlayer",0,0.2f);
        ifcollect = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ifcollect == true)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToPlayer * moveSpeed * Time.deltaTime;
            transform.position = newPosition;
        }
    }

    private void DistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        detectionRange = playercontroller.pickupRange;

        if (distanceToPlayer < detectionRange)
        {
            ifcollect = true;
            CancelInvoke("DistanceToPlayer");
        }
    }

    private void OnDisable()
    {
        CancelInvoke("DistanceToPlayer");
    }
}

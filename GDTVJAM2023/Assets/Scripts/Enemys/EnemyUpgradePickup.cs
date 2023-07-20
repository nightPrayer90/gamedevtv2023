using UnityEngine;

public class EnemyUpgradePickup : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float detectionRange = 0.5f;

    private Transform playerTransform;
    private bool ifcollect = false;

    // Start is called before the first frame update
   
    void OnEnable()
    {

        //playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
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
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

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

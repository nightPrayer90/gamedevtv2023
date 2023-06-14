using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public GameObject particles;
    public int explosionDamage= 5;

    public float speed = 5f; // Geschwindigkeit der Bewegung
    public float jumpForce = 5f; // Sprungkraft

    private Rigidbody rb;
    private LayerMask layerMask;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Layermask
        layerMask = (1 << 6); //| (1 << 3);
        
    }

    private void FixedUpdate()
    {
        // Horizontale Bewegung mit den Pfeiltasten
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(moveHorizontal, 0f, 0f);
        rb.AddForce(movement * speed);

        // Sprung mit der Leertaste
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        // postion of explosion Object
        Vector3 pos = transform.position;
        
        // array of all Objects in the explosionRadius
        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

        foreach (var obj in surroundingObjects)
        {
            // get rigidbodys from all objects in range
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            // calculate distance between explosioncenter and objects in Range
            float distance = Vector3.Distance(pos, rb.transform.position);
            //Debug.Log(distance);

            if (distance < explosionRadius)
            {
                float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
                int adjustedDamage = Mathf.CeilToInt(explosionDamage * scaleFactor);

                // get EnemyHealthscript
                EnemyHitControl eHC = obj.GetComponent<EnemyHitControl>();
                Debug.Log("SF + " + scaleFactor + " | distance " + distance);

                // calculate enemy damage
                eHC.takeDamage(adjustedDamage);
            }
           
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // explosion Effect
        GameObject go = Instantiate(particles, transform.position, Quaternion.identity);
        go.transform.localScale *= explosionRadius;

        // return to pool!
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

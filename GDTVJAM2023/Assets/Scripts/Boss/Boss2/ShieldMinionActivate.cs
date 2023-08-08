using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMinionActivate : MonoBehaviour
{
    public EnemyShield enemyShield;
    public Rigidbody rb;
    private Boss2upPhase upPhase;
    private Boss02 boss02;
    private bool isShieldActive_ = false;
    private bool isSetConstraints = true;


    private void Start()
    {
        upPhase = GameObject.Find("Boss2_upPart").GetComponent<Boss2upPhase>();
        boss02 = GameObject.Find("Boss 02").GetComponent<Boss02>();
        Invoke("InvokeCanConstraints", 2f);
    }

    private void OnEnable()
    {
        rb.AddForce(transform.right * 360f, ForceMode.Impulse);
    }
    private void InvokeCanConstraints()
    {
        isSetConstraints = false;
    }

    private void Update()
    {
        if (upPhase.isShieldActive == true && isShieldActive_ == false)
        {
            if (Vector3.Distance(transform.position, boss02.gameObject.transform.position) > 2f)
            {
                enemyShield.ActivateShield();
                isShieldActive_ = true;
            }
            else 
            {
                Destroy(gameObject);
            }
        }

        if (rb.velocity.magnitude < 0.1 && isSetConstraints == false)
        {
            if (Vector3.Distance(transform.position, boss02.gameObject.transform.position) < 2f)
            {
                rb.AddForce(transform.right * 300f, ForceMode.Impulse);
            }
            else
            {
                isSetConstraints = true;
                rb.constraints = RigidbodyConstraints.FreezePositionX |
                             RigidbodyConstraints.FreezePositionY |
                             RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationY |
                             RigidbodyConstraints.FreezeRotationZ;
            }
        }

        if (boss02.isDying == true)
        {
            enemyShield.ShieldDie();
            enabled = false;
        }
    }

}

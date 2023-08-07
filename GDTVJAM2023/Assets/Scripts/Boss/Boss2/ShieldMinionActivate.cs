using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMinionActivate : MonoBehaviour
{
    public EnemyShield enemyShield;
    public Rigidbody rb;
    private Boss2upPhase upPhase;
    private bool isShieldActive_ = false;
    private bool isSetConstraints = true;


    private void Start()
    {
        upPhase = GameObject.Find("Boss2_upPart").GetComponent<Boss2upPhase>();
        Invoke("InvokeCanConstraints", 2f);
    }

    private void InvokeCanConstraints()
    {
        isSetConstraints = false;
    }

    private void Update()
    {
        if (upPhase.isShieldActive == true && isShieldActive_ == false)
        {
            enemyShield.ActivateShield();
            isShieldActive_ = true;
        }

        if (rb.velocity.magnitude < 0.1 && isSetConstraints == false)
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

}

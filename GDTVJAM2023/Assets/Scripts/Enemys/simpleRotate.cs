using UnityEngine;

public class simpleRotate : MonoBehaviour
{
    public float torque = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddTorque(Vector3.up * torque);
    }
}

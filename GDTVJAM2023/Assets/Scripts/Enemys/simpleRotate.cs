using UnityEngine;

public class simpleRotate : MonoBehaviour
{
    public float torque = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log(rb);
    }

    void FixedUpdate()
    {
        rb.AddTorque(Vector3.up * torque);
    }
}

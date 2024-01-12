using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public float horizontalInput2;

    private float flySpeed;
    [SerializeField] private Rigidbody playerRigidbody;

    //debug
    public GameObject centerOfMass;

    // Start is called before the first frame update
    void Awake()
    {
        playerRigidbody.centerOfMass = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFlySpeed();

        HandleInput();

        //debug
        //masseanzeiger
        centerOfMass.transform.localPosition = playerRigidbody.centerOfMass + new Vector3(0f,1f,0f);
    }

    void HandleInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        horizontalInput2 = Input.GetAxis("Horizontal2");
    }

    void CalculateFlySpeed()
    {
        flySpeed = playerRigidbody.velocity.magnitude;

        // Hier kannst du die berechnete Geschwindigkeit verwenden oder ausgeben
        Debug.Log("Speed: " + flySpeed);
    }
}

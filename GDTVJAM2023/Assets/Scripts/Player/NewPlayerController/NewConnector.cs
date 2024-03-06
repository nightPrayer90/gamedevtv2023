using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewConnector : MonoBehaviour
{
    public float mass = 0.1f;
    public int playerHealth = 1;
    private NewPlayerController playerController;
    private Rigidbody playerRigidbody;


    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<NewPlayerController>();
        playerRigidbody = playerController.GetComponent<Rigidbody>();
        playerRigidbody.mass += mass;
        playerController.playerMaxHealth += playerHealth;
        playerController.playerCurrentHealth += playerHealth;

        // Change the center of Mass
        playerRigidbody.centerOfMass = playerRigidbody.centerOfMass + new Vector3((transform.localPosition.x / 2)* mass, 0f, (transform.localPosition.z/2)*mass);
    }
}

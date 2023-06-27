using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalLaser : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 10f;
    private GameObject player;
    private Transform playerTransform;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        
    }

    private void Update()
    {
        transform.position = playerTransform.position;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}

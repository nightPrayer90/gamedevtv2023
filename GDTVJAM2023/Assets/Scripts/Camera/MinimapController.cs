using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    private Vector3 startTransform;
    private Transform playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        startTransform = transform.position;
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerController.position.x, 10, playerController.position.z);
        transform.rotation = Quaternion.Euler(90f, playerController.rotation.eulerAngles.y, 0f);
    }
}

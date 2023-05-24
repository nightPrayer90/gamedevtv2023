using UnityEngine;

public class GotoDimensions : MonoBehaviour
{

    private PlayerController playercontroller;
    private Rigidbody playerRb;
    private bool ifcollect = false;

    // Start is called before the first frame update
    void Start()
    {
        playercontroller = GameObject.Find("Player").GetComponent<PlayerController>();
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

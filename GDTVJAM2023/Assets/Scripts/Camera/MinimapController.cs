using UnityEngine;

public class MinimapController : MonoBehaviour
{
    private Vector3 startTransform;
    public Transform playerController;

    // Start is called before the first frame update
    void Start()
    {
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

using UnityEngine;
using DG.Tweening;


public class CameraControllerHangar : MonoBehaviour
{
    public GameObject playerStartPoint;
    public Vector3 cameraOffset_;
    private Vector3 cameraOffset;


    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = transform.position - cameraOffset_;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerStartPoint.transform.position + cameraOffset;
    }

}

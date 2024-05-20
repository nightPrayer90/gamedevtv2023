using UnityEngine;
using UnityEngine.InputSystem;


public class CameraRotate : MonoBehaviour
{

    public Transform target; 
    public float mouseSpeed = 0.1f; 
    public float orbitDamping = 10f; 
    public float distance = 5f;

    public float zoomDamping = 5f; // Damping for the zoom
    public float zoomSpeed = 2f;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;

    private Vector3 localRotation;
    private Vector2 lastMousePosition;
    public Camera mainCamera;
    private float targetFOV;

    void Start()
    {
        targetFOV = mainCamera.fieldOfView;
        transform.position = target.position - transform.forward * distance;
        localRotation = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.middleButton.isPressed)
        {
            // Calculate the mouse delta
            Vector3 mouseDelta = Mouse.current.position.ReadValue() - lastMousePosition;

            localRotation.x += -mouseDelta.y * mouseSpeed;
            localRotation.y += mouseDelta.x * mouseSpeed;

            // Clamp the rotation around the x-axis
            localRotation.x = Mathf.Clamp(localRotation.x, -20f, 50f);

            Quaternion qt = Quaternion.Euler(localRotation.x, localRotation.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, qt, Time.deltaTime * orbitDamping);

            // Update the camera position based on the new rotation
            transform.position = target.position - transform.forward * distance;
        }


        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            targetFOV -= scroll * zoomSpeed * Time.deltaTime;
            targetFOV = Mathf.Clamp(targetFOV, minZoom, maxZoom);
        }

        // Smoothly interpolate to the target FOV
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomDamping);

        // Update last mouse position
        lastMousePosition = Mouse.current.position.ReadValue();
    }
}

using UnityEngine;

public class FloatingMesh : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public float floatSpeed = 0.1f;
    public float floatAmount = 0.5f;
    public float floatOffset = 0.5f;

    private Vector3 startPosition;

    public float yOffset = 0;

    public Material[] inactivMaterials;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // rotation
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // floating
        float newPositionY = startPosition.y + Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatAmount;
        transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);

    }
}
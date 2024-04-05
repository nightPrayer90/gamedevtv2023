using UnityEngine;

public class ShopFloatingBoss : MonoBehaviour
{
    public float rotationSpeed = 20f; 
    public float floatSpeed = 0.1f; 
    public float floatAmount = 0.5f; 
    public float floatOffset = 0.5f; 

    private Vector3 startPosition;
    public ShopModuleContainer shopModuleContainer;
    private MeshRenderer meshRenderer;

    public float yOffset=0;

    public Material[] inactivMaterials;

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        startPosition = transform.position;

        if (shopModuleContainer.isActive == false)
        {
            transform.position = new Vector3(startPosition.x, yOffset, startPosition.z);

            meshRenderer.materials = inactivMaterials;
            enabled = false;
        }
    }

    void Update()
    {
        if (shopModuleContainer.isActive == true)
        {
            // rotation
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            // floating
            float newPositionY = startPosition.y + Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatAmount;
            transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
        }
    }
}
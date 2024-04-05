using UnityEngine;

public class ShopFloatingObject : MonoBehaviour
{
    public float floatSpeed = 0.1f; 
    public float floatAmount = 0.5f; 
    public float floatOffset = 0.5f; 

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newPositionY = startPosition.y + Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatAmount;
        transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
    }

    public void SetInActive(bool isActive) 
    {
        if (isActive == false)
            enabled = false;
    }
}
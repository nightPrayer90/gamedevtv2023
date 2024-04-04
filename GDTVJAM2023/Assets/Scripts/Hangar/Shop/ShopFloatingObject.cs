using UnityEngine;

public class ShopFloatingObject : MonoBehaviour
{
    public float rotationSpeed = 20f; 
    public float floatSpeed = 0.1f; 
    public float floatAmount = 0.5f; 
    public float floatOffset = 0.5f; 

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        // ToDo boss only activ if the Player destroy the boss in game
    }

    void Update()
    {
        // Rotation um die Y-Achse
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Bewegung auf und ab (Schweben)
        float newPositionY = startPosition.y + Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatAmount;
        transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
    }
}
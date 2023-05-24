using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float rotationStep = 5f;
    public NavigationController navigationController;

  
    private void Update()
    {
        Vector3 direction = navigationController.targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep * rotationSpeed * Time.deltaTime);
    }
}
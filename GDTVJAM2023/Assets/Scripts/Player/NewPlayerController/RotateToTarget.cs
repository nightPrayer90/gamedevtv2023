using UnityEngine;
using DG.Tweening;


public class RotateToTarget : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float rotationStep = 5f;
    public NavigationController navigationController;

    private void OnEnable()
    {
        transform.DOPunchScale(new Vector3(0.07f, 0.0f, 0.07f), 0.5f, 5, 0.5f).SetLoops(-1,LoopType.Restart);
    }

    private void OnDisable()
    {
        transform.DOKill(true);
    }

    private void OnDestroy()
    {
        transform.DOKill(true);
    }

    private void Update()
    {
        Vector3 direction = navigationController.targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Vector3 eulerRotation = targetRotation.eulerAngles;
        eulerRotation.x = 0f; // Setze die X-Komponente auf 0 (keine Drehung um die X-Achse)
        eulerRotation.z = 0f; // Setze die Z-Komponente auf 0 (keine Drehung um die Z-Achse)
        targetRotation = Quaternion.Euler(eulerRotation);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep * rotationSpeed * Time.deltaTime);  
    }
}
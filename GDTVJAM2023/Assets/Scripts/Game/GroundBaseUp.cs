using UnityEngine;

public class GroundBaseUp : MonoBehaviour
{
    public float targetYPosition = 5f;
    public float movementSpeed = 1f;

    private Vector3 targetPosition;
    private bool isGrowing = false;
    private void Start()
    {
        targetPosition = new Vector3(transform.position.x, targetYPosition, transform.position.z);
    }

    public void GrowUP()
    {
        isGrowing = true;
    }
    public void Update()
    {
        if (isGrowing == true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (distanceToTarget > 0.01f)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed / distanceToTarget);
                transform.position = newPosition;
            }
            else
            {
                isGrowing = false;
                AudioManager.Instance.PlaySFX("DestrictIsDone");
            }

        }
    }
}

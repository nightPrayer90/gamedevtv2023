using UnityEngine;

public class GroundBaseUp : MonoBehaviour
{
    public GameManager gameManager;

    public float targetYPosition = 5f;
    public float movementSpeed = 1f;

    private Vector3 targetPosition;
    private bool isGrowing = false;

    [Header("Dimension Shift")]
    public Material districtBase;
    public Material districtEmissive;
    public Material districtEmissiveReverse;
    public Material districtEmissiveClear;
    private MeshRenderer meshR;
    public int districtIndex;

    private void Start()
    {
        meshR = gameObject.GetComponent<MeshRenderer>();
        ChangeEmissiv(0,1);
        targetPosition = new Vector3(transform.position.x, targetYPosition, transform.position.z);
    }

    public void GrowUP()
    {
        ChangeEmissiv(0,0);
        isGrowing = true;
    }

    public void ChangeEmissiv(int typ, int currentDistrict)
    {
        Material[] materials = meshR.materials;
        switch(typ)
        {
            case 0: // normal dimension
                materials[0] = districtBase;
                if (currentDistrict <= districtIndex)
                {
                    materials[1] = districtEmissive;
                }
                else
                {
                    materials[1] = districtEmissiveClear;
                }
                break;

            case 1: // boss dimension
                materials[0] = districtBase;
                materials[1] = districtEmissiveReverse;
                break;
        }
        meshR.materials = materials;
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

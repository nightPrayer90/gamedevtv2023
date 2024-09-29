using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public GameManager gameManager;
    public SpawnDistrictList spawnDoistrictList;
    public GameObject navigatorPivot;
    public Vector3 targetPosition;


    public void SetTargetPosition()
    {
        // Position des GoToDimensionPickups
        targetPosition = spawnDoistrictList.goToDimensionPickup[gameManager.districtNumber - 1].transform.position;
        navigatorPivot.SetActive(true);
    }

    public void DeactivateNavigatorMesh()
    {
        navigatorPivot.SetActive(false);
    }
  
}
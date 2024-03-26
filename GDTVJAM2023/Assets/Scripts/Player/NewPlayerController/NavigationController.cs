using UnityEngine;

public class NavigationController : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnDistrictList spawnDoistrictList;
    public GameObject navigatorPivot;
    public Vector3 targetPosition;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnDoistrictList = GameObject.Find("Game Manager").GetComponent<SpawnDistrictList>();
    }

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